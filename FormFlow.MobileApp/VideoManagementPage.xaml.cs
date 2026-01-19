using System.Collections.ObjectModel;
using FormFlow.MobileApp.DTOs;
using FormFlow.MobileApp.Services;

namespace FormFlow.MobileApp;

public partial class VideoManagementPage : ContentPage
{
    private readonly VideoService _videoService;
    private readonly TokenStore _tokenStore;

    public ObservableCollection<VideoListDto> MyVideos { get; } = new();

    public VideoManagementPage()
    {
        InitializeComponent();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7110/")
        };

        _videoService = new VideoService(httpClient);
        _tokenStore = new TokenStore();

        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadVideosAsync();
    }

    private async Task LoadVideosAsync()
    {
        try
        {
            var token = await _tokenStore.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
                return;

            var videos = await _videoService.GetMyVideosAsync(token);

            MyVideos.Clear();
            foreach (var v in videos)
                MyVideos.Add(v);
        }
        catch
        {
            await DisplayAlert("Fehler", "Videos konnten nicht geladen werden.", "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is not Button btn ||
            btn.CommandParameter is not VideoListDto video)
            return;

        bool confirm = await DisplayAlert(
            "Löschen",
            $"Möchtest du „{video.FileName}“ wirklich löschen?",
            "Ja",
            "Nein");

        if (!confirm)
            return;

        try
        {
            var token = await _tokenStore.GetAccessTokenAsync();
            await _videoService.DeleteVideoAsync(video.Id, token);
            MyVideos.Remove(video);
        }
        catch
        {
            await DisplayAlert("Fehler", "Video konnte nicht gelöscht werden.", "OK");
        }
    }

    private async void OnRenameClicked(object sender, EventArgs e)
    {
        if (sender is not Button btn ||
            btn.CommandParameter is not VideoListDto video)
            return;

        var newName = await DisplayPromptAsync(
            "Umbenennen",
            "Neuer Dateiname:",
            initialValue: video.FileName);

        if (string.IsNullOrWhiteSpace(newName) ||
            newName == video.FileName)
            return;

        try
        {
            var token = await _tokenStore.GetAccessTokenAsync();
            await _videoService.RenameVideoAsync(video.Id, newName, token);

            int index = MyVideos.IndexOf(video);
            MyVideos.RemoveAt(index);
            video.FileName = newName;
            MyVideos.Insert(index, video);
        }
        catch
        {
            await DisplayAlert("Fehler", "Umbenennen fehlgeschlagen.", "OK");
        }
    }

    private async void OnReAnalyzeClicked(object sender, EventArgs e)
    {
        if (sender is not Button btn ||
            btn.CommandParameter is not VideoListDto video)
            return;

        bool confirm = await DisplayAlert(
            "Re-Analyse",
            $"Video „{video.FileName}“ erneut analysieren?",
            "Ja",
            "Abbrechen");

        if (!confirm)
            return;

        try
        {
            var token = await _tokenStore.GetAccessTokenAsync();

            var result = await _videoService.ReAnalyzeAsync(
                video.Id,
                token,
                CancellationToken.None);

            await DisplayAlert(
                "Analyse abgeschlossen",
                $"Fehler gefunden: {result.ErrorCount}",
                "OK");
        }
        catch
        {
            await DisplayAlert("Fehler", "Re-Analyse fehlgeschlagen.", "OK");
        }
    }
}
