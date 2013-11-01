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
using System.ServiceModel.Syndication;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

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

            SettingsFeedHelper.SaveFeeds(e.Feed.Items.ToArray());
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
                                                                   FileMode.Open,
                                                                   FileAccess.Read))
                    {
                        docfeed = XDocument.Load(isoStream);
                        isoStore.Dispose();
                        isoStream.Flush();
                        isoStream.Dispose();
                        isoStream.Dispose();
                    }
                }

                if (docfeed != null)
                {
                    string title = docfeed.Element("feed").Attribute("title").Value;
                    string date = docfeed.Element("feed").Attribute("date").Value;
                    string canal = docfeed.Element("feed").Attribute("canal").Value;
                    Boolean alarm = docfeed.Element("feed").Attribute("has_alarm").Value == "true";


                    if (title.Length >= 135)
                    {
                        RenderText(title.Substring(0, 135) + "...", canal, date, 336, 336, 30, "TopTVTilesContent_Image" + tileNumber + ".png", "TopTVTilesContent_MediumBackgroundImage_back" + tileNumber + ".jpg");
                    }
                    else
                    {
                        RenderText(title, canal, date, 336, 336, 28, "TopTVTilesContent_Image" + tileNumber + ".png", "TopTVTilesContent_MediumBackgroundImage_back" + tileNumber + ".jpg");
                    }

                    //RenderText(title, canal, date, 691, 336, 40, "TopTVTilesContent_Image" + tileNumber + ".png", "TopTVTilesContent_LargeBackgroundImage_back" + tileNumber + ".jpg");

                    
                    //tile.CycleImages.ToList().Add(new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back" + tileNumber + ".jpg"));
                    
            }
        }

        public void UpdateTileTask()
        {
            ShellTile tile = ShellTile.ActiveTiles.First();
           
            if (tile != null && IsolatedStorageSettings.ApplicationSettings.Contains("SaveFeeds_TotalFeeds"))
            {
                CycleTileData data = new CycleTileData();
                for (int i = 1; (Convert.ToInt32(IsolatedStorageSettings.ApplicationSettings["SaveFeeds_TotalFeeds"]) > i && i <= 9); i++)
                {
                    data.Count = 0;
                    data.SmallBackgroundImage =  new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_SmallBackgroundImage.jpg", UriKind.Absolute);
                    data.Title = "TopTV";
                    AddImageTileCycle(i);
                }

                 tile.Update(new CycleTileData()
                {
                    Title = "Top TV",
                    CycleImages = new Uri[]
                {
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back1.jpg", UriKind.Absolute), 
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back2.jpg", UriKind.Absolute), 
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back3.jpg", UriKind.Absolute), 
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back4.jpg", UriKind.Absolute), 
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back5.jpg", UriKind.Absolute), 
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back6.jpg", UriKind.Absolute), 
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back7.jpg", UriKind.Absolute), 
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back8.jpg", UriKind.Absolute), 
                        new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back9.jpg", UriKind.Absolute), 
                }
                });

                //tile.Update(data);
            }
        }

        //public void UpdateTileTask()
        //{
        //    ShellTile tile = ShellTile.ActiveTiles.First();
           
        //    XDocument docfeed;
        //    if (tile != null && IsolatedStorageSettings.ApplicationSettings.Contains("SaveFeeds_TotalFeeds"))
        //    {

        //        string tileNumber = GenerateNumber();
        //        string tileNumber2 = GenerateNumber();
        //        using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
        //        {
        //            using (IsolatedStorageFileStream isoStream = isoStore.OpenFile("shared/ShellContent/TopTVTilesContent_Data" + tileNumber + ".xml",
        //                                                           FileMode.Open,
        //                                                           FileAccess.Read))
        //            {
        //                docfeed = XDocument.Load(isoStream);
        //                isoStore.Dispose();
        //                isoStream.Dispose();
        //            }
        //        }

        //        if (docfeed != null)
        //        {
        //            string title = docfeed.Element("feed").Attribute("title").Value;
        //            string date = docfeed.Element("feed").Attribute("date").Value;
        //            string canal = docfeed.Element("feed").Attribute("canal").Value;
        //            Boolean alarm = docfeed.Element("feed").Attribute("has_alarm").Value == "true";

        //            FlipTileData data = new FlipTileData();
        //            data.Title = date;
        //            data.BackTitle = date;
        //            data.BackContent = "";
        //            data.WideBackContent = "";
        //            data.Count = (alarm) ? 1 : 0;

        //            data.SmallBackgroundImage = new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_SmallBackgroundImage.jpg", UriKind.Absolute);
                    
        //            data.BackgroundImage = new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back2.jpg", UriKind.Absolute);
        //            data.WideBackgroundImage = new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_LargeBackgroundImage_back2.jpg", UriKind.Absolute);

        //            data.BackBackgroundImage = new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_MediumBackgroundImage_back1.jpg", UriKind.Absolute);                    
        //            data.WideBackBackgroundImage = new Uri("isostore:/Shared/ShellContent/TopTVTilesContent_LargeBackgroundImage_back1.jpg", UriKind.Absolute);


                   
        //            if (title.Length >= 135)
        //            {
        //                RenderText(title.Substring(0, 135) + "...", canal, date, 336, 336, 30, "TopTVTilesContent_Image" + tileNumber + ".png", "TopTVTilesContent_MediumBackgroundImage_back1.jpg");
        //            }
        //            else
        //            {
        //                RenderText(title, canal, date, 336, 336, 28, "TopTVTilesContent_Image" + tileNumber + ".png", "TopTVTilesContent_MediumBackgroundImage_back1.jpg");
        //            }

        //            RenderText(title, canal, date, 691, 336, 40, "TopTVTilesContent_Image" + tileNumber + ".png", "TopTVTilesContent_LargeBackgroundImage_back1.jpg");


        //            if (title.Length >= 135)
        //            {
        //                RenderText(title.Substring(0, 135) + "...", canal, date, 336, 336, 30, "TopTVTilesContent_Image" + tileNumber2 + ".png", "TopTVTilesContent_MediumBackgroundImage_back2.jpg");
        //            }
        //            else
        //            {
        //                RenderText(title, canal, date, 336, 336, 28, "TopTVTilesContent_Image" + tileNumber2 + ".png", "TopTVTilesContent_MediumBackgroundImage_back2.jpg");
        //            }

        //            RenderText(title, canal, date, 691, 336, 40, "TopTVTilesContent_Image" + tileNumber2 + ".png", "TopTVTilesContent_LargeBackgroundImage_back2.jpg");

        //            tile.Update(data);
        //        }
        //    }
        //}


        private static AutoResetEvent Wait;
        private static void RenderText(string text, string canal, string date, int width, int height, int fontsize, string imagenameBack, string imageNameFinal)
        {
            ScheduledAgent.Wait = new AutoResetEvent(false);  
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {

                    WriteableBitmap b = new WriteableBitmap(width, height);
                    
                    Grid canvas = new Grid();
                    canvas.Width = b.PixelWidth;
                    canvas.Height = b.PixelHeight;


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
                    Grid canalCanvas = new Grid();
                    canalCanvas.Background = new SolidColorBrush(Colors.Black);
                    var textBlockCanal = new TextBlock();

                    textBlockCanal.Text = canal;
                    textBlockCanal.FontWeight = FontWeights.Bold;
                    textBlockCanal.TextAlignment = TextAlignment.Left;
                    textBlockCanal.HorizontalAlignment = HorizontalAlignment.Left;
                    textBlockCanal.VerticalAlignment = VerticalAlignment.Top;
                    textBlockCanal.Margin = new Thickness(80,40,10,10);
                    textBlockCanal.Width = b.PixelWidth - textBlockCanal.Margin.Left * 2;
                    textBlockCanal.Height = height * 0.2;
                    textBlockCanal.TextWrapping = TextWrapping.Wrap;
                    textBlockCanal.Foreground = new SolidColorBrush(Colors.White); //color of the text on the Tile
                    textBlockCanal.FontSize = fontsize;
                    canalCanvas.Children.Add(textBlockCanal);
                    canvas.Children.Add(canalCanvas);

                    Grid.SetColumn(textBlockCanal, 0);
                    Grid.SetRow(textBlockCanal, 0);

                    //var textBlockDate = new TextBlock();
                    //textBlockDate.Text = date;
                    //textBlockDate.FontWeight = FontWeights.Bold;
                    //textBlockDate.TextAlignment = TextAlignment.Right;
                    //textBlockDate.HorizontalAlignment = HorizontalAlignment.Left;
                    //textBlockDate.VerticalAlignment = VerticalAlignment.Top;
                    //textBlockDate.Margin = new Thickness(5);
                    //textBlockDate.Width = b.PixelWidth - textBlockDate.Margin.Left * 2;
                    //textBlockDate.Height = height * 0.2;
                    //textBlockDate.TextWrapping = TextWrapping.Wrap;
                    //textBlockDate.Foreground = new SolidColorBrush(Colors.White); //color of the text on the Tile

                    //textBlockDate.FontSize = fontsize;

                    //canvas.Children.Add(textBlockDate);

                    //Grid.SetColumn(textBlockDate, 1);
                    //Grid.SetRow(textBlockDate, 0);

                    var textBlock = new TextBlock();
                    textBlock.Text = text;
                    textBlock.FontWeight = FontWeights.Bold;
                    textBlock.TextAlignment = TextAlignment.Left;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    textBlock.Margin = new Thickness(80, 80, 40, 40);
                    textBlock.Width = b.PixelWidth - textBlock.Margin.Left * 2;
                    textBlock.Height = height * 0.8;
                    textBlock.TextWrapping = TextWrapping.Wrap;
                    textBlock.Foreground = new SolidColorBrush(Colors.White); //color of the text on the Tile

                    textBlock.FontSize = fontsize;

                    canvas.Children.Add(textBlock);
                    Grid.SetColumnSpan(textBlock, 2);
                    Grid.SetColumn(textBlock, 0);
                    Grid.SetRow(textBlock, 1);


                    b.Render(background, null);
                    b.Render(canvas, null);
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