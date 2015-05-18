using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using QuestEditor.Messages;
using System.IO;
using HQMFileConverter;
using System.Collections.Generic;

namespace QuestEditor.ViewModels
{
    public enum MouseMode
    {
        Drag,
        CreateLink,
        DeleteLink
    }

    public sealed class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            this.addQuestSetCommand = new RelayCommand(this.AddQuestSet);
            this.loadQuestLineCommand = new RelayCommand(this.LoadQuestLine);

            QuestViewModel[] quests =
            {
                new QuestViewModel { Id = 0, XPos = 50, YPos = 50, Name="Quest of Champions", Description="In west Philadelphia" },
                new QuestViewModel { Id = 1, XPos = 120, YPos = 130, Name="Other Quest", Description="Born and raised" },
                new QuestViewModel { Id = 2, XPos = 10, YPos = 10, Name="My Favorite Quest", Description="In the playground" },
                new QuestViewModel { Id = 3, XPos = 22, YPos = 22, Name="Unnamed Quest", Description="Is where I spent" },
                new QuestViewModel { Id = 4, XPos = 81, YPos = 92, Name="Boring Quest", Description="Most of my days" }
            };

            QuestLinkViewModel[] questLinks =
            {
                new QuestLinkViewModel { FromQuest = quests[0], ToQuest = quests[1] },
                new QuestLinkViewModel { FromQuest = quests[2], ToQuest = quests[3] }
            };

            this.questSets = new ObservableCollection<QuestSetViewModel>
            {
                new QuestSetViewModel("Q1", new ArraySegment<QuestViewModel>(quests, 0, 2), new ArraySegment<QuestLinkViewModel>(questLinks, 0, 1)),
                new QuestSetViewModel("Q2", new ArraySegment<QuestViewModel>(quests, 2, 2), new ArraySegment<QuestLinkViewModel>(questLinks, 1, 1)),
                new QuestSetViewModel("Q3", new ArraySegment<QuestViewModel>(quests, 4, 1), Enumerable.Empty<QuestLinkViewModel>())
            };

            this.questSetsReadOnly = new ReadOnlyObservableCollection<QuestSetViewModel>(this.questSets);
            this.selectedQuestSet = this.questSets[0];
        }

        private QuestSetViewModel selectedQuestSet;
        public QuestSetViewModel SelectedQuestSet
        {
            get { return this.selectedQuestSet; }
            set { this.Set(ref this.selectedQuestSet, value); }
        }

        private readonly ObservableCollection<QuestSetViewModel> questSets;
        private readonly ReadOnlyObservableCollection<QuestSetViewModel> questSetsReadOnly;
        public ReadOnlyObservableCollection<QuestSetViewModel> QuestSets { get { return this.questSetsReadOnly; } }

        private readonly RelayCommand loadQuestLineCommand;
        public RelayCommand LoadQuestLineCommand { get { return this.loadQuestLineCommand; } }

        private readonly RelayCommand addQuestSetCommand;
        public RelayCommand AddQuestSetCommand { get { return this.addQuestSetCommand; } }

        private MouseMode mouseMode;
        public MouseMode MouseMode
        {
            get { return this.mouseMode; }
            set { this.Set(ref this.mouseMode, value); }
        }

        private void AddQuestSet()
        {
            // TODO: what's the max length again?
            SelectStringMessage message = new SelectStringMessage { MaxLength = 45, Title = "Quest Set Name" };
            this.MessengerInstance.Send(message);
            if (!message.Accepted)
            {
                return;
            }

            this.questSets.Add(new QuestSetViewModel(message.StringValue, Enumerable.Empty<QuestViewModel>(), Enumerable.Empty<QuestLinkViewModel>()));
        }

        private void LoadQuestLine()
        {
            SelectFileMessage message = new SelectFileMessage();
            this.MessengerInstance.Send(message);
            if (String.IsNullOrEmpty(message.SelectedFilePath))
            {
                return;
            }

            this.questSets.Clear();

            QuestLine ql;
            using (var stream = File.OpenRead(message.SelectedFilePath))
            {
                ql = new HQMQuestLineReader().ReadQuestLine(stream);
            }

            Dictionary<int, QuestSetViewModel> questSetMapping = ql.QuestSets.ToDictionary(qs => qs.Id, qs => new QuestSetViewModel { Name = qs.Name });
            Dictionary<int, QuestViewModel> questMapping = new Dictionary<int, QuestViewModel>();

            foreach (var quest in ql.Quests.Where(q => q != null))
            {
                QuestSetViewModel set = questSetMapping[quest.QuestSetId];
                QuestViewModel q = new QuestViewModel
                {
                    Id = quest.Id,
                    Description = quest.Description,
                    Name = quest.Name,
                    XPos = quest.XPos,
                    YPos = quest.YPos
                };

                set.AddQuest(q);
                questMapping.Add(q.Id, q);
            }

            foreach (var q in questMapping.Values)
            {
                var qOrig = ql.Quests[q.Id];
                if (qOrig.RequiredQuestIds == null)
                {
                    continue;
                }

                foreach (var id in qOrig.RequiredQuestIds)
                {
                    var reQ = ql.Quests[id];
                    if (qOrig.QuestSetId == reQ.QuestSetId)
                    {
                        QuestViewModel fromQuest = questMapping[reQ.Id];
                        QuestSetViewModel set = questSetMapping[reQ.QuestSetId];
                        set.AddQuestLink(fromQuest, q);
                    }
                }
            }

            foreach (var set in questSetMapping.OrderBy(q => q.Key).Select(q => q.Value))
            {
                this.questSets.Add(set);
            }

            this.SelectedQuestSet = this.questSets[0];
        }
    }
}
