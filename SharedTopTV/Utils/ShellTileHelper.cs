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
using System.Windows.Threading;

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

        private static AutoResetEvent Wait;


        public static void UpdateCycleTile(List<FeedItemsModel> feeds)
        {
            var threadFinishEvents = new List<WaitHandle>();

            DownloadImageGeneral("Small", 159, 159);
            DownloadImageGeneral("Medium", 336, 336);
            DownloadImageGeneral("Large", 336, 691);
            DownloadImages(feeds);
            DownloadData(feeds);


        }


        private static void DownloadImageGeneral(string TypeName, int height, int width)
        {
            ShellTileHelper.Wait = new AutoResetEvent(false);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
              {

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
                      fileStream.Flush();
                      fileStream.Close();
                      fileStream.Dispose();
                      myIsolatedStorage.Dispose();
                      ;

                  }
                  ShellTileHelper.Wait.Set();
              });

            ShellTileHelper.Wait.WaitOne();
        }


        private static void DownloadImages(List<FeedItemsModel> feeds)
        {

            ShellTileHelper.Wait = new AutoResetEvent(false);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                for (int i = 0; feeds.Count > i && i <= 4; i++)
                {
                    if (feeds != null && feeds.Count > 0 && feeds[i] != null)
                    {
                                var localI = i;
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
                                            isoStoreFile.Flush();
                                            isoStoreFile.Close();
                                            isoStoreFile.Dispose();
                                            response.Close();
                                            response.Dispose();
                                        }
                                        myIsolatedStorage.Dispose();
                                        if (localI == 4)
                                        {
                                            ShellTileHelper.Wait.Set();
                                        }
                                    }
                                }, null);
                    }
                }

            });
            ShellTileHelper.Wait.WaitOne();
        }

        private static void DownloadData(List<FeedItemsModel> feeds)
        {
            ShellTileHelper.Wait = new AutoResetEvent(false);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
              {
                  for (int i = 0; feeds.Count > i && i <= 4; i++)
                  {
                      if (feeds != null && feeds.Count > 0 && feeds[i] != null)
                      {
                          var localI = i;


                          using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                          {

                              using (IsolatedStorageFileStream isoStream = myIsolatedStorage.OpenFile("shared/shellcontent/TopTVTilesContent_Data" + localI + ".xml",
                                                                             FileMode.OpenOrCreate,
                                                                             FileAccess.ReadWrite))
                              {
                                  feeds[i].ToXML().Save(isoStream);
                                  isoStream.Flush();
                                  isoStream.Close();
                                  isoStream.Dispose();
                              }
                              myIsolatedStorage.Dispose();
                          }

                          if (localI == 4)
                          {
                              ShellTileHelper.Wait.Set();
                          }

                      }
                  }
              });

            ShellTileHelper.Wait.WaitOne();
        }

        #endregion
    }
}
