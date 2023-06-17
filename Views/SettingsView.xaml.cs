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

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var selectedRadioButton = sender as RadioButton;

        if (selectedRadioButton.IsChecked)
        {
            switch (selectedRadioButton.Content.ToString())
            {
                case "Dark":
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    break;
                case "Light":
                    Application.Current.UserAppTheme = AppTheme.Light;
                    break;
                case "System":
                    Application.Current.UserAppTheme = AppTheme.Unspecified;
                    break;
            }
        }
    }
}