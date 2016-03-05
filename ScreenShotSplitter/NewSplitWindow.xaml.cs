using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using Microsoft.Win32;

namespace ScreenShotSplitter
{
    /// <summary>
    /// Interaction logic for NewSplitWindow.xaml
    /// </summary>
    public partial class NewSplitWindow : Window
    {

        private Split _currentSplit;

        public NewSplitWindow()
        {
            InitializeComponent();
        }

        private void imageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string path = imageTextBox.Text;
            _currentSplit = null;
            if(File.Exists(path))
            {
                try
                {
                    var i = System.Drawing.Image.FromFile(path);
                    MemoryStream ms = new MemoryStream();
                    i.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.EndInit();

                    image.Source = bi;
                    _currentSplit = new Split();
                    _currentSplit.SplitName = SplitNameTextBox.Text;
                    _currentSplit.SplitImage = image.Source;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }

            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == true)
            {
                imageTextBox.Text = ofd.FileName;
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveSplitButton_Click(object sender, RoutedEventArgs e)
        {
            if(_currentSplit != null)
            MainWindow.splits.AddSplit(_currentSplit);
        }
    }
}
