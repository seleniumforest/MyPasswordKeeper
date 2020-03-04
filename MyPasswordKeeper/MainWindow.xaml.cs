using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using MyPasswordKeeper.ArchiveWorker;
using MyPasswordKeeper.DataStorage;
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
        private string enteredPassword => this.PasswordTextBox.Password;
        private IEnumerable<Identity> enteredEntities => mainGrid.ItemsSource.OfType<Identity>();

        private IDataStorage dataStorage = new LocalStorage();
        private IArchiveWorker archiveWorker = new ZipArchiveWorker();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OpenDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(enteredPassword))
            {
                StatusLabel.Content = Labels.NoPasswordProvided;
                return;
            }

            try
            {
                var result = await dataStorage.Load(enteredPassword);
                mainGrid.ItemsSource = result;
                StatusLabel.Content = Labels.Successfully_loaded;
            }
            catch (Exception)
            {
                StatusLabel.Content = Labels.CantOpenArchive;
            }
        }

        private async void SaveDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(enteredPassword))
            {
                StatusLabel.Content = Labels.NoPasswordProvided;
                return;
            }

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Overwrite Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {

                try
                {
                    var archive = await archiveWorker.CreateArchive(enteredPassword, enteredEntities);
                    var success = await dataStorage.Upload(archive);
                    if (success)
                        StatusLabel.Content = Labels.Saved;
                    else
                        StatusLabel.Content = Labels.CantSaveArchive;
                }
                catch (Exception ex)
                {
                    StatusLabel.Content = Labels.CantSaveArchive;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainGrid.ItemsSource = new List<Identity>();
        }
    }
}
