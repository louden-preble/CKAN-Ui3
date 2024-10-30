using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CKAN_Ui3
{
    public sealed partial class MainWindow : Window
    {
        private readonly CkanMetadataService metadataService = new CkanMetadataService();
        private ModInstaller modInstaller;
        private List<ModModel> mods = new List<ModModel>();

        public MainWindow()
        {
            this.InitializeComponent();
            this.Activated += MainWindow_Activated;
        }

        private async void MainWindow_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == WindowActivationState.CodeActivated)
            {
                LoadingProgressBar.Visibility = Visibility.Visible; // Show loading bar
                mods = await metadataService.LoadCkanMetadataAsync();
                ModListView.ItemsSource = mods;
                modInstaller = new ModInstaller("C:\\Path\\To\\GameDirectory"); // Set the correct path
                LoadingProgressBar.Visibility = Visibility.Collapsed; // Hide loading bar
            }
        }

        private void ModListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModListView.SelectedItem is ModModel selectedMod)
            {
                ModNameText.Text = selectedMod.Name;
                ModAuthorText.Text = $"Author: {selectedMod.Author}";
                ModDescriptionText.Text = selectedMod.Description;
                ModCompatibilityText.Text = $"Compatible with: {selectedMod.Compatibility}";
            }
        }

        private async void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModListView.SelectedItem is ModModel selectedMod)
            {
                await modInstaller.InstallModAsync(selectedMod);
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Installation Successful",
                    Content = $"{selectedMod.Name} has been installed successfully.",
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModListView.SelectedItem is ModModel selectedMod)
            {
                modInstaller.UninstallMod(selectedMod);
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Uninstallation Successful",
                    Content = $"{selectedMod.Name} has been uninstalled successfully.",
                    CloseButtonText = "OK"
                };
                dialog.ShowAsync();
            }
        }
    }
}
