using System.Collections.ObjectModel;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace QuestEditor.ViewModels
{
    public sealed class EditReputationRewardsViewModel : ViewModelBase
    {
        public EditReputationRewardsViewModel(ObservableCollection<ReputationRewardViewModel> reputationRewards)
        {
            this.ReputationRewards = reputationRewards.ValidateNotNull(nameof(reputationRewards));

            this.DeleteReputationRewardCommand = new RelayCommand<ReputationRewardViewModel>(this.DeleteReputationReward);
            this.AddReputationRewardCommand = new RelayCommand(this.AddReputationReward);
        }

        public ObservableCollection<ReputationRewardViewModel> ReputationRewards { get; }

        private ReputationRewardViewModel selectedReputationReward;
        public ReputationRewardViewModel SelectedReputationReward
        {
            get { return this.selectedReputationReward; }
            set { this.Set(ref this.selectedReputationReward, value); }
        }

        public RelayCommand<ReputationRewardViewModel> DeleteReputationRewardCommand { get; }
        private void DeleteReputationReward(ReputationRewardViewModel reputationReward)
        {
            this.ReputationRewards.Remove(reputationReward);
        }

        public RelayCommand AddReputationRewardCommand { get; }
        private void AddReputationReward()
        {
            this.ReputationRewards.Add(new ReputationRewardViewModel());
        }
    }
}
