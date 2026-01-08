namespace FormFlow.MobileApp;

public partial class UploadPage : ContentPage {
    public UploadPage() {
        InitializeComponent();
    }
    private async void OnSelectVideoClicked(object sender, EventArgs e) {
        var choice = await DisplayActionSheet(
            "Select video source",
            "Cancel",
            null,
            "Choose from storage",
            "Record with camera");

        if(choice == "Cancel" || string.IsNullOrEmpty(choice))
            return;

        await DisplayAlert(
            "Selection",
            $"You selected: {choice}",
            "OK");
    }
}