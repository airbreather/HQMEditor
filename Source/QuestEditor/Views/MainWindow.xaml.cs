using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using GalaSoft.MvvmLight.Messaging;

using QuestEditor.ViewModels;

namespace QuestEditor.Views
{
    public sealed partial class MainWindow
    {
        public MainWindow()
        {
            this.ViewModel = new MainViewModel();
            Dialogs.Register(Messenger.Default, this);

            this.InitializeComponent();
        }

        public MainViewModel ViewModel { get; }

        private async void OnQuestMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (this.ViewModel.MouseMode == MouseMode.DeleteLink)
            {
                return;
            }

            Ellipse ellipse = (Ellipse)sender;

            if (!ellipse.CaptureMouse())
            {
                return;
            }

            Canvas capturedCanvas = FindParent<Canvas>(ellipse);
            QuestViewModel quest = (QuestViewModel)ellipse.DataContext;

            var mouseMoves = Observable.FromEventPattern<MouseEventArgs>(ellipse, "MouseMove");
            var mouseLeftButtonUps = Observable.FromEventPattern<MouseEventArgs>(ellipse, "MouseLeftButtonUp");

            bool lostMouseCapture = false;
            var lostMouseCaptures = Observable.FromEventPattern<MouseEventArgs>(ellipse, "LostMouseCapture")
                                              .Do(_ => lostMouseCapture = true);

            var mouseButtonUpOrLostMouseCapture = Observable.Merge(mouseLeftButtonUps, lostMouseCaptures);

            bool draggedOut = false;
            Point lastPos = e.GetPosition(capturedCanvas);
            using (mouseButtonUpOrLostMouseCapture.Subscribe(ep => lastPos = ep.EventArgs.GetPosition(capturedCanvas)))
            {
                switch (this.ViewModel.MouseMode)
                {
                    case MouseMode.EditQuests:
                        await mouseMoves.TakeUntil(mouseButtonUpOrLostMouseCapture)
                                        .ForEachAsync(ep =>
                                                      {
                                                          Point pos = ep.EventArgs.GetPosition(capturedCanvas);
                                                          draggedOut |= capturedCanvas.InputHitTest(pos) != ellipse;
                                                          if (draggedOut)
                                                          {
                                                              quest.XPos = (int)pos.X;
                                                              quest.YPos = (int)pos.Y;
                                                              lastPos = pos;
                                                          }
                                                      });
                        break;

                    case MouseMode.CreateLink:
                        await mouseMoves.TakeUntil(mouseButtonUpOrLostMouseCapture);
                        break;
                }
            }

            if (lostMouseCapture)
            {
                return;
            }

            IInputElement lastHitTest = capturedCanvas.InputHitTest(lastPos);

            ellipse.ReleaseMouseCapture();

            switch (this.ViewModel.MouseMode)
            {
                case MouseMode.EditQuests:
                    draggedOut |= lastHitTest != sender;

                    if (draggedOut)
                    {
                        quest.XPos = (int)lastPos.X;
                        quest.YPos = (int)lastPos.Y;
                    }
                    else
                    {
                        quest.EditCommand.Execute(false);
                    }

                    break;

                case MouseMode.CreateLink:
                    Ellipse other = lastHitTest as Ellipse;
                    if (other != null && other != sender)
                    {
                        this.ViewModel.SelectedQuestSet.AddQuestLink(quest, (QuestViewModel)other.DataContext);
                    }

                    this.ViewModel.MouseMode = MouseMode.EditQuests;
                    break;
            }
        }

        private void OnQuestLinkMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (this.ViewModel.MouseMode != MouseMode.DeleteLink)
            {
                return;
            }

            Line line = (Line)sender;
            QuestLinkViewModel questLink = (QuestLinkViewModel)line.DataContext;
            this.ViewModel.SelectedQuestSet.RemoveQuestLink(questLink);
            this.ViewModel.MouseMode = MouseMode.EditQuests;
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            T result;

            do
            {
                child = VisualTreeHelper.GetParent(child);
                result = child as T;
            }
            while (result == null);

            return result;
        }
    }
}
