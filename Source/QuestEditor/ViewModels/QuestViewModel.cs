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
            this.editCommand = new RelayCommand(this.Edit);
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
            this.repeatOption.RepeatType = other.repeatOption.RepeatType;
            this.repeatOption.RepeatIntervalHours = other.repeatOption.RepeatIntervalHours;
            this.triggerOption.TriggerType = other.triggerOption.TriggerType;
            this.triggerOption.TaskCount = other.triggerOption.TaskCount;
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
            set { this.Set(ref this.xPos, value); this.RaisePropertyChanged("Pos"); }
        }

        private int yPos;
        public int YPos
        {
            get { return this.yPos; }
            set { this.Set(ref this.yPos, value); this.RaisePropertyChanged("Pos"); }
        }

        public Point Pos { get { return new Point(this.xPos, this.yPos); } }

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

        private readonly QuestRepeatOptionViewModel repeatOption = new QuestRepeatOptionViewModel();
        public QuestRepeatOptionViewModel RepeatOption
        {
            get { return this.repeatOption; }
        }

        private readonly QuestTriggerOptionViewModel triggerOption = new QuestTriggerOptionViewModel();
        public QuestTriggerOptionViewModel TriggerOption
        {
            get { return this.triggerOption; }
        }

        private QuestSetViewModel questSet;
        public QuestSetViewModel QuestSet
        {
            get { return this.questSet; }
            set { this.Set(ref this.questSet, value); }
        }

        private readonly RelayCommand editCommand;
        public RelayCommand EditCommand { get { return this.editCommand; } }

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
    }
}
