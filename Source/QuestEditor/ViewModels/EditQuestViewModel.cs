using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class EditQuestViewModel : ViewModelBase
    {
        public EditQuestViewModel(QuestViewModel quest)
        {
            this.quest = quest;

#if FALSE
            this.registerValidationErrorCommand = new RelayCommand<ValidationErrorEventArgs>(this.RegisterValidationError);
#endif
        }

        private readonly QuestViewModel quest;
        public QuestViewModel Quest { get { return this.quest; } }

#if FALSE
        private readonly RelayCommand<ValidationErrorEventArgs> registerValidationErrorCommand;
        public RelayCommand<ValidationErrorEventArgs> RegisterValidationErrorCommand { get { return this.registerValidationErrorCommand; } }

        private int validationErrorCount;
        public bool IsValid { get { return this.validationErrorCount == 0; } }

        private void RegisterValidationError(ValidationErrorEventArgs args)
        {
            switch (args.Action)
            {
                case ValidationErrorEventAction.Added:
                    this.validationErrorCount++;
                    break;

                case ValidationErrorEventAction.Removed:
                    this.validationErrorCount--;
                    break;
            }

            switch (this.validationErrorCount)
            {
                case 0:
                case 1:
                    this.RaisePropertyChanged(() => this.IsValid, this.validationErrorCount == 1, this.IsValid, broadcast: true);
                    break;
            }
        }
#endif
    }
}
