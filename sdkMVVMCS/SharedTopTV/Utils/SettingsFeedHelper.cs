using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using TopTV.Model;

namespace TopTV.Utils
{
    public static class SettingsFeedHelper
    {

        public static String URL_FEED = "http://www.sincroguia.tv/rss/rss.php?types=relevant";

        public static void SaveFeeds(List<FeedItemsModel> feeds, DateTime lastDate)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            //Boolean HasSaveFeedsAndNotChanges = settings.Where(s => s.Key.Contains("SaveFeeds")).Count() > 0;

            if (feeds != null)
            {
                var removeSettings = settings.Where(s => s.Key.Contains("SaveFeeds")).ToList();
                foreach (var item in removeSettings)
                {
                    settings.Remove(item.Key);
                }
                settings.Add("SaveFeeds_LastUpdate", lastDate);
                settings.Add("SaveFeeds_Items", feeds);
                settings.Add("SaveFeeds_TotalFeeds", feeds.Count().ToString());                
                settings.Save();
            }
        }

        public static void SaveFeeds(SyndicationItem[] feedsSyndication)
        {
            SettingsFeedHelper.SaveFeeds(SettingsFeedHelper.ConvertFeeds(feedsSyndication), DateTime.Now);
        }

        public static void UpdateFeeds(EventHandler<FeedRetrievedEventArgs> grabber_FeedRetrieved, EventHandler<FeedErrorEventArgs> grabber_FeedError, ref DateTime LastUpdate)
        {
            RssFeedGrabber grabber = new RssFeedGrabber();

            grabber.FeedRetrieved +=
                new EventHandler<FeedRetrievedEventArgs>(grabber_FeedRetrieved);
            grabber.FeedError +=
                new EventHandler<FeedErrorEventArgs>(grabber_FeedError);

            grabber.RetrieveFeedAsync(
                new Uri(URL_FEED, UriKind.Absolute));

            LastUpdate = DateTime.Now;
        }

        public static List<FeedItemsModel> ConvertFeeds(SyndicationItem[] feeds)
        {
            List<FeedItemsModel> returnFeedItemsModel = new List<FeedItemsModel>();
            foreach (SyndicationItem feed in feeds)
            {
                FeedItemsModel newFeed = new FeedItemsModel();
                newFeed.Id = feed.Id;
                newFeed.Title = feed.Title.Text;
                newFeed.Content = ((TextSyndicationContent)feed.Content).Text;
                newFeed.Summary = feed.Summary.Text;
                newFeed.PublishDate = feed.PublishDate.ToString("dd-MM-yyyy HH:mm");
                newFeed.Canal = feed.Copyright.Text;
                returnFeedItemsModel.Add(newFeed);
            }
            return returnFeedItemsModel;
        }
    }
}
