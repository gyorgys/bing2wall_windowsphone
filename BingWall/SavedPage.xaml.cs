using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace BingWall
{
    public partial class SavedPage : PhoneApplicationPage
    {
        public SavedPage()
        {
            InitializeComponent();

            dontShowCheckBox.IsChecked = Settings.DontShowSaved;
        }

        private void dontShowCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Settings.DontShowSaved = true;
        }

        private void dontShowCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            Settings.DontShowSaved = false;
        }

    }
}