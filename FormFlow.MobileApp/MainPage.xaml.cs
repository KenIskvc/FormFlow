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

        
    }
}