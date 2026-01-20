using FormFlow.MobileApp.Services;
using FormFlow.MobileApp.DTOs;

namespace FormFlow.MobileApp;

public partial class UploadPage : ContentPage
{
    private FileResult? _selectedVideo;
    private readonly VideoService _videoService;
    private readonly TokenStore _tokenStore;
    private string _userToken = "";

    public UploadPage()
    {
        InitializeComponent();

        _tokenStore = new TokenStore();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7110/"),
            Timeout = TimeSpan.FromMinutes(5)
        };

        _videoService = new VideoService(httpClient);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CheckLoginStatusAsync();
    }

    private async Task CheckLoginStatusAsync()
    {
        _userToken = await _tokenStore.GetAccessTokenAsync() ?? "";
        ActionButton.Text = string.IsNullOrEmpty(_userToken)
            ? "Analyse starten (Gast)"
            : "Upload & Analyse starten";
    }

    private async void OnSelectVideoClicked(object sender, EventArgs e)
    {
        try
        {
            _selectedVideo = await MediaPicker.Default.PickVideoAsync();
            if (_selectedVideo != null)
            {
                FileNameLabel.Text = $"Datei: {_selectedVideo.FileName}";
                SelectedVideoArea.IsVisible = true;
            }
        }
        catch
        {
            await DisplayAlert("Error", "Video couldn't be loaded.", "OK");
        }
    }

    private async void OnUploadAndAnalyzeClicked(object sender, EventArgs e)
    {
        if (_selectedVideo == null)
            return;

        try
        {
            SetLoading(true);

            UploadResultDto result =
                await _videoService.UploadAndAnalyzeAsync(
                    _selectedVideo,
                    _userToken,
                    CancellationToken.None);

            await DisplayAlert(
                "Anaysis finished",
                $"Analysis created at: {result.Analysis.CreatedAt}",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Upload-Error", ex.Message, "OK");
        }
        finally
        {
            SetLoading(false);
        }
    }

    private void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsRunning = isLoading;
        ActionButton.IsEnabled = !isLoading;

        ActionButton.Text = isLoading
            ? "Verarbeite..."
            : string.IsNullOrEmpty(_userToken)
                ? "Start Analysis"
                : "Upload & Analyze";
    }
}
