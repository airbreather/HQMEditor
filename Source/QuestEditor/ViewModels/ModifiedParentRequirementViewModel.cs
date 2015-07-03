using System;

using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class ModifiedParentRequirementViewModel : ViewModelBase
    {
        private bool useModifiedParentRequirement;
        public bool UseModifiedParentRequirement
        {
            get { return this.useModifiedParentRequirement; }
            set { this.Set(ref this.useModifiedParentRequirement, value); }
        }

        private int modifiedParentRequirementCount;
        public int ModifiedParentRequirementCount
        {
            get { return this.modifiedParentRequirementCount; }
            set { this.Set(ref this.modifiedParentRequirementCount, value.ValidateMinAndMax(nameof(value), 0, Int32.MaxValue)); }
        }
    }
}
