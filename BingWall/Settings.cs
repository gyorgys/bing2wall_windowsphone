using System;
using System.Globalization;
using System.IO.IsolatedStorage;

namespace BingWall
{
    public static class Settings
    {
        private static bool initialized = false;

        private static string cultureCode;
        private static string lastGoodCultureCode;
        private static bool oobeShown;
        private static bool regionFilter;
        //private static bool processImage;
        private static string cultureName;
        private static bool dontShowSaved;
        private static bool liveTile;

#if DEBUG
        //private static bool resampleTo16Bit;
#endif

        public static string CultureName
        {
            get { InitIfNeeded(); return Settings.cultureName; }
        }

        public static bool RegionFilter
        {
            get
            {
                InitIfNeeded();
                return Settings.regionFilter;
            }
            set
            {
                Settings.regionFilter = value;
                IsolatedStorageSettings.ApplicationSettings["regionFilter"] = regionFilter;
            }
        }

        public static bool DontShowSaved
        {
            get
            {
                InitIfNeeded();
                return Settings.dontShowSaved;
            }
            set
            {
                Settings.dontShowSaved = value;
                IsolatedStorageSettings.ApplicationSettings["dontShowSaved"] = dontShowSaved;
            }
        }

        public static bool LiveTile
        {
            get
            {
                InitIfNeeded();
                return Settings.liveTile;
            }
            set
            {
                Settings.liveTile = value;
                IsolatedStorageSettings.ApplicationSettings["liveTile"] = liveTile;
            }
        }

/*
        public static bool ProcessImage
        {
            get
            {
                InitIfNeeded();
                return Settings.processImage;
            }
            set
            {
                Settings.processImage = value;
                IsolatedStorageSettings.ApplicationSettings["processImage"] = processImage;
            }
        }

#if DEBUG
        public static bool ResampleTo16Bit
        {
            get
            {
                InitIfNeeded();
                return Settings.resampleTo16Bit;
            }
            set
            {
                Settings.resampleTo16Bit = value;
                IsolatedStorageSettings.ApplicationSettings["resampleTo16Bit"] = resampleTo16Bit;
            }
        }

#endif
*/
        public static string CultureCode
        {
            get
            {
                InitIfNeeded();
                return cultureCode;
            }
            set
            {
                cultureCode = value;
                IsolatedStorageSettings.ApplicationSettings["cultureCode"] = cultureCode;

                CalcCultureName();
            }
        }

        private static void CalcCultureName()
        {
            string culture = cultureCode == "default" ? CultureInfo.CurrentCulture.Name : cultureCode;
            try
            {
                RegionItem ri = RegionItem.GetRegion(culture);
                cultureName = ri.ShortName;
            }
            catch (Exception)
            {
                cultureName = culture;
            }
        }

        public static string LastGoodCultureCode
        {
            get
            {
                InitIfNeeded();
                return lastGoodCultureCode;
            }
            set
            {
                // we change this frequently, so check if new value really needs to be stored?
                if (lastGoodCultureCode != value)
                {
                    lastGoodCultureCode = value;
                    IsolatedStorageSettings.ApplicationSettings["lastGoodCultureCode"] = lastGoodCultureCode;
                }
            }
        }


        public static bool OobeShown
        {
            get
            {
                InitIfNeeded();
                return oobeShown;
            }
            set
            {
                oobeShown = value;
                IsolatedStorageSettings.ApplicationSettings["oobeShown4"] = oobeShown;
            }
        }

        private static void InitIfNeeded()
        {
            if (!initialized)
            {
                try
                {
                    cultureCode = (string)IsolatedStorageSettings.ApplicationSettings["cultureCode"];
                    CalcCultureName();
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    CultureCode = "default";
                }

                try
                {
                    lastGoodCultureCode = (string)IsolatedStorageSettings.ApplicationSettings["lastGoodCultureCode"];
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    LastGoodCultureCode = "default";
                }

                try
                {
                    oobeShown = (bool)IsolatedStorageSettings.ApplicationSettings["oobeShown4"];
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    OobeShown = false;
                }

                try
                {
                    dontShowSaved = (bool)IsolatedStorageSettings.ApplicationSettings["dontShowSaved"];
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    DontShowSaved = false;
                }

                try
                {
                    liveTile = (bool)IsolatedStorageSettings.ApplicationSettings["liveTile"];
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    liveTile = false;
                }
               
                try
                {
                    regionFilter = (bool)IsolatedStorageSettings.ApplicationSettings["regionFilter"];
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    RegionFilter = true;
                }
/*
                try
                {
                    processImage = (bool)IsolatedStorageSettings.ApplicationSettings["processImage"];
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    ProcessImage = false;
                }

#if DEBUG
                try
                {
                    resampleTo16Bit = (bool)IsolatedStorageSettings.ApplicationSettings["resampleTo16Bit"];
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    ResampleTo16Bit = false;
                }
#endif
*/
                initialized = true;
            }
        }

    }
}
