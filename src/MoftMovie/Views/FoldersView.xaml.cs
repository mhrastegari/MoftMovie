using CommunityToolkit.Maui.Alerts;

namespace MoftMovie.Views;

public partial class FoldersView : ContentPage
{
    public static FoldersView Current { get; set; }

    private string currentFolderUrl = string.Empty;
    private string currentServer = string.Empty;

    public FoldersView()
    {
        InitializeComponent();
        Current = this;
        _ = LoadFolders();
    }

    public async Task LoadFolders()
    {
        currentServer = Preferences.Get("FileProviderUrl", "https://dl4.gemexit.com");

        var httpClient = new HttpClient();

        try
        {
            var html = await httpClient.GetStringAsync(currentServer);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var folderNodes = doc.DocumentNode.SelectNodes("//td[@class='link']/a");
            if (folderNodes == null)
            {
                folderNodes = doc.DocumentNode.SelectNodes("//pre/a[starts-with(@href, '/')]");
                foldersView.ItemsSource = folderNodes;
            }
            else
            {
                foldersView.ItemsSource = folderNodes;
            }
        }
        catch
        {
            var result = await DisplayAlert("Error", "Unable to load folders. Please check your internet connection and try again.", "Refresh", "Cancel");

            if (result)
            {
                await LoadFolders();
            }
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var selectedItem = sender as StackLayout;
        var selectedFolderNode = selectedItem.BindingContext as HtmlAgilityPack.HtmlNode;
        var folderUrl = selectedFolderNode.GetAttributeValue("href", "");
        var absoluteUrl = new Uri(new Uri(currentServer + currentFolderUrl), folderUrl).AbsoluteUri;

        if (folderUrl.EndsWith("/"))
        {
            await LoadSubfolders(absoluteUrl);
        }
        else if (folderUrl.EndsWith(".mkv") || folderUrl.EndsWith(".mp4"))
        {
            var fileName = selectedFolderNode.InnerText;
#if WINDOWS
            await Navigation.PushModalAsync(new MediaView(fileName, absoluteUrl));
#else
            await Navigation.PushAsync(new MediaView(fileName, absoluteUrl));
#endif
        }
        else
        {
#if WINDOWS
            new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo(absoluteUrl)
                {
                    UseShellExecute = true
                }
            }.Start();
#else
            await Launcher.OpenAsync(new OpenFileRequest
            {
                Title = "Open with",
                File = new ReadOnlyFile(absoluteUrl)
            });
#endif
        }
    }

    private async Task LoadSubfolders(string folderUrl)
    {
        var httpClient = new HttpClient();

        if (folderUrl.StartsWith(currentServer))
        {
            folderUrl = folderUrl.Substring(currentServer.Length);
        }

        var absoluteUrl = new Uri(new Uri(currentServer), folderUrl).AbsoluteUri;
        var html = await httpClient.GetStringAsync(absoluteUrl);
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);

        var folderNodes = doc.DocumentNode.SelectNodes("//td[@class='link']/a");
        if (folderNodes == null)
        {
            folderNodes = doc.DocumentNode.SelectNodes("//pre/a[starts-with(@href, '/')]");
            foldersView.ItemsSource = folderNodes;
        }
        else
        {
            foldersView.ItemsSource = folderNodes;
        }

        currentFolderUrl = folderUrl;
    }

    private void copy_button_Clicked(object sender, EventArgs e)
    {
        var clickedButton = sender as Frame;
        var selectedFolderNode = clickedButton.BindingContext as HtmlAgilityPack.HtmlNode;
        var folderUrl = selectedFolderNode.GetAttributeValue("href", "");

        var fileLink = currentServer + folderUrl;
        Clipboard.SetTextAsync(fileLink);

        Toast.Make("Link copied in the clipboard").Show();
    }
}