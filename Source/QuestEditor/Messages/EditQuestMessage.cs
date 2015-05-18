using GalaSoft.MvvmLight.Messaging;

using QuestEditor.ViewModels;

namespace QuestEditor.Messages
{
    public sealed class EditQuestMessage : MessageBase
    {
        public bool Accepted { get; set; }
        public QuestViewModel Quest { get; set; }
    }
}
