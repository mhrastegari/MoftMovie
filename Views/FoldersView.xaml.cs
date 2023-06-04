using HtmlAgilityPack;
using System.Diagnostics;

namespace MoftMovie.Views;

public partial class FoldersView : ContentPage
{
    private string currentFolderUrl = string.Empty;

    public FoldersView()
    {
        InitializeComponent();
        LoadFolders();
    }

    private async Task LoadFolders()
    {
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync("https://dl4.gemexit.com");
        var doc = new HtmlDocument();
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
        var selectedFolderNode = selectedItem.BindingContext as HtmlNode;
        var folderUrl = selectedFolderNode.GetAttributeValue("href", "");
        var absoluteUrl = new Uri(new Uri("https://dl4.gemexit.com/" + currentFolderUrl), folderUrl).AbsoluteUri;

        if (folderUrl.EndsWith("/"))
        {
            await LoadSubfolders(absoluteUrl);
        }
        else if (folderUrl.EndsWith(".mkv") || folderUrl.EndsWith(".mp4"))
        {
            var fileName = selectedFolderNode.InnerText;
            await Navigation.PushAsync(new MediaView(fileName, absoluteUrl));
        }
        else
        {
#if WINDOWS
            new Process
            {
                StartInfo = new ProcessStartInfo(absoluteUrl)
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
        var baseUrl = "https://dl4.gemexit.com/";

        if (folderUrl.StartsWith(baseUrl))
        {
            folderUrl = folderUrl.Substring(baseUrl.Length);
        }

        var absoluteUrl = new Uri(new Uri(baseUrl), folderUrl).AbsoluteUri;

        var html = await httpClient.GetStringAsync(absoluteUrl);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var folderNodes = doc.DocumentNode.SelectNodes("//td[@class='link']/a");
        if (folderNodes != null)
        {
            foldersView.ItemsSource = folderNodes;
        }

        currentFolderUrl = folderUrl;
    }
}