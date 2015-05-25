using GalaSoft.MvvmLight;

using HQMFileConverter;

namespace QuestEditor.ViewModels
{
    public sealed class QuestRepeatOptionViewModel : ViewModelBase
    {
        private RepeatType repeatType;
        public RepeatType RepeatType
        {
            get { return this.repeatType; }
            set { this.Set(ref this.repeatType, value.ValidateEnum("value", RepeatType.Cooldown, RepeatType.Instant, RepeatType.Interval, RepeatType.None)); this.RaisePropertyChanged(() => this.RepeatIntervalIsRelevant); }
        }

        private int repeatIntervalHours;
        public int RepeatIntervalHours
        {
            get { return this.repeatIntervalHours; }
            set { this.Set(ref this.repeatIntervalHours, value); }
        }

        public bool RepeatIntervalIsRelevant
        {
            get { return this.repeatType == RepeatType.Cooldown || this.repeatType == RepeatType.Interval; }
        }
    }
}
