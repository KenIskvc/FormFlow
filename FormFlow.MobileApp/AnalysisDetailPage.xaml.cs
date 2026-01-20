using FormFlow.MobileApp.ViewModels;
using FormFlow.MobileApp.Models;

namespace FormFlow.MobileApp;

public partial class AnalysisDetailPage : ContentPage
{
    public AnalysisDetailPage(AnalysisListItem item)
    {
        InitializeComponent();
        BindingContext = new AnalysisDetailViewModel(item);
    }
}
