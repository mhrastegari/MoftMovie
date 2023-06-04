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
        media.Stop();
        media.Source = null;
        
        return base.OnBackButtonPressed();
    }
}