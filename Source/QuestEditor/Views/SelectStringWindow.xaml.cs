using System;
using System.Windows;

using QuestEditor.ViewModels;

namespace QuestEditor.Views
{
    public partial class SelectStringWindow : Window
    {
        public SelectStringWindow()
            : this(new SelectStringViewModel("Example Title", 100, "Example Initial Value"))
        {
        }

        public SelectStringWindow(SelectStringViewModel viewModel)
        {
            this.ViewModel = viewModel.ValidateNotNull(nameof(viewModel));
            this.InitializeComponent();
        }

        public SelectStringViewModel ViewModel { get; }

        private void OnOKButtonClick(object sender, EventArgs e) =>
            this.DialogResult = true;
    }
}
