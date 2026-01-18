using System.Text;

namespace FormFlow.MobileApp;
public partial class MainPage : ContentPage {
    private readonly HttpClient _client;
    public MainPage(HttpClient client) {
        InitializeComponent();
        _client = client;
    }

    // private void OnCounterClicked(object? sender, EventArgs e)
    // {
    //     count++;
    //
    //     if (count == 1)
    //         CounterBtn.Text = $"Clicked {count} time";
    //     else
    //         CounterBtn.Text = $"Clicked {count} times";
    //
    //     SemanticScreenReader.Announce(CounterBtn.Text);
    // }

    private void NavigateToUpload(object? sender, EventArgs e) {

        /*if(count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);*/
    }


    
