using System.Collections.ObjectModel;

using GalaSoft.MvvmLight.Messaging;

using QuestEditor.ViewModels;

namespace QuestEditor.Messages
{
    public sealed class EditReputationRewardsMessage : MessageBase
    {
        public bool Accepted { get; set; }
        public ObservableCollection<ReputationRewardViewModel> ReputationRewards { get; set; }
    }
}
