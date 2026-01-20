using FormFlow.MobileApp.DTOs;
using FormFlow.MobileApp.Contracts;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace FormFlow.MobileApp;

public partial class AnalysisPage : ContentPage
{
    private readonly ObservableCollection<AnalysisListItem> _analyses = new();

    private readonly ITokenStore _tokenStore;
    private readonly IAnalysisApi _analysisApi;

    // Constructor that initializes dependencies and sets up the page.
    // It wires the collection to the UI and prepares the initial layout.
    public AnalysisPage(
        ITokenStore tokenStore,
        IAnalysisApi analysisApi)
    {
        InitializeComponent();
        _tokenStore = tokenStore;
        _analysisApi = analysisApi;

        AnalysisCollection.ItemsSource = _analyses;
        AnalysisCollection.SelectionChanged += OnAnalysisSelected;

        SetInitialLayout();
    }

    // Called every time the page becomes visible.
    // Responsible for loading the user's persisted analyses
    // from the backend and refreshing the UI
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        UpdateGridSpan();

        var accessToken = await _tokenStore.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(accessToken))
            return; 

        _analyses.Clear();

        var persistedAnalyses =
            await _analysisApi.GetMyAnalysesAsync(CancellationToken.None);

        foreach (var dto in persistedAnalyses)
            AddAnalysisFromDto(dto);
    }

    // Sets an initial grid layout with a fixed number of columns.
    // This is used before the actual screen size is known.
    private void SetInitialLayout()
    {
        AnalysisCollection.ItemsLayout =
            new GridItemsLayout(4, ItemsLayoutOrientation.Vertical)
            {
                HorizontalItemSpacing = 10,
                VerticalItemSpacing = 10
            };
    }

    // Converts an AnalysisResponseDto (transport object)
    // into an AnalysisListItem (UI model) and inserts it
    // at the top of the list.
    public void AddAnalysisFromDto(AnalysisResponseDto dto)
    {
        _analyses.Insert(0, new AnalysisListItem
        {
            AnalysisId = dto.AnalysisId,
            CreatedAt = dto.CreatedAt,
            ErrorCount = dto.ErrorCount,
            Report = dto.Report,
            VideoTitle = dto.VideoTitle
        });
    }

    // Triggered whenever the page size changes (e.g. window resize).
    // Used to recalculate the grid layout dynamically.
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateGridSpan();
    }

    // Dynamically calculates how many cards fit into the available width
    // and updates the grid layout accordingly.
    private void UpdateGridSpan()
    {
        if (Width <= 0)
            return;

        const double targetCardWidth = 220;

        int span = Math.Max(4, (int)(Width / targetCardWidth));

        AnalysisCollection.ItemsLayout =
            new GridItemsLayout(span, ItemsLayoutOrientation.Vertical)
            {
                HorizontalItemSpacing = 10,
                VerticalItemSpacing = 10
            };
    }

    // Handles selection of an analysis card.
    // Shows an action sheet that lets the user open, download,
    // or delete the selected analysis.
    private async void OnAnalysisSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not AnalysisListItem selected)
            return;

        string action = await DisplayActionSheet(
            "Analysis",
            "Cancel",
            null,
            "Open",
            "Download PDF",
            "Delete");

        switch (action)
        {
            case "Open":
                OpenAnalysis(selected);
                break;

            case "Download PDF":
                await DownloadPdfAsync(selected);
                break;

            case "Delete":
                DeleteAnalysis(selected);
                break;
        }

        ((CollectionView)sender).SelectedItem = null;
    }

    // Deletes an analysis.
    // Session-only analyses are removed locally,
    // persisted analyses are deleted via the backend API.
    private async void DeleteAnalysis(AnalysisListItem item)
    {
        if (!item.IsPersisted)
        {
            _analyses.Remove(item);
            return;
        }

        bool confirm = await DisplayAlert(
            "Delete analysis",
            "Are you sure you want to delete this analysis? This action cannot be undone.",
            "Delete",
            "Cancel");

        if (!confirm)
            return;

        try
        {
            await _analysisApi.DeleteAnalysisAsync(
                item.AnalysisId!.Value,
                CancellationToken.None);

            _analyses.Remove(item);
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                "The analysis could not be deleted.",
                "OK");
        }
    }

    // Downloads the analysis report as a PDF.
    // This is currently a placeholder and will later
    // call the backend PDF endpoint.
    private async Task DownloadPdfAsync(AnalysisListItem item)
    {
        // später:
        // var pdfBytes = await Api.GetAnalysisPdf(item.AnalysisId);

        await DisplayAlert(
            "Download",
            "PDF download will be implemented here.",
            "OK");
    }

    // Navigates to the analysis detail page
    // to show a structured, human-readable analysis view.
    private async void OpenAnalysis(AnalysisListItem item)
    {
        await Navigation.PushAsync(new AnalysisDetailPage(item));
    }

}
