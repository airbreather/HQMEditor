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
            set { this.Set(ref this.triggerType, value.ValidateEnum("value", TriggerType.Normal, TriggerType.ReverseTrigger, TriggerType.TaskCount, TriggerType.Trigger)); this.RaisePropertyChanged(() => this.TaskCountIsRelevant); }
        }

        private int taskCount;
        public int TaskCount
        {
            get { return this.taskCount; }
            set { this.Set(ref this.taskCount, value.ValidateMinAndMax("value", 0, Int32.MaxValue)); }
        }

        private bool TaskCountIsRelevant
        {
            get { return this.TriggerType == TriggerType.TaskCount; }
        }
    }
}
