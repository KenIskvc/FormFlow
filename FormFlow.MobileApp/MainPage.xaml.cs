using System.Collections.ObjectModel;

namespace FormFlow.MobileApp
{
    public partial class MainPage : ContentPage
    {
        public class Club
        {
            public string Name { get; set; } = string.Empty;
        }

        public ObservableCollection<Club> AssignedClubs { get; set; }

        public MainPage()
        {
            InitializeComponent();

            AssignedClubs = new ObservableCollection<Club>
            {
                new Club { Name = "Vienna Volleyball Union" },
                new Club { Name = "Danube Volleys" }
            };

            ClubsListView.ItemsSource = AssignedClubs;
        }

        // FR-M06 Interaction
        private async void OnClubSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Club selectedClub)
            {
                await DisplayAlert("Club Info", $"Opening details for: {selectedClub.Name}", "OK");
                ((CollectionView)sender).SelectedItem = null;
            }
        }

        // FR-M07 Placeholder
        private async void OnUploadClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Navigation", "Opening Upload Window...", "OK");
        }

        // FR-M08 Placeholder
        private async void OnAnalysesClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Navigation", "Opening Analysis Reports...", "OK");
        }

        // FR-M09 Placeholder
        private async void OnManagementClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Navigation", "Opening Video Management...", "OK");
        }
    }
}