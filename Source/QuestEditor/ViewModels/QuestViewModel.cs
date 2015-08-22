using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using QuestEditor.Messages;

namespace QuestEditor.ViewModels
{
    public sealed class QuestViewModel : ViewModelBase
    {
        public QuestViewModel()
        {
            this.id = -1;
            this.EditCommand = new RelayCommand(this.Edit);
            this.ReputationRewards = new ReadOnlyObservableCollection<ReputationRewardViewModel>(this.reputationRewardsMutable);
        }

        public void CopyFrom(QuestViewModel other)
        {
            this.Id = other.id;
            this.XPos = other.xPos;
            this.YPos = other.yPos;
            this.Name = other.name;
            this.Description = other.description;
            this.Icon = other.icon;
            this.IsBig = other.isBig;
            this.QuestSet = other.questSet;
            this.RepeatOption.RepeatType = other.RepeatOption.RepeatType;
            this.RepeatOption.RepeatIntervalHours = other.RepeatOption.RepeatIntervalHours;
            this.TriggerOption.TriggerType = other.TriggerOption.TriggerType;
            this.TriggerOption.TaskCount = other.TriggerOption.TaskCount;
            this.ModifiedParentRequirement.UseModifiedParentRequirement = other.ModifiedParentRequirement.UseModifiedParentRequirement;
            this.ModifiedParentRequirement.ModifiedParentRequirementCount = other.ModifiedParentRequirement.ModifiedParentRequirementCount;
            this.reputationRewardsMutable.Clear();
            this.reputationRewardsMutable.AddRange(other.reputationRewardsMutable);
        }

        private int id;
        public int Id
        {
            get { return this.id; }
            set { this.Set(ref this.id, value); }
        }

        private int xPos;
        public int XPos
        {
            get { return this.xPos; }
            set { this.Set(ref this.xPos, value); this.RaisePropertyChanged(nameof(this.Pos)); }
        }

        private int yPos;
        public int YPos
        {
            get { return this.yPos; }
            set { this.Set(ref this.yPos, value); this.RaisePropertyChanged(nameof(this.Pos)); }
        }

        public Point Pos => new Point(this.xPos, this.yPos);

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

        private ItemStackViewModel icon;
        public ItemStackViewModel Icon
        {
            get { return this.icon; }
            set { this.Set(ref this.icon, value); }
        }

        private bool isBig;
        public bool IsBig
        {
            get { return this.isBig; }
            set { this.Set(ref this.isBig, value); }
        }

        private readonly ObservableCollection<ReputationRewardViewModel> reputationRewardsMutable = new ObservableCollection<ReputationRewardViewModel>();
        public ReadOnlyObservableCollection<ReputationRewardViewModel> ReputationRewards { get; }

        private QuestSetViewModel questSet;
        public QuestSetViewModel QuestSet
        {
            get { return this.questSet; }
            set { this.Set(ref this.questSet, value); }
        }
        
        public ModifiedParentRequirementViewModel ModifiedParentRequirement { get; } = new ModifiedParentRequirementViewModel();
        public QuestRepeatOptionViewModel RepeatOption { get; } = new QuestRepeatOptionViewModel();
        public QuestTriggerOptionViewModel TriggerOption { get; } = new QuestTriggerOptionViewModel();

        public RelayCommand EditCommand { get; }
        private void Edit()
        {
            QuestViewModel copiedQuest = new QuestViewModel();
            copiedQuest.CopyFrom(this);

            EditQuestMessage message = new EditQuestMessage { Quest = copiedQuest };
            this.MessengerInstance.Send(message);
            if (message.Accepted)
            {
                this.CopyFrom(copiedQuest);
            }
        }
        
        internal void ReplaceReputationRewards(IEnumerable<ReputationRewardViewModel> newReputationRewards)
        {
            this.reputationRewardsMutable.Clear();
            this.reputationRewardsMutable.AddRange(newReputationRewards);
        }
    }
}
