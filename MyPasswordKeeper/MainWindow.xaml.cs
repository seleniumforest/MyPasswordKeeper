using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using MyPasswordKeeper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyPasswordKeeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string enteredPassword => this.PasswordTextBox.Text;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SaveCustomButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            var show = dialog.ShowDialog(this);
            if (show.HasValue && show.Value)
            {
                var path = dialog.InitialDirectory + dialog.FileName;
                await Helpers.TrySaveArchive(enteredPassword, mainGrid.ItemsSource.OfType<Identity>(), path);
                StatusLabel.Content = "Saved";
            }
        }

        private async void SaveDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            await Helpers.TrySaveArchive(enteredPassword, mainGrid.ItemsSource.OfType<Identity>(), UserSettings.pathToArchive);
            StatusLabel.Content = "Saved";
        }

        private async void OpenCustomButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserSettings.isArchiveExists)
            {
                var dialog = new OpenFileDialog();
                var show = dialog.ShowDialog(this);
                if (show.HasValue && show.Value)
                {
                    var result = await Helpers.TryLoadArchive(UserSettings.pathToArchive, enteredPassword);
                    if (result.success)
                    {
                        mainGrid.ItemsSource = result.identities;
                        StatusLabel.Content = "Loaded";
                    }
                    else
                    {
                        StatusLabel.Content = "Corrupted zip or incorrect password";
                    }
                }
            }
        }

        private async void OpenDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserSettings.isArchiveExists)
            {
                var result = await Helpers.TryLoadArchive(UserSettings.pathToArchive, enteredPassword);
                if (result.success)
                {
                    mainGrid.ItemsSource = result.identities;
                    StatusLabel.Content = "Loaded";
                }
                else
                {
                    StatusLabel.Content = "Corrupted zip or incorrect password";
                }
            }
            else
            {
                StatusLabel.Content = "Archive doesn't exists";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainGrid.ItemsSource = new List<Identity>();
        }
    }
}
