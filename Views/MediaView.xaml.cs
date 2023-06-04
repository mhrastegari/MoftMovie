using CommunityToolkit.Maui.Views;

namespace MoftMovie.Views;

public partial class MediaView : ContentPage
{
    public MediaView(string fileName, string fileUrl)
    {
        InitializeComponent();

        Title = fileName;
        media.Source = MediaSource.FromUri(fileUrl);
    }

    protected override bool OnBackButtonPressed()
    {
        HandleBack();
        return true;
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        HandleBack();
    }

    private async void HandleBack()
    {
        media.Stop();
        media.Source = null;
        await Navigation.PopModalAsync();
    }

    private void media_Tapped(object sender, EventArgs e)
    {
        header.IsVisible = !header.IsVisible;
    }
}