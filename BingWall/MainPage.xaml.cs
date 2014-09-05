using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Net.NetworkInformation;
using System.Collections.Generic;
using System.Windows.Media;
#if ADSUPPORTED
using Microsoft.Advertising.Mobile.UI;
#endif

namespace BingWall
{
    public partial class MainPage : PhoneApplicationPage
    {
        //private bool firstInstance = false;
        private int daysAgo = 0;
        private bool loading = false;
        private bool saving = false;
        private bool currentSaved = false;
        private string displayedFileName;

        private bool warnedDataConnection = false;

        enum Mode
        {
            HomeMode,
            WorldMode,
            WorldItemMode,
        }

        private Mode mode = Mode.HomeMode;

        ApplicationBarIconButton backButton;
        ApplicationBarIconButton nextButton;
        ApplicationBarIconButton saveButton;
        ApplicationBarIconButton modeButton;

        ApplicationBar normalAppBar;
        ApplicationBar itemAppBar;

        FlyoutAnimation flyoutAnimation = new FlyoutAnimation();

        private WorldListLoader currentListLoader;
        private BitmapSource currentImageSource;
        private string currentPageTitle;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            CreateAppBar();

            this.BackKeyPress += new EventHandler<CancelEventArgs>(MainPage_BackKeyPress);

            flyoutAnimation.Attach(PageTitle);
            flyoutAnimation.Attach(worldList);
            flyoutAnimation.Attach(imagePreview);

            flyoutAnimation.Completed += new EventHandler(flyoutAnimation_Completed);
#if ADSUPPORTED
            SetupAds();
#endif               
        }

#if ADSUPPORTED
        private void SetupAds()
        {

#if DEBUG
            AdControl adControl = new AdControl("test_client", // ApplicationID
                                "Image480_80",    // AdUnitID
                                AdModel.Contextual, // AdModel
                                true);         // RotationEnabled


            AdControl.TestMode = true;
#else
                AdControl adControl = new AdControl("28600c69-dba2-41fc-8f9c-d2fb81d37000", // ApplicationID
                                    "10013904",    // AdUnitID
                                    AdModel.Contextual, // AdModel
                                    true);         // RotationEnabled
                AdControl.TestMode = false;

#endif
            adControl.Width = 480;
            adControl.Height = 80;
            adControl.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            adControl.Margin = new Thickness(0, 0, 0, 0);
            TitlePanel.Margin = new Thickness(0, 17, 0, 8);
            ContentPanel.Children.Add(adControl);

            worldList.Height = 474;
            imagePreview.Height = 474;
            PageTitle.FontSize = 48;
        }
#endif

        private void SetWorldListLoader(WorldListLoader loader)
        {
            currentListLoader = loader;
            if (!flyoutAnimation.IsRunning) worldList.ItemsSource = currentListLoader.List;
        }

        private void SetImageSource(BitmapSource source)
        {
            currentImageSource = source;
            if (!flyoutAnimation.IsRunning) imagePreview.Source = source;
        }

        private void SetPageTitle(string title)
        {
            currentPageTitle = title;
            if (!flyoutAnimation.IsRunning) PageTitle.Text = title;
        }

        void flyoutAnimation_Completed(object sender, EventArgs e)
        {
            PageTitle.Text = currentPageTitle;

            if (imagePreview.Source != currentImageSource) imagePreview.Source = currentImageSource;

            if (currentListLoader != null)
            {
                if (worldList.ItemsSource != currentListLoader.List) worldList.ItemsSource = currentListLoader.List;
            }
        }

