using System;

using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class QuestLinkViewModel : ViewModelBase, IEquatable<QuestLinkViewModel>
    {
        public QuestLinkViewModel(QuestViewModel fromQuest, QuestViewModel toQuest)
        {
            this.FromQuest = fromQuest.ValidateNotNull(nameof(fromQuest));
            this.ToQuest = toQuest.ValidateNotNull(nameof(toQuest));
        }

        public QuestViewModel FromQuest { get; }
        public QuestViewModel ToQuest { get; }

        public bool Conflicts(QuestViewModel quest1, QuestViewModel quest2) => (this.FromQuest == quest1 && this.ToQuest == quest2) || (this.FromQuest == quest2 && this.ToQuest == quest1);

        public override bool Equals(object obj) => this.Equals(obj as QuestLinkViewModel);
        public bool Equals(QuestLinkViewModel other) => other != null && Equals(this.FromQuest, other.FromQuest) && Equals(this.ToQuest, other.ToQuest);
        public override int GetHashCode() => this.FromQuest.GetHashCode() ^ this.ToQuest.GetHashCode();
    }
}
