using System;
using System.Windows;

using QuestEditor.ViewModels;

namespace QuestEditor.Views
{
    public partial class EditQuestWindow : Window
    {
        public EditQuestWindow()
            : this(new QuestViewModel())
        {
        }

        public EditQuestWindow(QuestViewModel quest)
        {
            this.ViewModel = new EditQuestViewModel(quest.ValidateNotNull(nameof(quest)));
            this.InitializeComponent();
        }

        public EditQuestViewModel ViewModel { get; }

        private void OnOKButtonClick(object sender, EventArgs e) =>
            this.DialogResult = true;
    }
}
