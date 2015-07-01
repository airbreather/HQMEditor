using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using QuestEditor.Messages;

namespace QuestEditor.ViewModels
{
    public sealed class QuestSetViewModel : ViewModelBase
    {
        public QuestSetViewModel()
            : this(-1, String.Empty, Enumerable.Empty<QuestViewModel>(), Enumerable.Empty<QuestLinkViewModel>())
        {
        }

        public QuestSetViewModel(int id, string name, IEnumerable<QuestViewModel> quests, IEnumerable<QuestLinkViewModel> questLinks)
        {
            this.AddQuestCommand = new RelayCommand(this.AddQuest);

            this.id = id;
            this.name = name.ValidateNotNull(nameof(name));

            this.Quests = new ReadOnlyObservableCollection<QuestViewModel>(this.questsMutable);
            this.QuestLinks = new ReadOnlyObservableCollection<QuestLinkViewModel>(this.questLinksMutable);

            foreach (var quest in quests.ValidateNotNull(nameof(quests)))
            {
                this.AddQuest(quest);
            }

            foreach (var questLink in questLinks.ValidateNotNull(nameof(questLinks)))
            {
                this.AddQuestLink(questLink.FromQuest, questLink.ToQuest);
            }
        }

        private int id;
        public int Id
        {
            get { return this.id; }
            set { this.Set(ref this.id, value); }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.Set(ref this.name, value); }
        }

        private string description;
        public string Description
        {
            get { return this.description; }
            set { this.Set(ref this.description, value); }
        }

        private readonly ObservableCollection<QuestViewModel> questsMutable = new ObservableCollection<QuestViewModel>();
        private readonly ObservableCollection<QuestLinkViewModel> questLinksMutable = new ObservableCollection<QuestLinkViewModel>();

        public ReadOnlyObservableCollection<QuestViewModel> Quests { get; }
        public ReadOnlyObservableCollection<QuestLinkViewModel> QuestLinks { get; }

        public void AddQuest(QuestViewModel quest)
        {
            this.questsMutable.Add(quest);
            quest.QuestSet = this;
        }

        public void AddQuestLink(QuestViewModel fromQuest, QuestViewModel toQuest)
        {
            // TODO: index this collection so we don't need to linear search
            if (this.questLinksMutable.Any(questLink => questLink.Conflicts(fromQuest, toQuest)))
            {
                MessageBox.Show("TODO: nicer error message here, but for now... there's a conflict yo!");
                return;
            }

            this.questLinksMutable.Add(new QuestLinkViewModel(fromQuest: fromQuest, toQuest: toQuest));
        }

        public void RemoveQuest(QuestViewModel quest)
        {
            quest.QuestSet = null;
            this.questsMutable.Remove(quest);
        }

        public void RemoveQuestLink(QuestLinkViewModel questLink)
        {
            this.questLinksMutable.Remove(questLink);
        }

        public RelayCommand AddQuestCommand { get; }
        private void AddQuest()
        {
            QuestViewModel newQuest = new QuestViewModel { QuestSet = this };
            EditQuestMessage message = new EditQuestMessage { Quest = newQuest };
            this.MessengerInstance.Send(message);
            if (message.Accepted)
            {
                this.AddQuest(newQuest);
            }
        }
    }
}
