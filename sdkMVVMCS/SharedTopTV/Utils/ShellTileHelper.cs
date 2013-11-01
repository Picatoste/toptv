using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using TopTV.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Xml;

namespace TopTV.Utils
{
    public static class ShellTileHelper
    {
        public static enumTypeTile typeTile;
        
        public enum enumTypeTile
        {
            Iconic,
            cycle
        }


        #region Tile Cycle
        private static readonly string CycleTileQuery = "tile=cycle";
        


        public static void UpdateCycleTile(List<FeedItemsModel> feeds)
        {
            var threadFinishEvents = new List<WaitHandle>();

            DownloadImageGeneral(ref threadFinishEvents, "Small", 159, 159);
            DownloadImageGeneral(ref threadFinishEvents, "Medium", 336, 336);
            DownloadImageGeneral(ref threadFinishEvents, "Large", 336, 691);
            DownloadImages(ref threadFinishEvents, feeds);
            DownloadData(ref threadFinishEvents, feeds);


            new Thread(() =>
            {
                Mutex.WaitAll(threadFinishEvents.ToArray());

                //ResolveTiles();

                //isoStore.Dispose();
            }).Start();

        }

        //private static void ResolveTiles()
        //{
        //    ShellTile tile = ShellTile.ActiveTiles.First();
        //    if (tile != null)
        //    {
        //        tile.Update(new CycleTileData()
        //        {
        //            Title = "Top TV",
        //            CycleImages = new Uri[]
        //        {
        //        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_Image0.png", UriKind.Absolute), 
        //        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_Image1.png", UriKind.Absolute), 
        //        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_Image2.png", UriKind.Absolute), 
        //        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_Image3.png", UriKind.Absolute), 
        //        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_Image4.png", UriKind.Absolute), 
        //        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_Image5.png", UriKind.Absolute), 
        //        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_Image6.png", UriKind.Absolute), 
        //        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_Image7.png", UriKind.Absolute), 
        //        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_Image8.png", UriKind.Absolute), 
        //        }
        //        });
        //    }

        //}

        private static void DownloadImageGeneral(ref List<WaitHandle> threadFinishEvents, string TypeName, int height, int width)
        {

            var threadFinish = new EventWaitHandle(false, EventResetMode.ManualReset);
            threadFinishEvents.Add(threadFinish);

            String smallBackImage = "shared/ShellContent/TopTVTilesContent_" + TypeName + "BackgroundImage.jpg";

            // Create virtual store and file stream. Check for duplicate tempJPEG files.
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(smallBackImage))
                {
                    myIsolatedStorage.DeleteFile(smallBackImage);
                }

                IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(smallBackImage);

                StreamResourceInfo sri = null;
                Uri uri = new Uri(TypeName + ".png", UriKind.Relative);
                sri = Application.GetResourceStream(uri);

                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(sri.Stream);
                WriteableBitmap wb = new WriteableBitmap(bitmap);

                // Encode WriteableBitmap object to a JPEG stream. 
                Extensions.SaveJpeg(wb, fileStream, width, height, 0, 100);

                sri.Stream.Close();
                sri.Stream.Dispose();
                fileStream.Close();
                fileStream.Dispose();
                myIsolatedStorage.Dispose();
                
                threadFinish.Set();
            }
        }

        private static void DownloadImages(ref List<WaitHandle> threadFinishEvents, List<FeedItemsModel> feeds)
        {

            for (int i = 0; i < feeds.Count; i++)
            {
                if (feeds != null && feeds.Count > 0 && feeds[i] != null)
                {
                    var localI = i;

                    var threadFinish = new EventWaitHandle(false, EventResetMode.ManualReset);
                    threadFinishEvents.Add(threadFinish);

                    var request = WebRequest.CreateHttp(feeds[i].Content);
                    request.BeginGetResponse(ir =>
                    {
                        using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            var result = request.EndGetResponse(ir);
                            using (IsolatedStorageFileStream isoStoreFile = myIsolatedStorage.OpenFile("shared/shellcontent/TopTVTilesContent_Image" + localI + ".png",
                                                                        FileMode.OpenOrCreate,
                                                                        FileAccess.ReadWrite))

                            using (var response = result.GetResponseStream())
                            {
                                var dataBuffer = new byte[1024];
                                while (response.Read(dataBuffer, 0, dataBuffer.Length) > 0)
                                {
                                    isoStoreFile.Write(dataBuffer, 0, dataBuffer.Length);
                                }
                                isoStoreFile.Close();
                                isoStoreFile.Dispose();
                            }

                            
                            myIsolatedStorage.Dispose();

                            threadFinish.Set();
                        }

                    }, null);
                }
            }
        }

        private static void DownloadData(ref List<WaitHandle> threadFinishEvents, List<FeedItemsModel> feeds)
        {
            for (int i = 0; i < feeds.Count; i++)
            {
                if (feeds != null && feeds.Count > 0 && feeds[i] != null)
                {
                    var localI = i;

                    var threadFinish = new EventWaitHandle(false, EventResetMode.ManualReset);
                    threadFinishEvents.Add(threadFinish);

                    using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {

                        using (IsolatedStorageFileStream isoStream = myIsolatedStorage.OpenFile("shared/shellcontent/TopTVTilesContent_Data" + localI + ".xml",
                                                                       FileMode.OpenOrCreate,
                                                                       FileAccess.ReadWrite))
                        {
                            feeds[i].ToXML().Save(isoStream);
                            isoStream.Close();
                            isoStream.Dispose();
                        }

                        myIsolatedStorage.Dispose();
                    }

                    threadFinish.Set();
                }
            }
        }

        #endregion
    }
}
