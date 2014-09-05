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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BingWall
{

    public delegate void FinishedEvent(CacheImageResultStatus status);

    public class WorldListLoader
    {
        private int daysAgo;
        public int DaysAgo { get { return daysAgo; } }

        private FrameworkElement boundElement;
        private string[] markets;
        private int marketIndex;
        private string etag;
        private bool isPaused = false;
        private int highestImageCount = 0;

        private WorldList theList;

        public event FinishedEvent Finished;

        public ObservableCollection<WorldListItem> List
        {
            get
            {
                return theList;
            }
        }

        public WorldListLoader(int daysAgo, FrameworkElement boundElement, string homeMarket)
        {
            theList = new WorldList();
            this.daysAgo = daysAgo;
            this.boundElement = boundElement;


            List<string> marketList = new List<string>();
            marketList.Add(homeMarket);
            if (homeMarket != "en-US")
            {
                marketList.Add("en-US");
            }

            foreach (RegionItem ri in RegionItem.Regions)
            {
                if (ri.IsTested && ri.LanguageCode != homeMarket && ri.LanguageCode != "en-US" && ri.LanguageCode != "default")
                {
                    marketList.Add(ri.LanguageCode);
                }
            }

            markets = marketList.ToArray();
            marketIndex = 0;
            etag = null;

        }

        public void Load()
        {
            isPaused = false;
            GetNextMarket();
        }

        public void Pause()
        {
            isPaused = true;
        }

        private void GetNextMarket()
        {
            if (marketIndex >= markets.Length)
            {
                if (this.Finished != null) this.Finished(CacheImageResultStatus.Success);
                return;
            }

            if (isPaused) return;

            string market = markets[marketIndex];
            try
            {
                string fileName = ImageCache.GetThumbnail(daysAgo, market, etag, OnEndGetThumbnail);

                if (fileName != null)
                {
                    marketIndex++;
                    AddThumbnail(fileName, market, etag);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERR: Exception when requesting thumbnail");
            }
        }

        private void AddThumbnail(string fileName, string market, string etag)
        {
            int count = 1;
            WorldListItem existingItem = theList.FindItem(etag);

            if(existingItem == null)
            {

                WorldListItem newItem = new WorldListItem();
                newItem.ETag = etag;
                newItem.Text = RegionItem.GetRegion(market).ShortName;
                newItem.ThumbnailFileName = fileName;
                newItem.RegionCount = 1;
                newItem.RegionCode = market;

                boundElement.Dispatcher.BeginInvoke(new AddWorldListItemeDelegate(AddWorldListItem), newItem);
            }
            else
            {
                count = existingItem.RegionCount + 1;
                boundElement.Dispatcher.BeginInvoke(new MergeWorldListItemDelegate(MergeWorldListItem), RegionItem.GetRegion(market).ShortName, existingItem);
            }

            if(count > highestImageCount)
            {
                highestImageCount = count;
                this.etag = etag;
            }

            GetNextMarket();
        }

        public delegate void AddWorldListItemeDelegate(WorldListItem item);

        private void AddWorldListItem(WorldListItem item)
        {
            theList.Add(item);
        }

        public delegate void MergeWorldListItemDelegate(string regionName, WorldListItem existingItem);

        private static void MergeWorldListItem(string regionName, WorldListItem existingItem)
        {
            existingItem.RegionCount++;
            if (existingItem.RegionCount <= 4)
            {
                existingItem.Text += "\n" + regionName;
            }
            else
            {
                if (existingItem.RegionCount == 5)
                {
                    existingItem.SubText = "and 1 more region";
                }
                else
                {
                    existingItem.SubText = String.Format("and {0} more regions", existingItem.RegionCount - 4);
                }
            }
        }

        private void OnEndGetThumbnail(CacheImageResult result)
        {
            if (result.status == CacheImageResultStatus.Success)
            {
                marketIndex++;
                AddThumbnail(result.fileName, result.market, result.etag);
            }
            else if (result.status == CacheImageResultStatus.NoContent)
            {
                marketIndex++;
                GetNextMarket();
            }
            else
            {
                if (this.Finished != null) this.Finished(result.status);
            }
        }
    }
    
}
