/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using sdkMVVMCS.Model;
using System.ServiceModel.Syndication;
using WindowsBlogReader;
using System.Collections.Generic;
using System.Linq;


namespace sdkMVVMCS.ViewModelNS
{
    public class ViewModel
    {
        private DateTime lastUpdate;
        public DateTime LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }


        public ObservableCollection<FeedItemsModel> Feeds { get; set; }
        public EventHandler<FeedRetrievedEventArgs> grabber_FeedRetrieved;
        public EventHandler<FeedErrorEventArgs> grabber_FeedError;
        //adsd
        public Boolean GetFeeds()
        {
            Boolean HasStorageFeed = IsolatedStorageSettings.ApplicationSettings.Where(s => s.Key.Contains("SaveFeeds")).Count() > 0;
            if (HasStorageFeed)
            {
                GetSavedFeeds();
            }
            else
            {
                UpdateFeeds();
            }

            return HasStorageFeed;
        }

        public void GetSavedFeeds()
        {
            ObservableCollection<FeedItemsModel> a = new ObservableCollection<FeedItemsModel>();

            DateTime settingsLastUpdate = ((DateTime)IsolatedStorageSettings.ApplicationSettings["SaveFeeds_LastUpdate"]);
            if (settingsLastUpdate != null)
            {
                this.LastUpdate = settingsLastUpdate;
                //IsolatedStorageSettings.ApplicationSettings.Remove("SaveFeeds_LastUpdate");
                List<SyndicationItem> synItemRetrieved = new List<SyndicationItem>();
                Feeds = ((ObservableCollection<FeedItemsModel>)IsolatedStorageSettings.ApplicationSettings["SaveFeeds_Items"]);
                
                foreach (FeedItemsModel feedAdd in from feeds in Feeds select feeds)
                {
                    a.Add(feedAdd);
                    SyndicationItem syndItemAdd = new SyndicationItem();
                    syndItemAdd.Id = feedAdd.Id;
                    syndItemAdd.Title = new TextSyndicationContent(feedAdd.Title);
                    syndItemAdd.Summary = new TextSyndicationContent(feedAdd.Summary);
                    syndItemAdd.Content = new TextSyndicationContent(feedAdd.Content);
                    syndItemAdd.PublishDate = DateTime.Parse(feedAdd.PublishDate);
                    syndItemAdd.Copyright = new TextSyndicationContent(feedAdd.Canal);
                    synItemRetrieved.Add(syndItemAdd);
                }
                //this.Feeds = a;
                grabber_FeedRetrieved(null, new FeedRetrievedEventArgs(new SyndicationFeed(synItemRetrieved)));
            }else
            {
                grabber_FeedError(null, new FeedErrorEventArgs(new Exception("ERROR: Not saved feeds in settings.")));
            }
        }

        public void UpdateFeeds()
        {
            RssFeedGrabber grabber = new RssFeedGrabber();

            grabber.FeedRetrieved +=
                new EventHandler<FeedRetrievedEventArgs>(grabber_FeedRetrieved);
            grabber.FeedError +=
                new EventHandler<FeedErrorEventArgs>(grabber_FeedError);

            grabber.RetrieveFeedAsync(
                new Uri("http://www.sincroguia.tv/rss/rss.php?types=relevant"));

            LastUpdate = DateTime.Now;
        }

        public void SetFeeds(SyndicationItem[] feeds)
        {
            ObservableCollection<FeedItemsModel> a = new ObservableCollection<FeedItemsModel>();
            foreach (SyndicationItem feed in feeds)
            {
                FeedItemsModel newFeed = new FeedItemsModel();
                newFeed.Id = feed.Id;
                newFeed.Title = feed.Title.Text;
                newFeed.Content = ((TextSyndicationContent)feed.Content).Text;
                newFeed.Summary = feed.Summary.Text;
                newFeed.PublishDate  = feed.PublishDate.ToString("dd-MM-yyyy HH:mm");
                newFeed.Canal = feed.Copyright.Text;
                a.Add(newFeed);
            }
            Feeds = a;
        }

     

        public void SaveFeeds()
        {

            if (Feeds != null && (this.LastUpdate.Date < DateTime.Now.Date))
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

                var removeSettings = settings.Where(s => s.Key.Contains("SaveFeeds")).ToList();
                foreach (var item in removeSettings)
                {
                    settings.Remove(item.Key);
                } 
            
                //if (settings.Contains("SaveFeeds_LastUpdate"))
                //{
                //    settings["SaveFeeds_LastUpdate"] = this.LastUpdate;
                //}
                //else
                //{
                    settings.Add("SaveFeeds_LastUpdate", this.LastUpdate);
                //}

                //foreach (FeedItemsModel a in Feeds)
                //{
                //if (settings.Contains("SaveFeeds_Items"))
                //{
                //    settings["SaveFeeds_Items"] = Feeds;
                //}
                //else
                //{
                    settings.Add("SaveFeeds_Items", Feeds);
                //}
                //}
                settings.Save();
            }
        }
      
    }

    
}
