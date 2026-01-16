using System.Collections.ObjectModel;

namespace FormFlow.MobileApp;

public partial class VideoManagementPage : ContentPage
{
    public class VideoItem
    {
        public string FileName { get; set; } = string.Empty;
        public string UploadDate { get; set; } = string.Empty;
    }

    public ObservableCollection<VideoItem> MyVideos { get; set; }

    public VideoManagementPage()
    {
        InitializeComponent();

        // FR-M09: Beispiel-Daten
        MyVideos = new ObservableCollection<VideoItem>
        {
            new VideoItem { FileName = "Serve_Practice.mp4", UploadDate = "2026-01-14" },
            new VideoItem { FileName = "Match_Analysis_Final.mp4", UploadDate = "2026-01-15" }
        };

        VideosListView.ItemsSource = MyVideos;
    }

    private async void OnReAnalyzeClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Analysis", "Re-analysis started for this video.", "OK");
    }

    private async void OnRenameClicked(object sender, EventArgs e)
    {
        string newName = await DisplayPromptAsync("Rename", "New name:");
        if (!string.IsNullOrEmpty(newName))
        {
            await DisplayAlert("Success", $"Renamed to {newName}", "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Delete", "Are you sure?", "Yes", "No");
        if (confirm)
        {
            // Logik zum Entfernen aus der Liste (optional für den Prototyp)
            await DisplayAlert("Deleted", "Video removed.", "OK");
        }
    }
}