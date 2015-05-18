using System;
using System.Windows;

using QuestEditor.ViewModels;

namespace QuestEditor.Views
{
    public partial class SelectStringWindow : Window
    {
        private readonly SelectStringViewModel viewModel;

        public SelectStringWindow()
            : this(new SelectStringViewModel("Example Title", 100, "Example Initial Value"))
        {
        }

        public SelectStringWindow(SelectStringViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.InitializeComponent();
        }

        public SelectStringViewModel ViewModel { get { return this.viewModel; } }

        private void OnOKButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
