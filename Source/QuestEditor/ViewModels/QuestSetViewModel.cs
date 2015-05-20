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
            : this(String.Empty, Enumerable.Empty<QuestViewModel>(), Enumerable.Empty<QuestLinkViewModel>())
        {
        }

        public QuestSetViewModel(string name, IEnumerable<QuestViewModel> quests, IEnumerable<QuestLinkViewModel> questLinks)
        {
            this.addQuestCommand = new RelayCommand(this.AddQuest);

            this.name = name;
            this.quests = new ObservableCollection<QuestViewModel>(quests);
            this.questsReadOnly = new ReadOnlyObservableCollection<QuestViewModel>(this.quests);

            this.questLinks = new ObservableCollection<QuestLinkViewModel>(questLinks);
            this.questLinksReadOnly = new ReadOnlyObservableCollection<QuestLinkViewModel>(this.questLinks);
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

        private readonly ObservableCollection<QuestViewModel> quests;
        private readonly ReadOnlyObservableCollection<QuestViewModel> questsReadOnly;
        public ReadOnlyObservableCollection<QuestViewModel> Quests { get { return this.questsReadOnly; } }

        private readonly ObservableCollection<QuestLinkViewModel> questLinks;
        private readonly ReadOnlyObservableCollection<QuestLinkViewModel> questLinksReadOnly;
        public ReadOnlyObservableCollection<QuestLinkViewModel> QuestLinks { get { return this.questLinksReadOnly; } }

        private readonly RelayCommand addQuestCommand;
        public RelayCommand AddQuestCommand { get { return this.addQuestCommand; } }

        internal void AddQuest(QuestViewModel quest)
        {
            this.quests.Add(quest);
        }

        internal void AddQuestLink(QuestViewModel fromQuest, QuestViewModel toQuest)
        {
            // TODO: index this collection so we don't need to linear search
            if (this.questLinks.Any(questLink => questLink.Conflicts(fromQuest, toQuest)))
            {
                MessageBox.Show("TODO: nicer error message here, but for now... there's a conflict yo!");
                return;
            }

            this.questLinks.Add(new QuestLinkViewModel { FromQuest = fromQuest, ToQuest = toQuest });
        }

        internal void RemoveQuest(QuestViewModel quest)
        {
            this.quests.Remove(quest);
        }

        internal void RemoveQuestLink(QuestLinkViewModel questLink)
        {
            this.questLinks.Remove(questLink);
        }

        private void AddQuest()
        {
            QuestViewModel newQuest = new QuestViewModel();
            EditQuestMessage message = new EditQuestMessage { Quest = newQuest };
            this.MessengerInstance.Send(message);
            if (message.Accepted)
            {
                this.quests.Add(newQuest);
            }
        }
    }
}
