using GalaSoft.MvvmLight.Messaging;

namespace QuestEditor.Messages
{
    public sealed class SelectStringMessage : MessageBase
    {
        public bool Accepted { get; set; }
        public string StringValue { get; set; }
        public int MaxLength { get; set; }
        public string Title { get; set; }
    }
}
