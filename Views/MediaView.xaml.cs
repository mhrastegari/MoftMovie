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
        media.Stop();
        media.Source = null;

#if ANDROID
        if ((int)Android.OS.Build.VERSION.SdkInt >= 19)
        {
            Platform.CurrentActivity.Window.DecorView.SystemUiVisibility &= ~(Android.Views.StatusBarVisibility)(
                Android.Views.SystemUiFlags.LayoutStable |
                Android.Views.SystemUiFlags.LayoutHideNavigation |
                Android.Views.SystemUiFlags.LayoutFullscreen |
                Android.Views.SystemUiFlags.HideNavigation |
                Android.Views.SystemUiFlags.Fullscreen |
                Android.Views.SystemUiFlags.Immersive);
        }
#endif

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

                break;
        }
        await Navigation.PopModalAsync();
#endif
        await Navigation.PopAsync();
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        HandleBack();
    }

    private void SetupFullScreen()
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
                    header.Margin = new Thickness(0, 10);
                    overlappedPresenter.Restore();
                }
                else
                {
                    overlappedPresenter.SetBorderAndTitleBar(false, false);
                    window.ExtendsContentIntoTitleBar = false;
                    header.Margin = new Thickness(0);
                    overlappedPresenter.Maximize();
                }

                break;
        }
#endif

#if ANDROID
        var activity = Platform.CurrentActivity;

        if ((int)Android.OS.Build.VERSION.SdkInt >= 19)
        {
            if (activity.Window.DecorView.SystemUiVisibility == Android.Views.StatusBarVisibility.Visible)
            {
                activity.Window.DecorView.SystemUiVisibility = (Android.Views.StatusBarVisibility)(
                    Android.Views.SystemUiFlags.LayoutStable |
                    Android.Views.SystemUiFlags.LayoutHideNavigation |
                    Android.Views.SystemUiFlags.LayoutFullscreen |
                    Android.Views.SystemUiFlags.HideNavigation |
                    Android.Views.SystemUiFlags.Fullscreen |
                    Android.Views.SystemUiFlags.Immersive);
            }
            else
            {
                activity.Window.DecorView.SystemUiVisibility &= ~(Android.Views.StatusBarVisibility)(
                    Android.Views.SystemUiFlags.LayoutStable |
                    Android.Views.SystemUiFlags.LayoutHideNavigation |
                    Android.Views.SystemUiFlags.LayoutFullscreen |
                    Android.Views.SystemUiFlags.HideNavigation |
                    Android.Views.SystemUiFlags.Fullscreen |
                    Android.Views.SystemUiFlags.Immersive);
            }
        }
#endif
    }

    private void FullScreen_Clicked(object sender, EventArgs e)
    {
        SetupFullScreen();
    }

    private void media_DoubleTapped(object sender, EventArgs e)
    {
        SetupFullScreen();
    }

    private void media_Tapped(object sender, EventArgs e)
    {
        header.IsVisible = !header.IsVisible;
    }
}