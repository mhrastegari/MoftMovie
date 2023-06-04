using CommunityToolkit.Maui.Views;
#if WINDOWS
using Microsoft.UI;
using WinRT.Interop;
#endif

namespace MoftMovie.Views;

public partial class MediaView : ContentPage
{
    public MediaView(string fileName, string fileUrl)
    {
        InitializeComponent();

        Title = fileName;
        media.Source = MediaSource.FromUri(fileUrl);
    }

#if WINDOWS
    private Microsoft.UI.Windowing.AppWindow GetAppWindow(MauiWinUIWindow window)
    {
        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
        return appWindow;
    }
#endif

    protected override bool OnBackButtonPressed()
    {
        HandleBack();
        return true;
    }

    private async void HandleBack()
    {
#if WINDOWS
        var window = GetParentWindow().Handler.PlatformView as MauiWinUIWindow;

        var appWindow = GetAppWindow(window);

        switch (appWindow.Presenter)
        {
            case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                if (overlappedPresenter.State == Microsoft.UI.Windowing.OverlappedPresenterState.Maximized)
                {
                    fullscreen_button.Source = ImageSource.FromFile("fullscreen.png");
                    overlappedPresenter.SetBorderAndTitleBar(true, true);
                    window.ExtendsContentIntoTitleBar = true;
                    overlappedPresenter.Restore();
                }

            break;
        }
#endif

        media.Stop();
        media.Source = null;
        await Navigation.PopModalAsync();
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        HandleBack();
    }

    private void media_Tapped(object sender, EventArgs e)
    {
        header.IsVisible = !header.IsVisible;
    }

    private void FullScreen_Clicked(object sender, EventArgs e)
    {
#if WINDOWS
        var window = GetParentWindow().Handler.PlatformView as MauiWinUIWindow;

        var appWindow = GetAppWindow(window);

        switch (appWindow.Presenter)
        {
            case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                if (overlappedPresenter.State == Microsoft.UI.Windowing.OverlappedPresenterState.Maximized)
                {
                    overlappedPresenter.SetBorderAndTitleBar(true, true);
                    window.ExtendsContentIntoTitleBar = true;
                    overlappedPresenter.Restore();
                }
                else
                {
                    overlappedPresenter.SetBorderAndTitleBar(false, false);
                    window.ExtendsContentIntoTitleBar = false;
                    overlappedPresenter.Maximize();
                }

            break;
        }
#endif
    }
}