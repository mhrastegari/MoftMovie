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
        LoadFolders();
    }

    public async Task LoadFolders()
    {
        currentServer = Preferences.Get("FileProviderUrl", "https://dl4.gemexit.com");
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(currentServer);
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);

        var folderNodes = doc.DocumentNode.SelectNodes("//td[@class='link']/a");
        if (folderNodes != null)
        {
            foldersView.ItemsSource = folderNodes;
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
            await Navigation.PushModalAsync(new MediaView(fileName, absoluteUrl));
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
        if (folderNodes != null)
        {
            foldersView.ItemsSource = folderNodes;
        }

        currentFolderUrl = folderUrl;
    }
}