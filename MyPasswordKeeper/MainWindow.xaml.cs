using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
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

        private IDataStorage dataStorage = new LocalStorage();

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


            var result = await dataStorage.Load(enteredPassword);
            if (result.success)
            {
                mainGrid.ItemsSource = result.identities;
                StatusLabel.Content = Labels.Successfully_loaded;
            }
            else
            {
                StatusLabel.Content = result.message;
            }
        }

        private async void SaveDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(enteredPassword))
            {
                StatusLabel.Content = Labels.NoPasswordProvided;
                return;
            }

            var archive = Helpers.CreateArchive(enteredPassword, mainGrid.ItemsSource.OfType<Identity>());
            var result = await dataStorage.Upload(archive);
            if (result.success)
                StatusLabel.Content = Labels.Saved;
            else
                StatusLabel.Content = result.message;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainGrid.ItemsSource = default(List<Identity>);
        }
    }
}
