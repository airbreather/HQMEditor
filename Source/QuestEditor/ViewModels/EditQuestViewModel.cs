using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using QuestEditor.Messages;

namespace QuestEditor.ViewModels
{
    public sealed class EditQuestViewModel : ViewModelBase
    {
        public EditQuestViewModel(QuestViewModel quest)
        {
            this.Quest = quest.ValidateNotNull(nameof(quest));
            this.EditIconCommand = new RelayCommand(this.EditIcon);
        }

        public QuestViewModel Quest { get; }

        public RelayCommand EditIconCommand { get; }

        private void EditIcon()
        {
            this.Quest.Icon = this.Quest.Icon ?? new ItemStackViewModel();

            ItemStackViewModel itemStack = new ItemStackViewModel();
            itemStack.CopyFrom(this.Quest.Icon);

            EditItemStackMessage message = new EditItemStackMessage { ItemStack = itemStack };
            this.MessengerInstance.Send(message);
            if (!message.Accepted)
            {
                return;
            }

            this.Quest.Icon.CopyFrom(message.ItemStack);
        }
    }
}
