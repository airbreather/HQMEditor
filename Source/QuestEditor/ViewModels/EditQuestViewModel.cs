using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class EditQuestViewModel : ViewModelBase
    {
        public EditQuestViewModel(QuestViewModel quest)
        {
            this.Quest = quest.ValidateNotNull(nameof(quest));
        }

        public QuestViewModel Quest { get; }
    }
}
