using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video.DirectShow;
using AForge.Video;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;

namespace ScreenShotSplitter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VideoCaptureDevice videoSource;
        FilterInfoCollection Sources;

        public static Splits splits;
        public MainWindow()
        {
            InitializeComponent();
            // enumerate video devices
            Sources = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            // create video source
            foreach (FilterInfo source in Sources)
            {
                comboBox.Items.Add(source.Name);
            }
            // set NewFrame event handler

            // ...


        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                System.Drawing.Image img = (Bitmap)eventArgs.Frame.Clone();

                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                bi.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    image.Source = bi;
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //Get the names moniker;
            string moniker = string.Empty;
            foreach (FilterInfo source in Sources)
            {
                if (source.Name == comboBox.SelectedItem as string)
                {
                    moniker = source.MonikerString;
                    break;
                }
            }

            //should assert or something?
            if(moniker == string.Empty)
            {
                Console.WriteLine("NO MONIKER FOUND THAT SHARED A NAME");
                return;
            }
            
            videoSource = new VideoCaptureDevice(moniker);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            // start the video source
            videoSource.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();

            }
            catch (Exception)
            {
                Console.WriteLine("Video Source not loaded");
                throw;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            NewSplitWindow win = new NewSplitWindow();
            win.Show();
        }
    }
}
