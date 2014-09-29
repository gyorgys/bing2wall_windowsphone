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
using System.Diagnostics;
using Microsoft.Phone.Net.NetworkInformation;

namespace BingWall
{
    public enum CacheImageResultStatus
    {
        None,
        Success,
        DownloadFailed,
        NoContent,
        Error,
        NoDataConnection,
    }

    public class CacheImageResult
    {
        public string fileName;
        public string etag;
        public int daysAgo;
        public string market;
        public CacheImageResultStatus status;
    }

    public abstract class CacheImageRequest
    {
        public string FileName { get; protected set; }
        public int DaysAgo { get; protected set; }
        public string Market { get; protected set; }
        public bool Landscape { get; protected set; }
 
        protected HttpWebRequest imageRequest;
        protected OnEndGetImageDelegate callback;

        public abstract void Begin(OnEndGetImageDelegate callback);
        public virtual void Abort()
        {
            if (imageRequest != null)
            {
                imageRequest.Abort();
            }
        }
    }


    public class CacheFullImageRequest : CacheImageRequest
    {

        public CacheFullImageRequest(string fileName, int daysAgo, string market, bool landscape)
        {
            this.FileName = fileName;
            this.DaysAgo = daysAgo;
            this.Market = market;
            this.Landscape = landscape;
        }

        public override void Begin(OnEndGetImageDelegate callback)
        {
            if (imageRequest != null)
            {
                throw new InvalidOperationException("Begin was already called on this object.");
            }

            this.callback = callback;

            string url = Utils.GetImageUrl(DaysAgo, Market, Landscape);

            Debug.WriteLine("Requesting " + url);

            imageRequest = HttpWebRequest.CreateHttp(url);
            imageRequest.AllowAutoRedirect = true;
            
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
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        dump += String.Format("ContentType: {0}\n", response.ContentType);
                        dump += String.Format("ContentLength: {0}\n", response.ContentLength);
                        if (response.ContentType == "image/jpeg")
                        {
                            cacheResult.etag = Utils.GetEtag(response);
                            string info = Utils.GetPhotoInfo(response);
                            ImageCache.CacheImage(response.GetResponseStream(), FileName, info, true);
                            Debug.WriteLine("Saved " + FileName);
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
                ((DependencyObject)(callback.Target)).Dispatcher.BeginInvoke(callback, cacheResult);
            }
        }

    }

    public delegate void OnEndGetImageDelegate(CacheImageResult result);

}
