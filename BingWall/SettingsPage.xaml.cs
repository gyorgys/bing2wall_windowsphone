using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System;

namespace BingWall
{
    public partial class SettingsPage : PhoneApplicationPage
    {

        const string filterOnText = "The country/region list is filtered to those with known mobile Bing background support.";
        const string filterOnButtonText = "Show all countries.";
        const string filterOffText = "Some countries/regions did not support mobile Bing backgrounds at the at the time of publishing the application.";
        const string filterOffButtonText = "Show only countries with known support.";

        private string originalRegion;

        public SettingsPage()
        {
            InitializeComponent();

            DataContext = Model;

            SetFilterText(false);
            originalRegion = Settings.CultureCode;
            // processCheckBox.IsChecked = Settings.ProcessImage;
            liveTileCheckBox.IsChecked = Settings.LiveTile;

            this.BackKeyPress += new System.EventHandler<System.ComponentModel.CancelEventArgs>(SettingsPage_BackKeyPress);

/*
#if DEBUG
            CheckBox bit16Checkbox = new CheckBox();
            bit16Checkbox.Content = "resample to 16 bits (dbg)";
            bit16Checkbox.IsChecked = Settings.ResampleTo16Bit;
            ContentPanel.Children.Add(bit16Checkbox);

            bit16Checkbox.Checked += new RoutedEventHandler(bit16Checkbox_Checked);
            bit16Checkbox.Unchecked += new RoutedEventHandler(bit16Checkbox_Unchecked);
#endif
 */
        }
/*
#if DEBUG
        void bit16Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.ResampleTo16Bit = false;
        }

        void bit16Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.ResampleTo16Bit = true;
        }
#endif
        */

        void SettingsPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (regionPopup.IsOpen)
            {
                ClosePopup();
                
                e.Cancel = true;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SettingsPageModel.Instance.EnsureLoadData();

            regionSelector.SelectedItem = Model.SelectedItem;
        }

        protected SettingsPageModel Model
        {
            get { return SettingsPageModel.Instance; }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (originalRegion != Settings.CultureCode)
            {
                ImageCache.ClearCachedFiles(true);
                originalRegion = Settings.CultureCode;
            }
        }

        private void SetFilterText(bool smoothResize)
        {
            if (Model.FilterRegions)
            {
                //regionFilterText.Height = 54;
                regionFilterText.Text = filterOnText;
                regionFilterToggleButton.Content = filterOnButtonText;
            }
            else
            {
                //regionFilterText.Height = 80;
                regionFilterText.Text = filterOffText;
                regionFilterToggleButton.Content = filterOffButtonText;
            }
       
        }

        private void regionFilterToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Model.FilterRegions = !Model.FilterRegions;
            if (!Model.Regions.Contains(Model.SelectedItem)) Model.SelectedItem = Model.Regions[0];
            regionSelector.SelectedItem = Model.SelectedItem;
            SetFilterText(true);
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //SystemTray.IsVisible = false;
            regionSelector.SelectionChanged += new SelectionChangedEventHandler(regionSelector_SelectionChanged);
            regionSelector.Loaded += new RoutedEventHandler(regionSelector_Loaded);
            regionPopup.IsOpen = true;
        }

        void regionSelector_Loaded(object sender, RoutedEventArgs e)
        {
            regionSelector.ScrollIntoView(regionSelector.SelectedItem);
        }

        private void regionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count != 0)
            {
                Model.SelectedItem = (RegionItem)regionSelector.SelectedItem;
                ClosePopup();
            }
        }

        private void ClosePopup()
        {
            regionSelector.SelectionChanged -= new SelectionChangedEventHandler(regionSelector_SelectionChanged);
            regionSelector.Loaded -= new RoutedEventHandler(regionSelector_Loaded);
            //SystemTray.IsVisible = true;
            regionPopup.IsOpen = false;
        }

        private void liveTileCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Utils.EnableLiveTile();
        }

        private void liveTileCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Utils.DisableLiveTile();
        }

        /*
        private void liveTile_Checked(object sender, RoutedEventArgs e)
        {
            ShellTileSchedule mySchedule;

            mySchedule = new ShellTileSchedule();

            // Run the schedule every day for seven days, unless StopACurrentSchedule is called.
            DateTime today = DateTime.Now.Date;

            mySchedule.StartTime = today;
            mySchedule.Interval = UpdateInterval.EveryDay;
            mySchedule.Recurrence = UpdateRecurrence.Interval;
            mySchedule.RemoteImageUri = new Uri(@"http://appserver.m.bing.net/BackgroundImageService/TodayImageService.svc/GetTodayImage?dateOffset=0&urlEncodeHeaders=true&osName=wince&osVersion=7.0&orientation=240x240&deviceName=WP7Device&mkt=en-us");

            mySchedule.Start();

            Settings.LiveTile = true;
        }

        private void liveTile_Unchecked(object sender, RoutedEventArgs e)
        {
            ShellTileSchedule mySchedule;

            mySchedule = new ShellTileSchedule();
            mySchedule.Stop();

            Settings.LiveTile = false;

        }
         */
/*
        private void processCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.ProcessImage = true;
        }

        private void processCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.ProcessImage = false;
        }
*/
    }
}