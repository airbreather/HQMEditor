using System;

using GalaSoft.MvvmLight;

using HQMFileConverter;

namespace QuestEditor.ViewModels
{
    public sealed class QuestTriggerOptionViewModel : ViewModelBase
    {
        private TriggerType triggerType;
        public TriggerType TriggerType
        {
            get { return this.triggerType; }
            set { this.Set(ref this.triggerType, value.ValidateEnum(nameof(value), TriggerType.Normal, TriggerType.ReverseTrigger, TriggerType.TaskCount, TriggerType.Trigger)); this.RaisePropertyChanged(nameof(this.TaskCountIsRelevant)); }
        }

        private int taskCount;
        public int TaskCount
        {
            get { return this.taskCount; }
            set { this.Set(ref this.taskCount, value.ValidateMinAndMax(nameof(value), 0, Int32.MaxValue)); }
        }

        public bool TaskCountIsRelevant => this.TriggerType == TriggerType.TaskCount;
    }
}
