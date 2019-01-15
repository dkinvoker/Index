using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Szablon elementu Kontrolka użytkownika jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234236

namespace Index
{
    public sealed partial class FileControl : UserControl
    {
        public StorageFile File { get { return this.DataContext as StorageFile; } }
        public FileControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            this.LoadImageAtContentChangeAsync();
        }

        private async void MakeBigImage(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var image = await File.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem);
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(image);
            Image.Source = bitmap;
        }

        private async void MakeSmallImage(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var image = await File.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.ListView);
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(image);
            Image.Source = bitmap;
        }

        //private async void LoadImageAtContentChangeAsync()
        //{
        //    switch (Globals.PictureMode)
        //    {
        //        case PictureMode.None:
        //            Image.Source = null;
        //            Image.MaxHeight = double.PositiveInfinity;
        //            break;
        //        case PictureMode.Small:
        //            Image.MaxHeight = double.PositiveInfinity;
        //            this.DataContextChanged += MakeSmallImage;
        //            break;
        //        case PictureMode.Big:
        //            Image.MaxHeight = 200;
        //            this.DataContextChanged += MakeBigImage;
        //            break;
        //    }
        //}

        public async void LoadImageAsync(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            switch (Globals.PictureMode)
            {
                case PictureMode.None:
                    Image.Source = null;
                    Image.MaxHeight = double.PositiveInfinity;
                    break;
                case PictureMode.Small:
                    Image.MaxHeight = double.PositiveInfinity;
                    MakeSmallImage(null, null);
                    break;
                case PictureMode.Big:
                    Image.MaxHeight = 200;
                    MakeBigImage(null, null);
                    break;
            }
        }
    }
}
