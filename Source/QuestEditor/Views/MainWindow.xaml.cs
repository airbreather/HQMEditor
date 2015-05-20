using GalaSoft.MvvmLight.Messaging;
using QuestEditor.Messages;
using QuestEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace QuestEditor.Views
{
    public sealed partial class MainWindow
    {
        private readonly MainViewModel viewModel;
        private readonly Stack<Cursor> cursorStack = new Stack<Cursor>();

        public MainWindow()
        {
            this.viewModel = new MainViewModel();
            Dialogs.Register(Messenger.Default, this);

            this.InitializeComponent();
        }

        public MainViewModel ViewModel { get { return this.viewModel; } }

        private void OnAddQuestLinkButtonClick(object sender, EventArgs e)
        {
            this.viewModel.MouseMode = MouseMode.CreateLink;
            this.PushCursor(Cursors.Pen);
        }

        private void OnRemoveQuestLinkButtonClick(object sender, EventArgs e)
        {
            this.viewModel.MouseMode = MouseMode.DeleteLink;
            this.PushCursor(Cursors.Cross);
        }

        private void OnRestoreMouseButtonClick(object sender, EventArgs e)
        {
            this.viewModel.MouseMode = MouseMode.Drag;
            if (this.cursorStack.Count > 0)
            {
                this.PopCursor();
            }
        }

        private async void OnQuestMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (this.viewModel.MouseMode == MouseMode.DeleteLink)
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

            bool draggedOut = false;
            Point lastPos = default(Point);
            using (mouseLeftButtonUps.Subscribe(ep => lastPos = ep.EventArgs.GetPosition(capturedCanvas)))
            {
                switch (this.viewModel.MouseMode)
                {
                    case MouseMode.Drag:
                        await mouseMoves.TakeUntil(mouseLeftButtonUps)
                                        .ForEachAsync(ep =>
                                                      {
                                                          Point pos = e.GetPosition(capturedCanvas);
                                                          draggedOut |= capturedCanvas.InputHitTest(pos) != ellipse;
                                                          if (draggedOut)
                                                          {
                                                              quest.XPos = (int)pos.X;
                                                              quest.YPos = (int)pos.Y;
                                                          }
                                                      });
                        break;

                    case MouseMode.CreateLink:
                        await mouseMoves.TakeUntil(mouseLeftButtonUps);
                        break;
                }
            }

            IInputElement lastHitTest = capturedCanvas.InputHitTest(lastPos);

            ellipse.ReleaseMouseCapture();

            switch (this.viewModel.MouseMode)
            {
                case MouseMode.Drag:
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
                        this.viewModel.SelectedQuestSet.AddQuestLink(quest, (QuestViewModel)other.DataContext);
                    }

                    this.viewModel.MouseMode = MouseMode.Drag;
                    this.PopCursor();
                    break;
            }
        }

        private void OnQuestLinkMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (this.viewModel.MouseMode != MouseMode.DeleteLink)
            {
                return;
            }

            Line line = (Line)sender;
            QuestLinkViewModel questLink = (QuestLinkViewModel)line.DataContext;
            this.viewModel.SelectedQuestSet.RemoveQuestLink(questLink);
            this.viewModel.MouseMode = MouseMode.Drag;
            this.PopCursor();
        }

        private void PushCursor(Cursor newCursor)
        {
            this.cursorStack.Push(this.Cursor);
            this.Cursor = newCursor;
        }

        private void PopCursor()
        {
            this.Cursor = this.cursorStack.Pop();
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
