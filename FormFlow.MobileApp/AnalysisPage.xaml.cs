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

    private void SetInitialLayout()
    {
        AnalysisCollection.ItemsLayout =
            new GridItemsLayout(4, ItemsLayoutOrientation.Vertical)
            {
                HorizontalItemSpacing = 10,
                VerticalItemSpacing = 10
            };
    }

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

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateGridSpan();
    }

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

    private void DeleteAnalysis(AnalysisListItem item)
    {
        // Session-Analyse
        if (!item.IsPersisted)
        {
            _analyses.Remove(item);
            return;
        }

        // Persistierte Analyse
        // später:
        // await Api.DeleteAnalysis(item.AnalysisId.Value);

        _analyses.Remove(item);
    }

    private async Task DownloadPdfAsync(AnalysisListItem item)
    {
        // später:
        // var pdfBytes = await Api.GetAnalysisPdf(item.AnalysisId);

        await DisplayAlert(
            "Download",
            "PDF download will be implemented here.",
            "OK");
    }

    private async void OpenAnalysis(AnalysisListItem item)
    {
        // Später: eigene Detail-Page
        await DisplayAlert(
            "Analysis details",
            item.Report,
            "OK"
        );
    }

}
