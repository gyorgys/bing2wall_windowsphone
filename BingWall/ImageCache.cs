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
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Windows.Threading;
using Microsoft.Phone.Net.NetworkInformation;
using System.IO;
using System.Collections.Generic;

namespace BingWall
{

    public class ImageCache
    {

        private static IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
        private static CacheImageRequest pendingRequest;

        public static string GetImage(int daysAgo, string market, OnEndGetImageDelegate callback)
        {
            string fileName = GetCachedFileName(daysAgo, market);
            if (myStore.FileExists(fileName))
            {
                return fileName;
            }
            else
            {
                if (pendingRequest != null)
                {
                    pendingRequest.Abort();
                    pendingRequest = null;
                }

                pendingRequest = new CacheFullImageRequest(fileName, daysAgo, market);
                pendingRequest.Begin(callback);

                return null;
            }

        }

        private static Dictionary<string, string> cachedThumbnails = new Dictionary<string, string>();

        public static string GetThumbnail(int daysAgo, string market, string etag, OnEndGetImageDelegate callback)
        {
            string key = ThumbnailKey(daysAgo, market);
            string fileName = null;
            if (cachedThumbnails.TryGetValue(key, out fileName))
            {
                if (myStore.FileExists(fileName))
                {
                    return fileName;
                }
            }


            if (pendingRequest != null)
            {
                pendingRequest.Abort();
                pendingRequest = null;
            }

            pendingRequest = new CacheThumbnailRequest(daysAgo, market, etag);
            pendingRequest.Begin(callback);

            return null;

        }

        private static string ThumbnailKey(int daysAgo, string market)
        {
            return String.Format("{0:yyyy-MM-dd}{1}", DateTime.Today.AddDays(-daysAgo), market);
        }

        public static void ClearCachedFiles(bool purgeAll)
        {
            string lastFileName = GetCachedFileName(6, "");
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            string[] fileNames = myStore.GetFileNames("cache\\*");
            foreach (string fileName in fileNames)
            {
                if (fileName.CompareTo(lastFileName) < 0 || purgeAll)
                    try
                    {
                        myStore.DeleteFile("cache\\" + fileName);
                    }
                    catch (IsolatedStorageException ise)
                    {
                        Debug.WriteLine(ise.Message);
                    }
            }
        }


        private static string GetCachedFileName(int daysAgo, string market)
        {
            return String.Format("cache\\Bing{0:yyyy-MM-dd}{1}.jpg", DateTime.Today.AddDays(-daysAgo), market);
        }

        public static string CacheThumbnail(Stream bitmapStream, string fileName, int daysAgo, string market, string etag)
        {
            CacheImage(bitmapStream, fileName, false);

            string key = ThumbnailKey(daysAgo, market);

            if (cachedThumbnails.ContainsKey(key))
            {
                cachedThumbnails[key] = fileName;
            }
            else
            {
                cachedThumbnails.Add(key, fileName);
            }

            return fileName;
        }

        public static string CacheImage(Stream bitmapStream, string fileName, bool overwrite)
        {

            string cacheFileName = fileName;

            IsolatedStorageFile myStore;
            //Create virtual store and file stream. Check for duplicate tempJPEG files.
            myStore = IsolatedStorageFile.GetUserStoreForApplication();

            if (myStore.FileExists(cacheFileName))
            {
                if (!overwrite)
                {
                    bitmapStream.Close();
                    Debug.WriteLine("Exists " + fileName);
                    return fileName;
                }
                myStore.DeleteFile(cacheFileName);
            }

            using (IsolatedStorageFileStream myFileStream = myStore.CreateFile(cacheFileName))
            {

                byte[] buffer = new byte[32000];
                while (true)
                {
                    int count = bitmapStream.Read(buffer, 0, buffer.Length);
                    if (count <= 0) break;
                    myFileStream.Write(buffer, 0, count);
                }

                bitmapStream.Close();
                myFileStream.Close();
                Debug.WriteLine("Saved " + fileName);
            }
            return fileName;
        }

    }
}
