using System;

using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class SelectStringViewModel : ViewModelBase
    {
        public SelectStringViewModel(string title, int maxLength, string initialValue)
        {
            this.Title = title;
            this.MaxLength = maxLength;
            this.value = initialValue ?? String.Empty;
        }

        public string Title { get; }
        public int MaxLength { get; }

        private string value;
        public string Value
        {
            get { return this.value; }
            set { this.Set(ref this.value, value); }
        }
    }
}
