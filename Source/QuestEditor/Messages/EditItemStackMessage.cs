using GalaSoft.MvvmLight.Messaging;

using QuestEditor.ViewModels;

namespace QuestEditor.Messages
{
    public sealed class EditItemStackMessage : MessageBase
    {
        public bool Accepted { get; set; }
        public ItemStackViewModel ItemStack { get; set; }
    }
}
