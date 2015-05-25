using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using HQMFileConverter;

using QuestEditor.Messages;

namespace QuestEditor.ViewModels
{
    public enum MouseMode
    {
        EditQuests,
        CreateLink,
        DeleteLink
    }

    public sealed class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            this.addQuestSetCommand = new RelayCommand(this.AddQuestSet);
            this.loadQuestLineCommand = new RelayCommand(this.LoadQuestLine);
            this.saveQuestLineCommand = new RelayCommand(this.SaveQuestLine);

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
                new QuestSetViewModel(1, "Q1", new ArraySegment<QuestViewModel>(quests, 0, 2), new ArraySegment<QuestLinkViewModel>(questLinks, 0, 1)),
                new QuestSetViewModel(2, "Q2", new ArraySegment<QuestViewModel>(quests, 2, 2), new ArraySegment<QuestLinkViewModel>(questLinks, 1, 1)),
                new QuestSetViewModel(3, "Q3", new ArraySegment<QuestViewModel>(quests, 4, 1), Enumerable.Empty<QuestLinkViewModel>())
            };

            this.questSetsReadOnly = new ReadOnlyObservableCollection<QuestSetViewModel>(this.questSets);
            this.selectedQuestSet = this.questSets[0];

            this.crossSetQuestLinks = new ObservableCollection<QuestLinkViewModel>();
            this.crossSetQuestLinksReadOnly = new ReadOnlyObservableCollection<QuestLinkViewModel>(this.crossSetQuestLinks);

            this.reputations = new ObservableCollection<ReputationViewModel>();
            this.reputationsReadOnly = new ReadOnlyObservableCollection<ReputationViewModel>(this.reputations);
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

        private readonly ObservableCollection<QuestLinkViewModel> crossSetQuestLinks;
        private readonly ReadOnlyObservableCollection<QuestLinkViewModel> crossSetQuestLinksReadOnly;
        public ReadOnlyObservableCollection<QuestLinkViewModel> CrossSetQuestLinks { get { return this.crossSetQuestLinksReadOnly; } }

        private readonly ObservableCollection<ReputationViewModel> reputations;
        private readonly ReadOnlyObservableCollection<ReputationViewModel> reputationsReadOnly;
        public ReadOnlyObservableCollection<ReputationViewModel> Reputations { get { return this.reputationsReadOnly; } }

        private readonly RelayCommand loadQuestLineCommand;
        public RelayCommand LoadQuestLineCommand { get { return this.loadQuestLineCommand; } }

        private readonly RelayCommand saveQuestLineCommand;
        public RelayCommand SaveQuestLineCommand { get { return this.saveQuestLineCommand; } }

        private readonly RelayCommand addQuestSetCommand;
        public RelayCommand AddQuestSetCommand { get { return this.addQuestSetCommand; } }

        private MouseMode mouseMode;
        public MouseMode MouseMode
        {
            get { return this.mouseMode; }
            set { this.Set(ref this.mouseMode, value); }
        }

        private string passCode;
        public string PassCode
        {
            get { return this.passCode; }
            set { this.Set(ref this.passCode, value); }
        }

        private string description;
        public string Description
        {
            get { return this.description; }
            set { this.Set(ref this.description, value); }
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

            this.questSets.Add(new QuestSetViewModel(-1, message.StringValue, Enumerable.Empty<QuestViewModel>(), Enumerable.Empty<QuestLinkViewModel>()));
        }

        private void LoadQuestLine()
        {
            SelectSourceFileMessage message = new SelectSourceFileMessage
            {
                FileExtension = ".hqm",
                FileExtensionFilter = "HQM Files (*.hqm)|*.hqm|All Files (*.*)|*.*"
            };
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

            this.PassCode = ql.PassCode;
            this.Description = ql.Description;

            Dictionary<int, QuestSetViewModel> questSetMapping = ql.QuestSets.ToDictionary(qs => qs.Id, qs => new QuestSetViewModel { Id = qs.Id, Name = qs.Name });
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
                    YPos = quest.YPos,
                    Icon = Conversions.ItemStackToItemStackViewModel(quest.Icon),
                    IsBig = quest.IsBig,
                    RepeatOption =
                    {
                        RepeatIntervalHours = quest.RepeatIntervalHours,
                        RepeatType = quest.RepeatType
                    },
                    TriggerOption =
                    {
                        TriggerType = quest.TriggerType,
                        TaskCount = quest.TriggerTaskCount
                    }
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
                    QuestViewModel fromQuest = questMapping[reQ.Id];
                    QuestSetViewModel set = questSetMapping[reQ.QuestSetId];

                    if (qOrig.QuestSetId == reQ.QuestSetId)
                    {
                        set.AddQuestLink(fromQuest, q);
                    }
                    else
                    {
                        this.crossSetQuestLinks.Add(new QuestLinkViewModel { FromQuest = fromQuest, ToQuest = q });
                    }
                }
            }

            foreach (var set in questSetMapping.OrderBy(q => q.Key).Select(q => q.Value))
            {
                this.questSets.Add(set);
            }

            foreach (var reputation in ql.Reputations)
            {
                this.reputations.Add(Conversions.ReputationToReputationViewModel(reputation));
            }

            this.SelectedQuestSet = this.questSets[0];
        }

        private void SaveQuestLine()
        {
            MessageBox.Show("Do not use this to overwrite your original *.hqm file!  This is still incomplete!  You have been warned.");

            SelectTargetFileMessage message = new SelectTargetFileMessage
            {
                FileExtension = ".hqm",
                FileExtensionFilter = "HQM Files (*.hqm)|*.hqm|All Files (*.*)|*.*",
                PromptForOverwrite = true
            };
            this.MessengerInstance.Send(message);
            if (String.IsNullOrEmpty(message.SelectedFilePath))
            {
                return;
            }

            QuestLine questLine = new QuestLine();

            questLine.Version = 20;
            questLine.PassCode = this.passCode;
            questLine.Description = this.description;

            int maxQuestSetId = this.questSets.Max(x => x.Id);

            questLine.QuestSets = new QuestSet[this.questSets.Count];
            for (int questSetIndex = 0; questSetIndex < questLine.QuestSets.Length; questSetIndex++)
            {
                var questSet = this.questSets[questSetIndex];

                // TODO: preserve old IDs, probably with a Dictionary<int, int>?
                questSet.Id = questSetIndex;

                QuestSet outputQuestSet = questLine.QuestSets[questSetIndex] = new QuestSet();

                outputQuestSet.Id = questSetIndex;
                outputQuestSet.Name = questSet.Name;
                outputQuestSet.Description = String.Empty;
            }

            var quests = this.questSets.SelectMany(questSet => questSet.Quests).ToArray();

            // TODO: preserve old IDs, probably with a Dictionary<int, int>?
            for (int questIndex = 0; questIndex < quests.Length; questIndex++)
            {
                quests[questIndex].Id = questIndex;
            }

            questLine.Quests = new Quest[quests.Length];
            for (int questIndex = 0; questIndex < questLine.Quests.Length; questIndex++)
            {
                var quest = quests[questIndex];
                var outputQuest = questLine.Quests[questIndex] = new Quest();

                outputQuest.Id = questIndex;
                outputQuest.Name = quest.Name;
                outputQuest.Description = quest.Description;
                outputQuest.Icon = Conversions.ItemStackViewModelToItemStack(quest.Icon);
                outputQuest.XPos = quest.XPos;
                outputQuest.YPos = quest.YPos;
                outputQuest.IsBig = quest.IsBig;
                outputQuest.QuestSetId = quest.QuestSet.Id;
                outputQuest.RepeatType = quest.RepeatOption.RepeatType;
                outputQuest.RepeatIntervalHours = quest.RepeatOption.RepeatIntervalHours;
                outputQuest.TriggerType = quest.TriggerOption.TriggerType;
                outputQuest.TriggerTaskCount = quest.TriggerOption.TaskCount;

                // TODO: tasks.
                outputQuest.Tasks = new QuestTask[0];

                // TODO: reputation rewards.
                outputQuest.ReputationRewards = new ReputationReward[0];
            }

            foreach (var questLink in this.questSets.SelectMany(questSet => questSet.QuestLinks).Concat(this.crossSetQuestLinks))
            {
                var q = questLine.Quests[questLink.ToQuest.Id];
                q.RequiredQuestIds = (q.RequiredQuestIds ?? Enumerable.Empty<int>()).Concat(new[] { questLink.FromQuest.Id }).ToArray();
            }

            questLine.Reputations = new Reputation[this.reputations.Count];

            for (int reputationIndex = 0; reputationIndex < questLine.Reputations.Length; reputationIndex++)
            {
                questLine.Reputations[reputationIndex] = Conversions.ReputationViewModelToReputation(this.reputations[reputationIndex]);
            }

            // TODO: bags.
            questLine.Tiers = new RewardBagTier[0];
            questLine.Bags = new RewardBag[0];

            using (var stream = File.OpenWrite(message.SelectedFilePath))
            {
                new HQMQuestLineWriter().WriteQuestLine(questLine, stream);
            }
        }
    }
}
