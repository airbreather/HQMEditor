using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class QuestLinkViewModel : ViewModelBase
    {
        private QuestViewModel fromQuest;
        public QuestViewModel FromQuest
        {
            get { return this.fromQuest; }
            set { this.Set(ref this.fromQuest, value); }
        }

        private QuestViewModel toQuest;
        public QuestViewModel ToQuest
        {
            get { return this.toQuest; }
            set { this.Set(ref this.toQuest, value); }
        }

        public bool Conflicts(QuestViewModel quest1, QuestViewModel quest2)
        {
            return (this.fromQuest == quest1 && this.toQuest == quest2) ||
                   (this.fromQuest == quest2 && this.toQuest == quest1);
        }
    }
}
