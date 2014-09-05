using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace BingWall
{
    public class WorldListItem : INotifyPropertyChanged
    {
        private string thumbnailFileName;
        public string ThumbnailFileName
        {
            get
            {
                return thumbnailFileName;
            }
            set
            {
                thumbnailFileName = value;
                NotifyPropertyChanged("ThumbnailFileName");
            }
        }

        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                NotifyPropertyChanged("Text");
            }
        }

        private string etag;
        public string ETag
        {
            get
            {
                return etag;
            }
            set
            {
                etag = value;
                NotifyPropertyChanged("ETag");
            }
        }

        private string subText;
        public string SubText
        {
            get
            {
                return subText;
            }
            set
            {
                subText = value;
                NotifyPropertyChanged("SubText");
            }
        }

        private string regionCode;
        public string RegionCode
        {
            get
            {
                return regionCode;
            }
            set
            {
                regionCode = value;
                NotifyPropertyChanged("RegionCode");
            }
        }

        private int regionCount;
        public int RegionCount
        {
            get
            {
                return regionCount;
            }
            set
            {
                regionCount = value;
                NotifyPropertyChanged("RegionCount");
            }
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class WorldList : ObservableCollection<WorldListItem>
    {
        public WorldListItem FindItem(string eTag)
        {
            foreach (WorldListItem item in this.Items)
            {
                if (item.ETag == eTag)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