        void MainPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            if (mode == Mode.WorldItemMode)
            {
                e.Cancel = true;
                CloseItem();
            }
        }

        private void CreateAppBar()
        {
            modeButton = new ApplicationBarIconButton(new Uri("/images/appbar.globe.rest.png", UriKind.Relative));
            modeButton.IsEnabled = true;
            modeButton.Text = "World";
            modeButton.Click += new EventHandler(modeButton_Click);

            saveButton = new ApplicationBarIconButton(new Uri("/images/appbar.save.rest.png", UriKind.Relative));
            saveButton.IsEnabled = false;
            saveButton.Text = "Save";
            saveButton.Click += new EventHandler(saveButton_Click);

            backButton = new ApplicationBarIconButton(new Uri("/images/appbar.back.rest.png", UriKind.Relative));
            backButton.IsEnabled = false;
            backButton.Text = "Previous";
            backButton.Click += new EventHandler(backButton_Click);

            nextButton = new ApplicationBarIconButton(new Uri("/images/appbar.next.rest.png", UriKind.Relative));
            nextButton.IsEnabled = false;
            nextButton.Text = "Next";
            nextButton.Click += new EventHandler(nextButton_Click);

            normalAppBar = new ApplicationBar();
            normalAppBar.IsVisible = true;

            itemAppBar = new ApplicationBar();
            itemAppBar.IsVisible = true;

            normalAppBar.Buttons.Add(modeButton);
            normalAppBar.Buttons.Add(saveButton);
            normalAppBar.Buttons.Add(backButton);
            normalAppBar.Buttons.Add(nextButton);

            itemAppBar.Buttons.Add(saveButton);

            normalAppBar.IsMenuEnabled = true;
            itemAppBar.IsMenuEnabled = true;

            ApplicationBarMenuItem settingsMenuItem = new ApplicationBarMenuItem("settings");
            settingsMenuItem.Click += new EventHandler(settingsMenuItem_Click);
            settingsMenuItem.IsEnabled = true;
            normalAppBar.MenuItems.Add(settingsMenuItem);
            itemAppBar.MenuItems.Add(settingsMenuItem);

            ApplicationBarMenuItem aboutMenuItem = new ApplicationBarMenuItem("about");
            aboutMenuItem.Click += new EventHandler(aboutMenuItem_Click);
            aboutMenuItem.IsEnabled = true;
            normalAppBar.MenuItems.Add(aboutMenuItem);
            itemAppBar.MenuItems.Add(aboutMenuItem);

#if DEBUG
            ApplicationBarMenuItem debugMenuItem = new ApplicationBarMenuItem("temp file count (dbg)");
            debugMenuItem.Click += new EventHandler(debugMenuItem_Click);
            debugMenuItem.IsEnabled = true;
            normalAppBar.MenuItems.Add(debugMenuItem);
#endif
            ApplicationBar = normalAppBar;
        }

        void modeButton_Click(object sender, EventArgs e)
        {
            if (mode == Mode.HomeMode)
            {
                SetMode(Mode.WorldMode, true);
            }
            else
            {
                SetMode(Mode.HomeMode, true);
            }
        }

        private void SetMode(Mode newMode, bool animate)
        {
            //if (mode == Mode.WorldItemMode && newMode != Mode.WorldItemMode)
            //{
            //    ApplicationBar = normalAppBar;
            //}

            mode = newMode;

            if (newMode == Mode.HomeMode)
            {
                worldList.Visibility = System.Windows.Visibility.Collapsed;
                imagePreview.Visibility = System.Windows.Visibility.Visible;
                modeButton.IconUri = new Uri("/images/appbar.globe.rest.png", UriKind.Relative);
                modeButton.Text = "World";
            }
            else if (newMode == Mode.WorldMode)
            {
                worldList.Visibility = System.Windows.Visibility.Visible;
                
                imagePreview.Visibility = System.Windows.Visibility.Collapsed;
                modeButton.IconUri = new Uri("/images/appbar.home.rest.png", UriKind.Relative);
                modeButton.Text = "Home";
            }
            else
            {
                worldList.Visibility = System.Windows.Visibility.Collapsed;
                //worldList.Opacity = 0.4;
                imagePreview.Visibility = System.Windows.Visibility.Visible;
                // ApplicationBar = itemAppBar;
            }
            LoadData();
        }

        void aboutMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.RelativeOrAbsolute));
        }

        void settingsMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }

