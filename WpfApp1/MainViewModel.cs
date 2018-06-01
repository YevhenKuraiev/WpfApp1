using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    class MainViewModel : INotifyPropertyChanged
    {

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public event PropertyChangedEventHandler PropertyChanged;
        public BitmapSource Img { get; set; }
        public BitmapSource Img2 { get; set; }
        private Thread thread;

        public ICommand CloseWindowCommand => new Command(() => thread.Abort());

        public MainViewModel()
        {
            thread = new Thread(SetBitmap);
            thread.Start();
        }

        private void SetBitmap()
        {
            var startupPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent?.FullName;
            while (true)
            {
                Img = GetBitmap(startupPath + "\\fsm.jpg");
                Img2 = GetBitmap(startupPath + "\\fsm2.jpg");
                Thread.Sleep(100);
            }
        }

        private string GetExeDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = System.IO.Path.GetDirectoryName(path);
            return path;
        }


        private BitmapSource GetBitmap(string path)
        {
            BitmapSource bitmapSource = null;
            using (var bmp = new Bitmap(path))
            {
                IntPtr hBitmap = bmp.GetHbitmap();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                            IntPtr.Zero,
                            Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    }
                    catch
                    {
                        // ignored
                    }
                    finally
                    {
                        DeleteObject(hBitmap);
                    }
                });

            }
            return bitmapSource;
        }

    }


}

