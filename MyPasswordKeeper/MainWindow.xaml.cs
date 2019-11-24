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

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            var show = dialog.ShowDialog(this);
            if (show.HasValue && show.Value)
            {
                var path = dialog.InitialDirectory + dialog.FileName;
                var storage = new Storage
                {
                    Password = enteredPassword,
                    PathToArchive = path
                };
                if (await storage.IsValidStorage())
                {
                    mainGrid.ItemsSource = await storage.Load();
                    StatusLabel.Content = "Loaded";
                } 
                else
                {
                    StatusLabel.Content = "Corrupted zip or incorrect password";
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            var show = dialog.ShowDialog(this);
            if (show.HasValue && show.Value)
            {
                var path = dialog.InitialDirectory + dialog.FileName;
                var storage = new Storage
                {
                    Data = mainGrid.ItemsSource.OfType<Identity>(),
                    Password = enteredPassword,
                    PathToArchive = path
                };
                await storage.Save();
                StatusLabel.Content = "Saved";
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var storage = new Storage
            //{
            //    Password = "1",
            //    PathToArchive = @"C:\Users\DevUser\Desktop\qwe.zip"
            //};

            mainGrid.ItemsSource = new List<Identity> { };
            //StatusLabel.Content = "Loaded";
        }
    }
}
