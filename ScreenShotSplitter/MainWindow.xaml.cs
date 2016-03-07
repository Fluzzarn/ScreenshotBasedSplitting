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
using AForge.Imaging;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;
using System.Runtime.InteropServices;

namespace ScreenShotSplitter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VideoCaptureDevice videoSource;
        FilterInfoCollection Sources;
        bool _isRunning;


        public Splits splits;
        public MainWindow()
        {
            InitializeComponent();

            splits = new Splits();
            // enumerate video devices
            Sources = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            // create video source
            foreach (FilterInfo source in Sources)
            {
                comboBox.Items.Add(source.Name);
            }
            _isRunning = false;
            splits.AddedSplit += Splits_AddedSplit;
        }

        private void Splits_AddedSplit(object sender, SplitsEventArgs e)
        {

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

                if(_isRunning)
                {
                    Split currentSplit = splits.GetCurrentSplit();

                    if(currentSplit == null)
                    {
                        Console.WriteLine("No more splits found");
                        _isRunning = false;
                        return;
                    }

                    Bitmap converted = BitmapImage2Bitmap(splits.GetCurrentSplit().SplitImage as BitmapImage);
                    
                    try
                    {
                        //compare with current split
                        ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
                        ImageSourceConverter c = new ImageSourceConverter();

                        TemplateMatch[] matchings = tm.ProcessImage((Bitmap)eventArgs.Frame.Clone(), converted);

                            Console.WriteLine(matchings[0].Similarity   );
                        if(matchings[0].Similarity >= 1.0f - splits.GetCurrentSplit().Threshold)
                        {

                            splits.GotoNextSplit();
                            Dispatcher.BeginInvoke(new ThreadStart(delegate
                            {
                                UpdateSplitText(splits.GetCurrentSplit());
                            }));
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Incorrected Image Dimensions!\nCaptured Dimensions: " + img.Width + " , " + img.Height + "\nSplit Dimensions: " + converted.Width + " , " + converted.Height, "Incorrect Dimensions!", MessageBoxButton.OK, MessageBoxImage.Error);
                        _isRunning = false;
                    }

                }
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
            NewSplitWindow win = new NewSplitWindow(splits);
            win.Show();
        }


        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                var bpp = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                using (Graphics gr = Graphics.FromImage(bpp))
                {
                    gr.DrawImage(bitmap, new System.Drawing.Rectangle(0, 0, bpp.Width, bpp.Height));
                }
                return bpp;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _isRunning = !_isRunning;
            splits.ResetSplits();
            UpdateSplitText(splits.GetCurrentSplit());
        }

        private void UpdateSplitText(Split e)
        {
            currentSplitTextblock.Text = "Current Split: " + e.SplitName;
        }
    }
}