#if DEBUG
        void debugMenuItem_Click(object sender, EventArgs e)
        {
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            string[] fileNames = myStore.GetFileNames("cache\\*");
            if (MessageBox.Show(String.Format("Cached files: {0}\nPurge?", fileNames.Length), "Temp Files", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ImageCache.ClearCachedFiles(true);
            }
        }
#endif
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetMode(mode, false);

        }

        private void LoadBingImage()
        {

            //this.imagePreview.Opacity = 0.4;
            SetImageSource(null);

            try
            {
                string filename = ImageCache.GetImage(this.daysAgo, Utils.EffectiveCulture(Settings.CultureCode), new OnEndGetImageDelegate(OnImageDownloaded));
                if (filename != null)
                {
                    DisplayImage(filename);
                }
            }
            catch (Exception ex)
            {
                DisplayImage(null);
            }

        }

        private void OnImageDownloaded(CacheImageResult result)
        {
            ShowOobe();

            if (result.daysAgo == this.daysAgo && mode != Mode.WorldMode)
            {
                if (result.status == CacheImageResultStatus.Success)
                {
                    if (mode == Mode.HomeMode)
                    {
                        Settings.LastGoodCultureCode = Settings.CultureCode;
                    }
                    DisplayImage(result.fileName);
                }
                else
                {
                    DisplayImage(null);
                    if (result.status == CacheImageResultStatus.NoContent)
                    {
                        statusLabel.Text = "no image for this day in this region";
                        if (Settings.CultureCode != Settings.LastGoodCultureCode && Settings.CultureCode != "default" && Settings.CultureCode != warnedRegion)
                        {
                            warnedRegion = Settings.CultureCode;
                            if (MessageBox.Show(String.Format("Could not load mobile Bing background for {0}.\n\nThis could be a temporary problem, but may also mean that this region/country does not support Bing mobile backgrounds.\n\nDo you want to revert to your previous region/country setting?", Settings.CultureName), "download failed", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                Settings.CultureCode = Settings.LastGoodCultureCode;
                                LoadData();
                            }
                        }
                    }
                    else if (result.status == CacheImageResultStatus.NoDataConnection)
                    {
                        statusLabel.Text = "no data connection";

                        if (!warnedDataConnection)
                        {
                            warnedDataConnection = true;
                            MessageBox.Show("Your data connection does not seem to be active.\n\nPlease check that your radio is turned on and you have a working data connection, then try again.", "no data network", MessageBoxButton.OK);

                        }
                    }
                    else
                    {
                        statusLabel.Text = "failed to download image";
                    }
                }
            }
        }

        private string loaderCacheMarket = null;
        private Dictionary<string, WorldListLoader> loaderCache = new Dictionary<string, WorldListLoader>();


        private void LoadWorldList()
        {
            if (loaderCacheMarket == null)
            {
                loaderCacheMarket = Settings.CultureCode;
            }
            if (loaderCacheMarket != Settings.CultureCode)
            {
                loaderCache.Clear();
                currentListLoader = null;
            }

            if (currentListLoader != null && currentListLoader.DaysAgo != this.daysAgo)
            {
                currentListLoader.Finished -= new FinishedEvent(currentListLoader_Finished);
                currentListLoader.Pause();
                currentListLoader = null;
            }

            if (currentListLoader == null)
            {
                string key = String.Format("{0:yyyy-MM-dd}", DateTime.Today.AddDays(-daysAgo));

                if (!loaderCache.TryGetValue(key, out currentListLoader))
                {
                    currentListLoader = new WorldListLoader(daysAgo, this, Utils.EffectiveCulture(Settings.CultureCode));
                    loaderCache.Add(key, currentListLoader);
                }
                currentListLoader.Finished += new FinishedEvent(currentListLoader_Finished);
            }

            SetWorldListLoader(currentListLoader);
            currentListLoader.Load();

            loading = false;
            progressBar1.Visibility = System.Windows.Visibility.Visible;
            flyoutAnimation.Appear();
            SetAppBarButtons();
        }

        void currentListLoader_Finished(CacheImageResultStatus status)
        {
            Dispatcher.BeginInvoke(new FinishedEvent(currentListLoader_DoFinished), status);
        }

        void currentListLoader_DoFinished(CacheImageResultStatus status)
        {
            ShowOobe();
            progressBar1.Visibility = System.Windows.Visibility.Collapsed;
            if (status == CacheImageResultStatus.NoDataConnection)
            {
                statusLabel.Text = "no data connection";
                statusLabel.Visibility = System.Windows.Visibility.Visible;

                if (!warnedDataConnection)
                {
                    warnedDataConnection = true;
                    MessageBox.Show("Your data connection does not seem to be active.\n\nPlease check that your radio is turned on and you have a working data connection, then try again.", "no data network", MessageBoxButton.OK);
                }

            }
        }

        private void ShowOobe()
        {
            if (!Settings.OobeShown)
            {
                Settings.OobeShown = true;
                string oobeMessage = "\nTry swiping left or right to change the day you are viewing!\n\n";
#if ADSUPPORTED
                oobeMessage += "Bing2Wall is ad supported. An ad-free version is avaliable for purchase: you can buy it from the about menu. (Tap ... to open the menu and select 'about'.)";
#endif
                MessageBox.Show(oobeMessage, "what's new?", MessageBoxButton.OK);
            }

        }

        private void DisplayImage(string fileName)
        {
            this.displayedFileName = null;
            lock (this.imagePreview)
            {
                if (fileName != null)
                {
                    Debug.WriteLine("Dispatched " + fileName);

                    Debug.WriteLine("Displaying " + fileName);
                    IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
                    try
                    {
                        using (Stream file = myStore.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                        {

                            BitmapImage image = new BitmapImage();
                            image.SetSource(file);

                            this.displayedFileName = fileName;
                            SetImageSource(image);
                            file.Close();
                            this.statusLabel.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                    catch (IsolatedStorageException ise)
                    {
                        SetImageSource(null);
                        this.statusLabel.Text = "error displaying image";
                        this.statusLabel.Visibility = System.Windows.Visibility.Visible;
                        Debug.WriteLine(ise.Message);
                    }
                }
                else
                {
                    SetImageSource(null);
                    this.statusLabel.Visibility = System.Windows.Visibility.Visible;
                }
                loading = false;
                currentSaved = false;
                progressBar1.Visibility = System.Windows.Visibility.Collapsed;
                SetAppBarButtons();
                //this.imagePreview.Opacity = 1;
                flyoutAnimation.Appear();
            }
        }

        private void SetAppBarButtons()
        {
            this.saveButton.IsEnabled = !this.loading && !this.saving && !this.currentSaved && this.displayedFileName != null && this.mode != Mode.WorldMode;
            this.backButton.IsEnabled = (this.daysAgo < 6) && !this.saving; // && !(this.mode == Mode.WorldItemMode);
            this.nextButton.IsEnabled = (this.daysAgo > 0) && !this.saving; // && !(this.mode == Mode.WorldItemMode);
            this.modeButton.IsEnabled = !this.saving;
        }

        private string warnedRegion;

        private void LoadData()
        {
            this.statusLabel.Visibility = System.Windows.Visibility.Collapsed;
            this.currentSaved = false;
            loading = true;
            progressBar1.Visibility = System.Windows.Visibility.Visible;
            SetDateLabel();
            SetAppBarButtons();

            if (mode == Mode.HomeMode)
            {
                LoadBingImage();
            }
            else if (mode == Mode.WorldMode)
            {
                LoadWorldList();
            }
            else
            {
                DisplayImage(displayedFileName);
            }
        }

        private void SetDateLabel()
        {
            string label;

            if (this.daysAgo == 0)
            {
                label = "today";
            }
            else if (this.daysAgo == 1)
            {
                label = "yesterday";
            }
            else
            {
                label = String.Format("{0} days ago", this.daysAgo);
            }

            SetPageTitle(label);
            if (mode == Mode.HomeMode)
            {
                this.countryLabel.Text = Settings.CultureName.ToLower();
            }
            else
            {
                this.countryLabel.Text = "world";
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (this.daysAgo > 0)
            {
                ChangeDay(true);
            }

        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (this.daysAgo < 6)
            {
                ChangeDay(false);
            }
        }

        private void ChangeDay(bool forward)
        {
            this.daysAgo += forward ? -1 : 1;

            if (mode == Mode.WorldItemMode) SetMode(Mode.WorldMode, true);

            if (forward)
            {
                flyoutAnimation.FlyoutLeft();
            }
            else
            {
                flyoutAnimation.FlyoutRight();
            }
            LoadData();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            this.imagePreview.Opacity = 0.4;
            this.statusLabel.Visibility = System.Windows.Visibility.Visible;
            this.statusLabel.Text = "saving...";
            this.progressBar1.Visibility = System.Windows.Visibility.Visible;
            this.saving = true;
            SetAppBarButtons();

            if (this.displayedFileName != null)
            {
                String fileName = this.displayedFileName;

                BackgroundWorker saveWorker = new BackgroundWorker();
                saveWorker.DoWork += new DoWorkEventHandler(saveWorker_DoWork);
                saveWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(saveWorker_RunWorkerCompleted);
                saveWorker.RunWorkerAsync(fileName);

            }
        }

        WriteableBitmap bufferBitmap = new WriteableBitmap(480, 800);

        void saveWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool success = (bool)e.Result;

            if (!success)
            {
                this.displayedFileName = null;
                MessageBox.Show("There was a problem with saving the image.\n\nIf you are currently syncing with your desktop, that is the problem since apps can't save to your photo library during sync.\n\nOtherwise please restart the application and try again!");
            }

            this.saving = false;
            this.currentSaved = true;
            SetAppBarButtons();
            this.progressBar1.Visibility = System.Windows.Visibility.Collapsed;
            if (success)
            {
                if (Settings.DontShowSaved)
                {
                    this.statusLabel.Text = "image has been saved";
                }
                else
                {
                    this.imagePreview.Opacity = 1;
                    //imagePreview.Source = bufferBitmap;
                    NavigationService.Navigate(new Uri("/SavedPage.xaml", UriKind.RelativeOrAbsolute));
                }
            }

        }

        void saveWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SaveImageToLibrary((string)e.Argument);
                e.Result = true;
            }
            catch (Exception ex)
            {
                e.Result = false;
            }
        }

        private void SaveImageToLibrary(string fileName)
        {

            string saveFileName = fileName;
/*
            if (Settings.ProcessImage)
            {
                saveFileName = Utils.SaveDitheredBitmap(fileName, bufferBitmap);
            }
*/
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            using (IsolatedStorageFileStream myFileStream = myStore.OpenFile(saveFileName, FileMode.Open, FileAccess.Read))
            {
                //Add the JPEG file to the photos library on the device.
                using (MediaLibrary library = new MediaLibrary())
                {
                    Picture pic = library.SavePicture(saveFileName.Split('\\')[1], myFileStream);
                    myFileStream.Close();
                }
            }

        }

        private void countryLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void worldList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            if (worldList.SelectedIndex != -1)
            {
                WorldListItem item = (WorldListItem)worldList.SelectedItem;
                ShowItem(item);
            }

        }

        private void ShowItem(WorldListItem item)
        {
            mode = Mode.WorldItemMode;
            //ApplicationBar = itemAppBar;

            if (currentListLoader != null) currentListLoader.Pause();

            DisplayImage(item.ThumbnailFileName);
            imagePreview.Opacity = 0.4;

            this.statusLabel.Visibility = System.Windows.Visibility.Visible;
            this.statusLabel.Text = "loading full version";
            loading = true;
            progressBar1.Visibility = System.Windows.Visibility.Visible;
            SetAppBarButtons();

            worldList.Visibility = System.Windows.Visibility.Collapsed;

            imagePreview.Visibility = System.Windows.Visibility.Visible;
            

            try
            {
                string filename = ImageCache.GetImage(this.daysAgo, item.RegionCode, new OnEndGetImageDelegate(OnImageDownloaded));
                if (filename != null)
                {
                    DisplayImage(filename);
                }
            }
            catch (Exception ex)
            {
                DisplayImage(null);
            }
        }

        private void CloseItem()
        {
            worldList.SelectedIndex = -1;
            SetMode(Mode.WorldMode, true);
        }

        private void imagePreview_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.currentSaved)
            {
                this.currentSaved = false;
                imagePreview.Opacity = 1;
                statusLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void paging_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            double pos = e.CumulativeManipulation.Translation.X;
            if (pos > 36 && daysAgo == 6) pos = 36;
            if(pos < -36 && daysAgo == 0) pos = -36;
            flyoutAnimation.SetPosition(pos);
        }

        private void paging_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (Math.Abs(e.TotalManipulation.Translation.X) > 64)
            {
                if (e.TotalManipulation.Translation.X > 0)
                {
                    if (daysAgo < 6)
                    {
                        ChangeDay(false);
                        return;
                    }
                }
                else
                {
                    if (daysAgo > 0)
                    {
                        ChangeDay(true);
                        return;
                    }
                }
            }
            flyoutAnimation.SetPosition(0);
        }


    }
}