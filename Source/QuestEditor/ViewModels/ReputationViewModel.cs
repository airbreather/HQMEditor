using System.Collections.ObjectModel;

using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class ReputationViewModel : ViewModelBase
    {
        public ReputationViewModel()
        {
            this.Markers = new ReadOnlyObservableCollection<ReputationMarkerViewModel>(this.markersMutable);
        }

        private int id;
        public int Id
        {
            get { return this.id; }
            set { this.Set(ref this.id, value); }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.Set(ref this.name, value); }
        }

        private readonly ObservableCollection<ReputationMarkerViewModel> markersMutable = new ObservableCollection<ReputationMarkerViewModel>();
        public ReadOnlyObservableCollection<ReputationMarkerViewModel> Markers { get; }

        public void AddMarker(ReputationMarkerViewModel marker) => this.markersMutable.Add(marker);
        public void RemoveMarker(ReputationMarkerViewModel marker) => this.markersMutable.Remove(marker);
    }
}
