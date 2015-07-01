using System;
using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class QuestLinkViewModel : ViewModelBase, IEquatable<QuestLinkViewModel>
    {
        public QuestLinkViewModel(QuestViewModel fromQuest, QuestViewModel toQuest)
        {
            this.fromQuest = fromQuest.ValidateNotNull("fromQuest");
            this.toQuest = toQuest.ValidateNotNull("toQuest");
        }

        private readonly QuestViewModel fromQuest;
        public QuestViewModel FromQuest { get { return this.fromQuest; } }

        private readonly QuestViewModel toQuest;
        public QuestViewModel ToQuest { get { return this.toQuest; } }

        public bool Conflicts(QuestViewModel quest1, QuestViewModel quest2)
        {
            return (this.fromQuest == quest1 && this.toQuest == quest2) ||
                   (this.fromQuest == quest2 && this.toQuest == quest1);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as QuestLinkViewModel);
        }

        public bool Equals(QuestLinkViewModel other)
        {
            return other != null &&
                   Equals(this.fromQuest, other.fromQuest) &&
                   Equals(this.toQuest, other.toQuest);
        }

        public override int GetHashCode()
        {
            return this.fromQuest.GetHashCode() ^ this.toQuest.GetHashCode();
        }
    }
}
