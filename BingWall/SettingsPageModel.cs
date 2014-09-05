using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace BingWall
{
    public class RegionItem : INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get { return name; }
            set 
            { 
                name = value;
                shortName = value;

                int pos = shortName.IndexOf(" (");
                if (pos >= 0)
                {
                    shortName = shortName.Substring(0, pos);
                }

                NotifyPropertyChanged("Name"); 
                NotifyPropertyChanged("ShortName"); 
            }
        }

        private string shortName;

        public string ShortName
        {
            get 
            { 
                return shortName; 
            }
        }

        private string languageCode;

        public string LanguageCode
        {
            get { return languageCode; }
            set { languageCode = value; NotifyPropertyChanged("LanguageCode"); }
        }

        private bool isTested;

        public bool IsTested
        {
            get { return isTested; }
            set { isTested = value; NotifyPropertyChanged("IsTested"); }
        }

        public RegionItem(string n, string c, bool t)
        {
            Name = n;
            languageCode = c;
            isTested = t;
        }

        public static RegionItem GetRegion(string code)
        {
            foreach (RegionItem ri in regionList)
            {
                if (SameRegion(code, ri.languageCode))
                {
                    return ri;
                }
            }
            return null;
        }

        private static bool SameRegion(string code1, string code2)
        {
            int pos;
            pos = code1.LastIndexOf('-');
            if (pos >= 0) code1 = code1.Substring(pos + 1);
            pos = code2.LastIndexOf('-');
            if (pos >= 0) code2 = code2.Substring(pos + 1);

            return (code1.ToUpper() == code2.ToUpper());

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

        public static RegionItem[] Regions
        {
            get
            {
                return regionList;
            }

        }

        #region Region List Constant

        private static RegionItem[] regionList = {
new RegionItem( "use phone language", "default", true ) ,
new RegionItem( "Afghanistan", "ps-AF", false ) ,
new RegionItem( "Albania", "sq-AL", false ) ,
new RegionItem( "Algeria", "ar-DZ", false ) ,
new RegionItem( "Argentina", "es-AR", true ) ,
new RegionItem( "Armenia", "hy-AM", false ) ,
new RegionItem( "Australia", "en-AU", true ) ,
new RegionItem( "Austria", "de-AT", true ) ,
new RegionItem( "Azerbaijan", "az-Cyrl-AZ", false ) ,
new RegionItem( "Bahrain", "ar-BH", false ) ,
new RegionItem( "Bangladesh", "bn-BD", false ) ,
new RegionItem( "Belarus", "be-BY", false ) ,
new RegionItem( "Belgium", "nl-BE", true ) ,
new RegionItem( "Belize", "en-BZ", false ) ,
new RegionItem( "Bolivia", "es-BO", false ) ,
new RegionItem( "Bosnia and Herzegovina", "sr-Cyrl-BA", false ) ,
new RegionItem( "Brazil", "pt-BR", true ) ,
new RegionItem( "Brunei Darussalam", "ms-BN", false ) ,
new RegionItem( "Bulgaria", "bg-BG", true ) ,
new RegionItem( "Cambodia", "km-KH", false ) ,
new RegionItem( "Canada", "en-CA", true ) ,
new RegionItem( "Caribbean", "en-029", false ) ,
new RegionItem( "Chile", "es-CL", true ) ,
new RegionItem( "China (P.R.C.)", "zh-CN", true ) ,
new RegionItem( "Colombia", "es-CO", false ) ,
new RegionItem( "Costa Rica", "es-CR", false ) ,
new RegionItem( "Croatia", "hr-HR", true ) ,
new RegionItem( "Czech Republic", "cs-CZ", true ) ,
new RegionItem( "Denmark", "da-DK", true ) ,
new RegionItem( "Dominican Republic", "es-DO", false ) ,
new RegionItem( "Ecuador", "es-EC", false ) ,
new RegionItem( "Egypt", "ar-EG", false ) ,
new RegionItem( "El Salvador", "es-SV", false ) ,
new RegionItem( "Estonia", "et-EE", true ) ,
new RegionItem( "Ethiopia", "am-ET", false ) ,
new RegionItem( "Faroe Islands", "fo-FO", false ) ,
new RegionItem( "Finland", "fi-FI", true ) ,
new RegionItem( "France", "fr-FR", true ) ,
new RegionItem( "Georgia", "ka-GE", false ) ,
new RegionItem( "Germany", "de-DE", true ) ,
new RegionItem( "Greece", "el-GR", true ) ,
new RegionItem( "Greenland", "kl-GL", false ) ,
new RegionItem( "Guatemala", "es-GT", false ) ,
new RegionItem( "Honduras", "es-HN", false ) ,
new RegionItem( "Hong Kong (S.A.R.)", "zh-HK", true ) ,
new RegionItem( "Hungary", "hu-HU", true ) ,
new RegionItem( "Iceland", "is-IS", false ) ,
new RegionItem( "India", "hi-IN", false ) ,
new RegionItem( "Indonesia", "id-ID", false ) ,
new RegionItem( "Iran", "fa-IR", false ) ,
new RegionItem( "Iraq", "ar-IQ", false ) ,
new RegionItem( "Ireland", "en-IE", true ) ,
new RegionItem( "Israel", "he-IL", true ) ,
new RegionItem( "Italy", "it-IT", true ) ,
new RegionItem( "Jamaica", "en-JM", false ) ,
new RegionItem( "Japan", "ja-JP", true ) ,
new RegionItem( "Jordan", "ar-JO", false ) ,
new RegionItem( "Kazakhstan", "kk-KZ", false ) ,
new RegionItem( "Kenya", "sw-KE", false ) ,
new RegionItem( "Korea", "ko-KR", false ) ,
new RegionItem( "Kuwait", "ar-KW", false ) ,
new RegionItem( "Kyrgyzstan", "ky-KG", false ) ,
new RegionItem( "Lao P.D.R.", "lo-LA", false ) ,
new RegionItem( "Latvia", "lv-LV", true ) ,
new RegionItem( "Lebanon", "ar-LB", false ) ,
new RegionItem( "Libya", "ar-LY", false ) ,
new RegionItem( "Liechtenstein", "de-LI", false ) ,
new RegionItem( "Lithuania", "lt-LT", true ) ,
new RegionItem( "Luxembourg", "de-LU", false ) ,
new RegionItem( "Macao (S.A.R.)", "zh-MO", false ) ,
new RegionItem( "Macedonia (FYROM)", "mk-MK", false ) ,
new RegionItem( "Malaysia", "ms-MY", false ) ,
new RegionItem( "Maldives", "dv-MV", false ) ,
new RegionItem( "Malta", "mt-MT", false ) ,
new RegionItem( "Mexico", "es-MX", true ) ,
new RegionItem( "Monaco", "fr-MC", false ) ,
new RegionItem( "Mongolia", "mn-MN", false ) ,
new RegionItem( "Morocco", "ar-MA", false ) ,
new RegionItem( "Nepal", "ne-NP", false ) ,
new RegionItem( "Netherlands", "nl-NL", true ) ,
new RegionItem( "New Zealand", "en-NZ", true ) ,
new RegionItem( "Nicaragua", "es-NI", false ) ,
new RegionItem( "Nigeria", "yo-NG", false ) ,
new RegionItem( "Norway", "nb-NO", true ) ,
new RegionItem( "Oman", "ar-OM", false ) ,
new RegionItem( "Pakistan", "ur-PK", false ) ,
new RegionItem( "Panama", "es-PA", false ) ,
new RegionItem( "Paraguay", "es-PY", false ) ,
new RegionItem( "Peru", "es-PE", false ) ,
new RegionItem( "Philippines", "fil-PH", false ) ,
new RegionItem( "Poland", "pl-PL", true ) ,
new RegionItem( "Portugal", "pt-PT", true ) ,
new RegionItem( "Puerto Rico", "es-PR", false ) ,
new RegionItem( "Qatar", "ar-QA", false ) ,
new RegionItem( "Romania", "ro-RO", true ) ,
new RegionItem( "Russia", "ru-RU", true ) ,
new RegionItem( "Rwanda", "rw-RW", false ) ,
new RegionItem( "Saudi Arabia", "ar-SA", false ) ,
new RegionItem( "Senegal", "wo-SN", false ) ,
new RegionItem( "Serbia", "sr-Cyrl-RS", false ) ,
new RegionItem( "Singapore", "zh-SG", false ) ,
new RegionItem( "Slovakia", "sk-SK", true ) ,
new RegionItem( "Slovenia", "sl-SI", true ) ,
new RegionItem( "South Africa", "af-ZA", false ) ,
new RegionItem( "Spain", "es-ES", true ) ,
new RegionItem( "Sri Lanka", "si-LK", false ) ,
new RegionItem( "Sweden", "sv-SE", true ) ,
new RegionItem( "Switzerland", "de-CH", true ) ,
new RegionItem( "Syria", "ar-SY", false ) ,
new RegionItem( "Taiwan", "zh-TW", true ) ,
new RegionItem( "Tajikistan", "tg-Cyrl-TJ", false ) ,
new RegionItem( "Thailand", "th-TH", true ) ,
new RegionItem( "Trinidad and Tobago", "en-TT", false ) ,
new RegionItem( "Tunisia", "ar-TN", false ) ,
new RegionItem( "Turkey", "tr-TR", true ) ,
new RegionItem( "Turkmenistan", "tk-TM", false ) ,
new RegionItem( "U.A.E.", "ar-AE", false ) ,
new RegionItem( "Ukraine", "uk-UA", false ) ,
new RegionItem( "United Kingdom", "en-GB", true ) ,
new RegionItem( "United States", "en-US", true ) ,
new RegionItem( "Uruguay", "es-UY", false ) ,
new RegionItem( "Uzbekistan", "uz-Cyrl-UZ", false ) ,
new RegionItem( "Venezuela", "es-VE", false ) ,
new RegionItem( "Vietnam", "vi-VN", false ) ,
new RegionItem( "Yemen", "ar-YE", false ) ,
new RegionItem( "Zimbabwe", "en-ZW", false )

                                       };

        #endregion

    }

    public class SettingsPageModel : INotifyPropertyChanged
    {
        private static SettingsPageModel _instance;

        public static SettingsPageModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingsPageModel();
                }
                return _instance;
            }
        }

        public bool IsLoaded { get; private set; }

        public bool FilterRegions
        {
            get
            {
                return Settings.RegionFilter;
            }
            set
            {
                Settings.RegionFilter = value;
                //selectedCode = null;
                NotifyPropertyChanged("FilterRegions");
                NotifyPropertyChanged("Regions");
            }
        }

        public ObservableCollection<RegionItem> Regions
        {
            get
            {
                if (!FilterRegions) return allRegions;
                return testedRegions;
            }
        }

        private ObservableCollection<RegionItem> allRegions;
        private ObservableCollection<RegionItem> testedRegions;

        private string cachedRegionCode;
        private RegionItem cachedRegionItem;

        public RegionItem SelectedItem
        {
            get
            {
                if (Settings.CultureCode != cachedRegionCode)
                {
                    cachedRegionCode = Settings.CultureCode;
                    cachedRegionItem = RegionItem.GetRegion(Settings.CultureCode);
                }
                return cachedRegionItem;
            }
            set
            {
                Settings.CultureCode = value.LanguageCode;
                NotifyPropertyChanged("SelectedItem");
            }
        }

        public SettingsPageModel()
        {
            IsLoaded = false;
            allRegions = new ObservableCollection<RegionItem>();
            testedRegions = new ObservableCollection<RegionItem>();
        }

        public void EnsureLoadData()
        {
            if (!IsLoaded)
            {
                foreach(RegionItem ri in RegionItem.Regions)
                {
                    this.allRegions.Add(ri);
                    if (ri.IsTested) this.testedRegions.Add(ri);
                }

                IsLoaded = true;
                Debug.WriteLine("Regions loaded");
                NotifyPropertyChanged("Regions");
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
}
