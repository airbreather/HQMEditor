using System;

using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class SelectStringViewModel : ViewModelBase
    {
        public SelectStringViewModel(string title, int maxLength, string initialValue)
        {
            this.title = title;
            this.maxLength = maxLength;
            this.value = initialValue ?? String.Empty;
        }

        private readonly string title;
        public string Title { get { return this.title; } }

        private readonly int maxLength;
        public int MaxLength { get { return this.maxLength; } }

        private string value;
        public string Value
        {
            get { return this.value; }
            set { this.Set(ref this.value, value); }
        }
    }
}
