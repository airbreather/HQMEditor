using System.Windows;

using Microsoft.Win32;

using GalaSoft.MvvmLight.Messaging;

using QuestEditor.Messages;
using QuestEditor.ViewModels;
using QuestEditor.Views;

namespace QuestEditor
{
    internal static class Dialogs
    {
        internal static void Register(IMessenger messenger, object recipient)
        {
            messenger.Register<SelectFileMessage>(recipient, SelectFile);
            messenger.Register<EditQuestMessage>(recipient, EditQuest);
            messenger.Register<SelectStringMessage>(recipient, SelectString);
        }

        private static void SelectFile(SelectFileMessage message)
        {
            Window sender = message.Target as Window;

            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog(sender) != true)
            {
                return;
            }

            message.SelectedFilePath = dlg.FileName;
        }

        private static void EditQuest(EditQuestMessage message)
        {
            Window sender = message.Target as Window;

            EditQuestWindow dlg = new EditQuestWindow(message.Quest) { Owner = sender };
            message.Accepted = dlg.ShowDialog() == true;
        }

        private static void SelectString(SelectStringMessage message)
        {
            Window sender = message.Target as Window;

            SelectStringViewModel viewModel = new SelectStringViewModel(message.Title, message.MaxLength, message.StringValue);
            SelectStringWindow dlg = new SelectStringWindow(viewModel) { Owner = sender };
            if (dlg.ShowDialog() != true)
            {
                return;
            }

            message.Accepted = true;
            message.StringValue = viewModel.Value;
        }
    }
}
