using System;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.IO;

namespace BingWall
{
    public class Utils
    {
        private static List<string> lastRequests = new List<string>();

        public static List<string> LastRequests
        {
            get { return Utils.lastRequests; }
        }

        public static void AddDump(string dump)
        {
            lastRequests.Add(dump);
            Debug.WriteLine("Added dump:" + dump);
            if (lastRequests.Count > 5)
            {
                lastRequests.RemoveAt(0);
            }
        }

        public static string GetImageUrl(int daysAgo, string culture)
        {
            return String.Format(
                "http://appserver.m.bing.net/BackgroundImageService/TodayImageService.svc/GetTodayImage?dateOffset=-{0}&urlEncodeHeaders=true&osName=wince&osVersion=7.0&orientation=480x800&deviceName=WP7Device&mkt={1}",
                daysAgo, culture);

        }

        public static string EffectiveCulture(string culture)
        {
            string cultureCode = culture == "default" ? CultureInfo.CurrentCulture.Name : culture;
            return cultureCode;
        }
/*
        internal static string SaveDitheredBitmap(string fileName, WriteableBitmap image)
        {
            string saveFileName = (fileName.Split('.'))[0];
            saveFileName += "pr.jpg";
            
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                //WriteableBitmap image = new WriteableBitmap(480, 800);
                using (Stream file = myStore.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                {
                    image.LoadJpeg(file);
                    file.Close();
                }

                DitherBitmap(image);

                using (Stream file = myStore.CreateFile(saveFileName))
                {
                    image.SaveJpeg(file, image.PixelWidth, image.PixelHeight, 0, 85);
                    file.Close();
                }

                return saveFileName;
            }
            catch (Exception ex)
            {
                return fileName;
            }
        }

        private static void DitherBitmap(WriteableBitmap image)
        {
            int pixelPos = 0;
            int lowerLeftNeighborPos = image.PixelWidth - 1;

            for(int y = 0; y < image.PixelHeight; y++)
            {
                for(int x = 0; x < image.PixelWidth; x++)
                {
                    int[] pixelDepth = { 4, 4, 4 };

#if DEBUG
                    if (Settings.ResampleTo16Bit)
                    {
                        pixelDepth = new int[] { 5, 6, 5 };
                    }
#endif

#if DEBUG
                    if(x == 369 && y == 230)
                    {
                        Debug.WriteLine("bad part");
                    }
#endif
                    int[] errors = new int[3];
                    image.Pixels[pixelPos] = DownSamplePixel(image.Pixels[pixelPos], ref errors, pixelDepth);
                    if (x < image.PixelWidth - 1)
                    {
                        image.Pixels[pixelPos + 1] = PropagateErrors(image.Pixels[pixelPos + 1], errors, 7);
                    }
                    if (y < image.PixelHeight - 1)
                    {
                        if (x > 0)
                        {
                            image.Pixels[lowerLeftNeighborPos] = PropagateErrors(image.Pixels[lowerLeftNeighborPos], errors, 3);
                        }
                        image.Pixels[lowerLeftNeighborPos + 1] = PropagateErrors(image.Pixels[lowerLeftNeighborPos + 1], errors, 5);
                        if (x < image.PixelWidth - 1)
                        {
                            image.Pixels[lowerLeftNeighborPos + 2] = PropagateErrors(image.Pixels[lowerLeftNeighborPos + 2], errors, 1);
                        }
                    }
                    pixelPos++;
                    lowerLeftNeighborPos++;

                }
            }
        }

        private static int DownSamplePixel(int pixel, ref int[] errors, int[] pixelDepth)
        {
            ARGB32 argb32 = (ARGB32)pixel;
            int error = 0;

            argb32.R = DownSampleComponent(argb32.R, pixelDepth[0], ref error);
            errors[(int)ARGB32.Component.Red] = error;

            argb32.G = DownSampleComponent(argb32.G, pixelDepth[1], ref error);
            errors[(int)ARGB32.Component.Green] = error;

            argb32.B = DownSampleComponent(argb32.B, pixelDepth[2], ref error);
            errors[(int)ARGB32.Component.Blue] = error;

            pixel = (int)argb32;

            return (int)argb32;
        }

        private static int[] mask = { 0, 0x80, 0xc0, 0xe0, 0xf0, 0xf8, 0xfc, 0xfe };
        private static int[] roundup = { 0, 0x7f, 0x3f, 0x1f, 0xf, 0x7, 0x3, 0x1 };

        private static byte DownSampleComponent(byte component, int bits, ref int error)
        {
            int val = (component & mask[bits]);
            //if(val > 0) val += roundup[bits];
            error = (component - val);
            Debug.Assert(error < 32 && error > -32);
            return (byte)val;
        }

        private static int PropagateErrors(int pixel, int[] errors, int weight)
        {
            ARGB32 argb32 = (ARGB32)pixel;
            int newVal;

            newVal = (argb32.R + ((errors[(int)ARGB32.Component.Red] * weight + 8) >> 4));
            Debug.Assert(newVal >= 0);
            argb32.R = (byte)(newVal < 256 ? newVal : 255);

            newVal = (argb32.G + ((errors[(int)ARGB32.Component.Green] * weight + 8) >> 4));
            Debug.Assert(newVal >= 0);
            argb32.G = (byte)(newVal < 256 ? newVal : 255);

            newVal = (argb32.B + ((errors[(int)ARGB32.Component.Blue] * weight + 8) >> 4));
            Debug.Assert(newVal >= 0);
            argb32.B = (byte)(newVal < 256 ? newVal : 255);

            return (int)argb32;
        }

        */

        internal static string GetThumbnailUrl(int daysAgo, string culture)
        {
            return String.Format(
                "http://appserver.m.bing.net/BackgroundImageService/TodayImageService.svc/GetTodayImage?dateOffset=-{0}&urlEncodeHeaders=true&osName=wince&osVersion=7.0&orientation=240x400&deviceName=WP7Device&mkt={1}",
                daysAgo, culture);
        }

        internal static string GetEtag(System.Net.HttpWebResponse response)
        {
            try
            {
                return response.Headers["ETag"];
            }
            catch (Exception)
            {
                return Guid.NewGuid().ToString();
            }
        }
    }
}

