using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media.Imaging;

namespace BingWall
{
    public partial class LocalFileImage : UserControl
    {
        public static DependencyProperty FileNameProperty = DependencyProperty.Register(
"FileName", typeof(string), typeof(LocalFileImage), new PropertyMetadata(null, OnFileNameChanged));

        private string fileName;

        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                SetThumbnail(fileName);
            }
        }

        private static void OnFileNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs a)
        {
            ((LocalFileImage)o).FileName = (string)a.NewValue;
        }

        private void SetThumbnail(string fileName)
        {

            if (fileName != null)
            {
                IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
                try
                {
                    using (Stream file = myStore.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                    {

                        BitmapImage image = new BitmapImage();
                        image.SetSource(file);
                        theImage.Source = image;
                        file.Close();

                    }
                }
                catch (IsolatedStorageException ise)
                {
                    theImage.Source = null;
                }
            }
            else
            {
                theImage.Source = null;
            }
        }


        public LocalFileImage()
        {
            InitializeComponent();
        }
    }
}
