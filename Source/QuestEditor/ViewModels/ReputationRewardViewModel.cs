using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class ReputationRewardViewModel : ViewModelBase
    {
        private int reputationId;
        public int ReputationId
        {
            get { return this.reputationId; }
            set { this.Set(ref this.reputationId, value); }
        }

        private int value;
        public int Value
        {
            get { return this.value; }
            set { this.Set(ref this.value, value); }
        }
    }
}
