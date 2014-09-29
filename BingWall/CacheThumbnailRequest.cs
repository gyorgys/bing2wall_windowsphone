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
using Microsoft.Phone.Net.NetworkInformation;
using System.Diagnostics;

namespace BingWall
{
    public class CacheThumbnailRequest : CacheImageRequest
    {
        public string Etag { get; private set; }

        public CacheThumbnailRequest(int daysAgo, string market, string etag)
        {
            this.FileName = null;
            this.DaysAgo = daysAgo;
            this.Market = market;
            this.Etag = etag;
        }

        public override void Begin(OnEndGetImageDelegate callback)
        {
            if (imageRequest != null)
            {
                throw new InvalidOperationException("Begin was already called on this object.");
            }

            this.callback = callback;

            string url = Utils.GetThumbnailUrl(DaysAgo, Market);

            Debug.WriteLine("Requesting " + url);

            imageRequest = HttpWebRequest.CreateHttp(url);
            imageRequest.AllowAutoRedirect = true;
            if (Etag != null)
            {
                imageRequest.Headers["If-None-Match"] = Etag;
            }

            imageRequest.BeginGetResponse(new AsyncCallback(ImageRequestCallback), this);

        }

        private void ImageRequestCallback(IAsyncResult result)
        {
            CacheImageResult cacheResult = new CacheImageResult();
            cacheResult.fileName = FileName;
            cacheResult.daysAgo = DaysAgo;
            cacheResult.market = Market;
            cacheResult.status = CacheImageResultStatus.None;

            string dump = String.Format("---------\nTime:{0}\nUTC:{1}\n", DateTime.Now.ToString(), DateTime.Now.ToUniversalTime().ToString());
            dump += String.Format("Days ago: {0} Market: {1}\n", DaysAgo, Market);

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)imageRequest.EndGetResponse(result))
                {
                    imageRequest = null;
                    Debug.WriteLine("End Request: " + response.StatusCode.ToString());
                    dump += String.Format("Status: {0}\n", response.StatusDescription);

                    if (response.StatusCode == HttpStatusCode.NotModified)
                    {
                        cacheResult.fileName = FileName = "cache\\" + Etag + ".jpg";
                        cacheResult.etag = Etag;
                        cacheResult.status = CacheImageResultStatus.Success;
                    }
                    else if (response.StatusCode == HttpStatusCode.OK)
                    {
                        cacheResult.etag = Utils.GetEtag(response);
                        cacheResult.fileName = FileName = "cache\\" + cacheResult.etag + ".jpg";

                        dump += String.Format("ContentType: {0}\n", response.ContentType);
                        dump += String.Format("ContentLength: {0}\n", response.ContentLength);
                        if (response.ContentType == "image/jpeg")
                        {
                            ImageCache.CacheImage(response.GetResponseStream(), FileName, null, false);
                            
                            dump += String.Format("Saved to: {0}\n", FileName);
                            cacheResult.status = CacheImageResultStatus.Success;
                        }
                        else
                        {
                            cacheResult.status = CacheImageResultStatus.NoContent;
                        }
                    }
                    else
                    {
                        cacheResult.status = CacheImageResultStatus.DownloadFailed;
                    }
                }
            }
            catch (WebException wex)
            {
                if (NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.None)
                {
                    cacheResult.status = CacheImageResultStatus.NoDataConnection;
                    dump += "No data connection\n";
                }
                else
                {
                    dump += String.Format("WebException: {0}\nNetworkInterface: {1}\n", wex.Status, NetworkInterface.NetworkInterfaceType.ToString());
                    cacheResult.status = CacheImageResultStatus.DownloadFailed;
                }
            }
            catch (Exception ex)
            {
                dump += String.Format("Exception: {0}\n", ex.Message);
                cacheResult.status = CacheImageResultStatus.Error;
            }
            finally
            {
                Utils.AddDump(dump);
                //((DependencyObject)(callback.Target)).Dispatcher.BeginInvoke(callback, cacheResult);
                callback(cacheResult);
            }
        }

    }
}
