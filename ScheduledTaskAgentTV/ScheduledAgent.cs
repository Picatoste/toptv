#define DEBUG_AGENT
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.Linq;
using System;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.IO;
using TopTV.Utils;
using SharedTopTV;
using System.ServiceModel.Syndication;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Threading;
using System.Text;

namespace ScheduledTaskAgentTV
{

    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// Constructor de ScheduledAgent que inicializa el controlador UnhandledException
        /// </remarks>
        static ScheduledAgent()
        {
            // Suscribirse al controlador de excepciones administradas
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Código para ejecutar en excepciones no controladas
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // Se ha producido una excepción no controlada; interrumpir el depurador
                Debugger.Break();
            }
        }



        public string GenerateNumber()
        {
            Random random = new Random();
            return random.Next(1, Convert.ToInt32(IsolatedStorageSettings.ApplicationSettings["SaveFeeds_TotalFeeds"])).ToString();
        }

        /// <summary>
        /// Agente que ejecuta una tarea programada
        /// </summary>
        /// <param name="task">
        /// Tarea invocada
        /// </param>
        /// <remarks>
        /// Se llama a este método cuando se invoca una tarea periódica o con uso intensivo de recursos
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            DateTime dateTime = new DateTime();
            SettingsFeedHelper.UpdateFeeds(grabber_FeedRetrieved, grabber_FeedError, ref dateTime);
#if DEBUG_AGENT
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(30));
#endif

        }

        #region Load Feeds

        void grabber_FeedError(object sender, FeedErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ERROR - " + e.Error.Message);
            NotifyComplete();
        }

        void grabber_FeedRetrieved(object sender, FeedRetrievedEventArgs e)
        {
            foreach (SyndicationItem item in e.Feed.Items)
            {
                System.Diagnostics.Debug.WriteLine(item.Title.Text);
            }
            ShellTileHelper.UpdateCycleTile(SettingsFeedHelper.SaveFeeds(e.Feed.Items.ToArray()));
            UpdateTileTask();
            NotifyComplete();
        }
        #endregion

        private void AddImageTileCycle(int tileNumber)
        {
            XDocument docfeed;
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream isoStream = isoStore.OpenFile("shared/ShellContent/TopTVTilesContent_Data" + tileNumber + ".xml",
                                                                   FileMode.OpenOrCreate,
                                                                   FileAccess.ReadWrite))
                    {
                        using (StreamReader oReader = new StreamReader(isoStream, Encoding.GetEncoding("ISO-8859-1")))
                        {
                        docfeed = XDocument.Load(oReader);
                        }

                        //docfeed = XDocument.Load(isoStream);
                        isoStore.Dispose();
                        isoStream.Dispose();
                    }
                }

                if (docfeed != null)
                {
                    string title = docfeed.Element("feed").Descendants("title").FirstOrDefault().Value;
                    string date = docfeed.Element("feed").Descendants("date").FirstOrDefault().Value;
                    string canal = docfeed.Element("feed").Descendants("canal").FirstOrDefault().Value;
                    Boolean alarm = docfeed.Element("feed").Descendants("has_alarm").FirstOrDefault().Value == "true";


                    if (title.Length >= 135)
                    {
                        RenderTile(title.Substring(0, 135) + "...", canal, date, 336, 336, "TopTVTilesContent_Image" + tileNumber + ".png", "TopTVTilesContent_MediumBackgroundImage_back" + tileNumber + ".jpg");                    
                    }
                    else
                    {
                        RenderTile(title, canal, date, 336, 336, "TopTVTilesContent_Image" + tileNumber + ".png", "TopTVTilesContent_MediumBackgroundImage_back" + tileNumber + ".jpg");        
                    }
                    
            }
        }

        public void UpdateTileTask()
        {
            ShellTile tile = ShellTile.ActiveTiles.First();
           
            if (tile != null && IsolatedStorageSettings.ApplicationSettings.Contains("SaveFeeds_TotalFeeds"))
            {
                CycleTileData data = new CycleTileData();
                for (int i = 0; (Convert.ToInt32(IsolatedStorageSettings.ApplicationSettings["SaveFeeds_TotalFeeds"])-1 > i && i <= 4); i++)
                {
                    data.Count = 0;
                    data.SmallBackgroundImage = new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_SmallBackgroundImage.jpg", UriKind.Absolute);
                    data.Title = "TopTV";
                    AddImageTileCycle(i);
                }

                 tile.Update(new CycleTileData()
                {
                    Title = "Top TV",
                    CycleImages = new Uri[]
                {
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back0.jpg", UriKind.Absolute), 
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back1.jpg", UriKind.Absolute), 
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back2.jpg", UriKind.Absolute), 
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back3.jpg", UriKind.Absolute), 
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back4.jpg", UriKind.Absolute),
                }
                });
            }
        }

        private static AutoResetEvent Wait;

        private static void RenderTile(string text, string canal, string date, int width, int height, string imagenameBack, string imageNameFinal)
        {
            ScheduledAgent.Wait = new AutoResetEvent(false);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                WriteableBitmap b = new WriteableBitmap(width, height);
                TileMediumTopTV customTile = new TileMediumTopTV();
                customTile.Measure(new Size(336, 336));
                customTile.Arrange(new Rect(0, 0, 336, 336));
                var background = new Canvas();
                background.Height = b.PixelHeight;
                background.Width = b.PixelWidth;
                BitmapImage bmi = new BitmapImage();
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + imagenameBack, System.IO.FileMode.OpenOrCreate, isf))
                    {
                        bmi.SetSource(imageStream);
                        imageStream.Flush();
                        imageStream.Close();
                        imageStream.Dispose();
                        isf.Dispose();
                    }
                }
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = bmi;
                brush.Stretch = Stretch.UniformToFill;
                //Created background color as Accent color
                background.Background = brush;
                SolidColorBrush colorBlackTransparent = new SolidColorBrush(Colors.Black);
                colorBlackTransparent.Opacity = 0.5;
                customTile.Canal = canal;
                customTile.Descripcion = text;
                customTile.Fecha = date;
                customTile.Background = colorBlackTransparent;
                b.Render(background, null);
                b.Render(customTile, null);
                b.Invalidate(); //Draw bitmap
                //Save bitmap as jpeg file in Isolated Storage
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream imageStream = new IsolatedStorageFileStream("/Shared/ShellContent/" + imageNameFinal, System.IO.FileMode.OpenOrCreate, isf))
                    {
                        b.SaveJpeg(imageStream, b.PixelWidth, b.PixelHeight, 0, 100);
                        imageStream.Flush();
                        imageStream.Close();
                        imageStream.Dispose();
                        isf.Dispose();
                    }
                }
                ScheduledAgent.Wait.Set();
            });
            ScheduledAgent.Wait.WaitOne();
        }
    }
}