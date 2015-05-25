using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class ReputationMarkerViewModel : ViewModelBase
    {
        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.Set(ref this.name, value); }
        }

        private int value;
        public int Value
        {
            get { return this.value; }
            set { this.Set(ref this.value, value); }
        }
    }
}
