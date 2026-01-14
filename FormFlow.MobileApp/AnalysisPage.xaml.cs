using FormFlow.MobileApp.Models;
using System.Collections.ObjectModel;

namespace FormFlow.MobileApp;

public partial class AnalysisPage : ContentPage
{
    private readonly ObservableCollection<AnalysisItem> _analyses = new();

    public AnalysisPage()
    {
        InitializeComponent();

        AnalysisCollection.ItemsSource = _analyses;
        LoadDummyAnalyses();
        AnalysisCollection.SelectionChanged += OnAnalysisSelected;

        // WICHTIG: Grid-Layout sofort setzen (sonst bleibt es eine Liste!)
        AnalysisCollection.ItemsLayout =
            new GridItemsLayout(4, ItemsLayoutOrientation.Vertical)
            {
                HorizontalItemSpacing = 10,
                VerticalItemSpacing = 10
            };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateGridSpan();
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
        if (e.CurrentSelection.FirstOrDefault() is not AnalysisItem selected)
            return;

        await DisplayAlert(
            "Analyse öffnen",
            $"Analyse: {selected.Title}",
            "OK"
        );

        ((CollectionView)sender).SelectedItem = null;
    }

    private void LoadDummyAnalyses()
    {
        _analyses.Clear();

        _analyses.Add(new AnalysisItem
        {
            Title = "Sprungaufschlag",
            Date = "12.01.2026",
            Status = "Fertig",
            StatusColor = Colors.Green,
            ErrorCountText = "3 Fehler erkannt"
        });

        _analyses.Add(new AnalysisItem
        {
            Title = "Blockbewegung",
            Date = "11.01.2026",
            Status = "In Analyse",
            StatusColor = Colors.Orange,
            ErrorCountText = "–"
        });

        _analyses.Add(new AnalysisItem
        {
            Title = "Blockbewegung",
            Date = "11.01.2026",
            Status = "In Analyse",
            StatusColor = Colors.Orange,
            ErrorCountText = "–"
        });

        _analyses.Add(new AnalysisItem
        {
            Title = "Blockbewegung",
            Date = "11.01.2026",
            Status = "In Analyse",
            StatusColor = Colors.Orange,
            ErrorCountText = "–"
        });

        _analyses.Add(new AnalysisItem
        {
            Title = "Blockbewegung",
            Date = "11.01.2026",
            Status = "In Analyse",
            StatusColor = Colors.Orange,
            ErrorCountText = "–"
        });

        _analyses.Add(new AnalysisItem
        {
            Title = "Blockbewegung",
            Date = "11.01.2026",
            Status = "In Analyse",
            StatusColor = Colors.Orange,
            ErrorCountText = "–"
        });

        _analyses.Add(new AnalysisItem
        {
            Title = "Ready Position",
            Date = "10.01.2026",
            Status = "Fehlgeschlagen",
            StatusColor = Colors.Red,
            ErrorCountText = "Analyse nicht möglich"
        });
    }
}
