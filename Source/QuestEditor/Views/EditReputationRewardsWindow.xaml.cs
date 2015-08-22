using System;
using System.Collections.ObjectModel;
using QuestEditor.ViewModels;

namespace QuestEditor.Views
{
    public partial class EditReputationRewardsWindow
    {
        public EditReputationRewardsWindow()
            : this(new EditReputationRewardsViewModel(new ObservableCollection<ReputationRewardViewModel>()))
        {
        }

        public EditReputationRewardsWindow(EditReputationRewardsViewModel viewModel)
        {
            this.ViewModel = viewModel.ValidateNotNull(nameof(viewModel));
            this.InitializeComponent();
        }

        public EditReputationRewardsViewModel ViewModel { get; }

        private void OnOKButtonClick(object sender, EventArgs e) => this.DialogResult = true;
    }
}
