using GalaSoft.MvvmLight.Messaging;

namespace QuestEditor.Messages
{
    public sealed class SelectFileMessage : MessageBase
    {
        public string SelectedFilePath { get; set; }
    }
}
