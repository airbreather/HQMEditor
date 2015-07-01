using System;
using System.Windows;

using QuestEditor.ViewModels;

namespace QuestEditor.Views
{
    public partial class EditQuestWindow : Window
    {
        private readonly EditQuestViewModel viewModel;

        public EditQuestWindow()
            : this(new QuestViewModel())
        {
        }

        public EditQuestWindow(QuestViewModel quest)
        {
            this.viewModel = new EditQuestViewModel(quest.ValidateNotNull("quest"));
            this.InitializeComponent();
        }

        public EditQuestViewModel ViewModel { get { return this.viewModel; } }

        private void OnOKButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
