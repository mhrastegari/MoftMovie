namespace MoftMovie.Views;

public partial class SettingsView : ContentPage
{
	public SettingsView()
	{
		InitializeComponent();
        LoadSavedUrl();
    }

    private void LoadSavedUrl()
    {
        var savedUrl = Preferences.Get("FileProviderUrl", string.Empty);
        picker.SelectedItem = savedUrl;
    }

    private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedUrl = picker.SelectedItem as string;
        Preferences.Set("FileProviderUrl", selectedUrl);

        await FoldersView.Current.LoadFolders();
    }
}