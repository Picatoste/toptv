using System;
using System.ServiceModel.Syndication;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace WindowsBlogReader
{
    #region Support Classes (exception, eventargs)

    public class FeedException : Exception
    {
        public FeedException()
            : base("Exception retrieving or processing feed.")
        {
        }

        public FeedException(string message)
            : base("Exception retrieving or processing feed. " + message)
        {
        }

        public FeedException(string message, Exception innerException)
            : base("Exception retrieving or processing feed. " + message,
                    innerException)
        {
        }

    }


    public class FeedRetrievedEventArgs : EventArgs
    {
        private SyndicationFeed _feed;

        public FeedRetrievedEventArgs(SyndicationFeed feed)
        {
            _feed = feed;
        }

        public SyndicationFeed Feed
        {
            get { return _feed; }
        }

    }

    public class FeedErrorEventArgs : EventArgs
    {
        private Exception _exception;

        public FeedErrorEventArgs(Exception exception)
        {
            _exception = exception;
        }

        public Exception Error
        {
            get { return _exception; }
        }

    }

    #endregion

    public class RssFeedGrabber
    {
        public event EventHandler<FeedRetrievedEventArgs> FeedRetrieved;
        public event EventHandler<FeedErrorEventArgs> FeedError;

        private const string _nullResultErrorMessage =
            "The returned stream was null. " +
            "The site may not have a valid client access policy in place.";
        private const string _parseErrorMessage =
            "Unable to parse feed.";

        /// <summary>
        /// Makes an async call to return the feed data
        /// </summary>
        /// <param name="url">URL of the feed</param>
        public void RetrieveFeedAsync(Uri url)
        {
            WebClient client = new WebClient();

            client.OpenReadCompleted +=
                new OpenReadCompletedEventHandler(client_OpenReadCompleted);

            client.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
            client.OpenReadAsync(url);
        }


        /// <summary>
        /// Event handler called when the WebClient returns the data from the call
        /// </summary>
        private void client_OpenReadCompleted(object sender,
            OpenReadCompletedEventArgs e)
        {
            // process the feed

            if (e.Error == null && e.Result != null)
            {
                using (XmlReader reader = XmlReader.Create(e.Result))
                {
                    try
                    { 
                        XDocument docFeed = XDocument.Load(reader);

                        XNamespace content = "http://purl.org/rss/1.0/modules/content/";
                        var feedItems = from datas in docFeed.Descendants("channel").Elements("item")
                                        select new SyndicationItem
                                        {
                                            Id = (string)datas.Element("GUID"),
                                            Title = new TextSyndicationContent((string)datas.Element("title")),
                                            Summary = new TextSyndicationContent((string)datas.Element("description")),
                                            PublishDate = new DateTimeOffset((DateTime)datas.Element("pubDate")),
                                            Content = new TextSyndicationContent(Regex.Match(((string)datas.Element(content + "encoded")), "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase).Groups[1].Value),
                                            Copyright = new TextSyndicationContent(Regex.Match(((string)datas.Element(content + "encoded")), "<b>Canal:</b>(.*?)\n", RegexOptions.Singleline).Groups[1].Value)
                                        };

                       
                        SyndicationFeed feed = new SyndicationFeed("Destacados", "Destacados", null, feedItems.ToList());

                        FeedRetrievedEventArgs args =
                            new FeedRetrievedEventArgs(feed);

                        OnFeedRetrieved(args);
                    }
                    catch (Exception ex)
                    {
                        // the loader couldn't load the feed
                        FeedErrorEventArgs args =
                            new FeedErrorEventArgs(
                                new FeedException(_parseErrorMessage, ex));
                        OnFeedError(args);
                    }
                }
            }
            else
            {
                if (e.Error != null)
                {
                    // there was an error returned from the call
                    FeedErrorEventArgs args = new FeedErrorEventArgs(e.Error);
                    OnFeedError(args);
                }
                else
                {
                    // an empty stream. You get this silent failure when there 
                    // is no crossdomain file in place (at least in beta2)
                    FeedErrorEventArgs args =
                        new FeedErrorEventArgs(
                            new FeedException(_nullResultErrorMessage));
                    OnFeedError(args);
                }
            }
        }

        /// <summary>
        /// Internal method used to raise the feed event
        /// </summary>
        protected void OnFeedRetrieved(FeedRetrievedEventArgs args)
        {
            if (FeedRetrieved != null)
                FeedRetrieved(this, args);
        }

        /// <summary>
        /// Internal method used to raise the error event
        /// </summary>
        protected void OnFeedError(FeedErrorEventArgs args)
        {
            if (FeedError != null)
                FeedError(this, args);
        }

    }
}