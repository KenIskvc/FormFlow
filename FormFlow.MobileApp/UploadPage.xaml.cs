using System;
using Microsoft.Maui.Media;

namespace FormFlow.MobileApp
{
    public partial class UploadPage : ContentPage
    {
        public UploadPage()
        {
            InitializeComponent();
        }

        private async void OnSelectVideoClicked(object sender, EventArgs e)
        {
            // Dein Teil: Das Auswahlfenster (FR-M07)
            var choice = await DisplayActionSheet(
                "Select video source",
                "Cancel",
                null,
                "Choose from Gallery",
                "Record with Camera");

            if (choice == "Cancel" || string.IsNullOrEmpty(choice))
                return;

            // Logik für die Auswahl aus der Galerie (Muss-Kriterium FR-M01)
            if (choice == "Choose from Gallery")
            {
                await PickVideoFromGallery();
            }
            else if (choice == "Record with Camera")
            {
                await DisplayAlert("Camera", "Camera recording is not implemented yet.", "OK");
            }
        }

        private async Task PickVideoFromGallery()
        {
            try
            {
                // Öffnet den Windows Explorer oder die Galerie (NFR-P01)
                FileResult video = await MediaPicker.Default.PickVideoAsync();

                if (video != null)
                {
                    // Erfolgreich ausgewählt (FR-M01)
                    await DisplayAlert("Success", $"Selected: {video.FileName}", "OK");

                    // Hier kann später die Analyse (FR-M02) starten
                }
            }
            catch (Exception ex)
            {
                // NFR-R01: Fehler abfangen, damit die App stabil bleibt
                await DisplayAlert("Error", $"Selection failed: {ex.Message}", "OK");
            }
        }

        // FR-M10: Zurück zur Startseite
        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}