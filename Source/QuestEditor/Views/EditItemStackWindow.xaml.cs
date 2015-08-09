using System;

using QuestEditor.ViewModels;

namespace QuestEditor.Views
{
    public partial class EditItemStackWindow
    {
        public EditItemStackWindow()
            : this(new ItemStackViewModel())
        {
        }

        public EditItemStackWindow(ItemStackViewModel itemStack)
        {
            this.ViewModel = new EditItemStackViewModel(itemStack.ValidateNotNull(nameof(itemStack)));
            this.InitializeComponent();
        }

        public EditItemStackViewModel ViewModel { get; }

        private void OnOKButtonClick(object sender, EventArgs e) => this.DialogResult = true;
    }
}
