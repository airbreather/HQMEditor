using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class ReputationRewardViewModel : ViewModelBase
    {
        public ReputationRewardViewModel()
        {
        }

        public ReputationRewardViewModel(ReputationRewardViewModel copyFrom)
        {
            this.reputationId = copyFrom.reputationId;
            this.value = copyFrom.value;
        }

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
