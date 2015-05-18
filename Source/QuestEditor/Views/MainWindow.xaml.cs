using GalaSoft.MvvmLight.Messaging;
using QuestEditor.Messages;
using QuestEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
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
        private Canvas capturedCanvas;
        private Ellipse capturedEllipse;
        private Point capturedPos;
        private bool draggedOut;

        public MainWindow()
        {
            this.viewModel = new MainViewModel();
            Dialogs.Register(Messenger.Default, this);

            this.InitializeComponent();
        }

        public MainViewModel ViewModel { get { return this.viewModel; } }

        private void OnAddQuestLinkButtonClick(object sender, EventArgs e)
        {
            this.capturedCanvas = null;
            this.capturedEllipse = null;
            this.draggedOut = false;

            this.viewModel.MouseMode = MouseMode.CreateLink;
            this.PushCursor(Cursors.Pen);
        }

        private void OnRemoveQuestLinkButtonClick(object sender, EventArgs e)
        {
            this.capturedCanvas = null;
            this.capturedEllipse = null;
            this.draggedOut = false;

            this.viewModel.MouseMode = MouseMode.DeleteLink;
            this.PushCursor(Cursors.Cross);
        }

        private void OnRestoreMouseButtonClick(object sender, EventArgs e)
        {
            this.capturedCanvas = null;
            this.capturedEllipse = null;
            this.draggedOut = false;

            this.viewModel.MouseMode = MouseMode.Drag;
            if (this.cursorStack.Count > 0)
            {
                this.PopCursor();
            }
        }

        private void OnQuestMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            this.capturedCanvas = null;
            this.capturedEllipse = null;
            this.draggedOut = false;

            if (this.viewModel.MouseMode == MouseMode.DeleteLink)
            {
                return;
            }

            Ellipse ellipse = (Ellipse)sender;

            Point pos = e.GetPosition(this.capturedCanvas);

            if (ellipse.CaptureMouse())
            {
                this.capturedCanvas = FindParent<Canvas>(ellipse);
                this.capturedEllipse = ellipse;
                this.capturedPos = pos;
            }
        }

        private void OnQuestMouseMove(object sender, MouseEventArgs e)
        {
            if (this.capturedEllipse == null ||
                this.capturedEllipse != sender)
            {
                return;
            }

            QuestViewModel quest = (QuestViewModel)this.capturedEllipse.DataContext;
            Point pos = e.GetPosition(this.capturedCanvas);

            switch (this.viewModel.MouseMode)
            {
                case MouseMode.Drag:
                    this.draggedOut |= this.capturedCanvas.InputHitTest(pos) != sender;

                    if (this.draggedOut)
                    {
                        quest.XPos = (int)pos.X;
                        quest.YPos = (int)pos.Y;
                    }

                    break;
            }
        }

        private void OnQuestMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (this.capturedEllipse == null ||
                this.capturedEllipse != sender)
            {
                return;
            }

            Point pos = e.GetPosition(this.capturedCanvas);

            IInputElement hitTest = this.capturedCanvas.InputHitTest(pos);

            QuestViewModel quest = (QuestViewModel)this.capturedEllipse.DataContext;
            switch (this.viewModel.MouseMode)
            {
                case MouseMode.Drag:
                    this.draggedOut |= hitTest != sender;
                    if (this.draggedOut)
                    {
                        quest.XPos = (int)pos.X;
                        quest.YPos = (int)pos.Y;
                    }
                    else
                    {
                        quest.EditCommand.Execute(false);
                    }

                    break;

                case MouseMode.CreateLink:
                    Ellipse other = hitTest as Ellipse;
                    if (other != null && other != sender)
                    {
                        this.viewModel.SelectedQuestSet.AddQuestLink(quest, (QuestViewModel)other.DataContext);
                    }

                    this.viewModel.MouseMode = MouseMode.Drag;
                    this.PopCursor();
                    break;
            }

            this.capturedEllipse.ReleaseMouseCapture();
            this.capturedCanvas = null;
            this.capturedEllipse = null;
            this.draggedOut = false;
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
