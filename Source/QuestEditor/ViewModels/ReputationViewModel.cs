using System.Collections.ObjectModel;

using GalaSoft.MvvmLight;

namespace QuestEditor.ViewModels
{
    public sealed class ReputationViewModel : ViewModelBase
    {
        public ReputationViewModel()
        {
            this.markers = new ObservableCollection<ReputationMarkerViewModel>();
            this.markersReadOnly = new ReadOnlyObservableCollection<ReputationMarkerViewModel>(this.markers);
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

        private readonly ObservableCollection<ReputationMarkerViewModel> markers;
        private readonly ReadOnlyObservableCollection<ReputationMarkerViewModel> markersReadOnly;
        public ReadOnlyObservableCollection<ReputationMarkerViewModel> Markers { get { return this.markersReadOnly; } }

        public void AddMarker(ReputationMarkerViewModel marker)
        {
            this.markers.Add(marker);
        }

        public void RemoveMarker(ReputationMarkerViewModel marker)
        {
            this.markers.Remove(marker);
        }
    }
}
