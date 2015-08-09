using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class EditItemStackViewModel : ViewModelBase
    {
        public EditItemStackViewModel(ItemStackViewModel itemStack)
        {
            this.ItemStack = itemStack.ValidateNotNull(nameof(itemStack));
        }

        public ItemStackViewModel ItemStack { get; }
    }
}
