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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace BingWall
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

#if ADSUPPORTED
            trialPanel.Visibility = System.Windows.Visibility.Visible;
            textBlock0.Text = "bing2wall (free)";
#endif
            
        }

         private void rateButton_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask reviewTask = new MarketplaceReviewTask();
            reviewTask.Show();
        }

        private void supportButton_Click(object sender, EventArgs e)
        {
            string emailBody = "\n\n\n\nDebugging info\nPlease leave in message\n";

            foreach (string reqDump in Utils.LastRequests)
            {
                emailBody += reqDump;
            }

            EmailComposeTask emailTask = new EmailComposeTask();
            emailTask.To = "bing2wall@live.com";
            emailTask.Body = emailBody;
            emailTask.Show();


        }

        private void buyButton_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceDetailTask detailTask = new MarketplaceDetailTask();
            detailTask.ContentIdentifier = "4ab358af-cd35-e011-854c-00237de2db9e"; // paid version's ProductID
            detailTask.Show();
        }


     }
}