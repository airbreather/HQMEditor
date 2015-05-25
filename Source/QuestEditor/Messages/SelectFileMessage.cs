using GalaSoft.MvvmLight.Messaging;

namespace QuestEditor.Messages
{
    public abstract class SelectFileMessageBase : MessageBase
    {
        public string SelectedFilePath { get; set; }
        public string FileExtension { get; set; }
        public string FileExtensionFilter { get; set; }
    }

    public sealed class SelectSourceFileMessage : SelectFileMessageBase
    {
    }

    public sealed class SelectTargetFileMessage : SelectFileMessageBase
    {
        public bool PromptForOverwrite { get; set; }
    }
}
