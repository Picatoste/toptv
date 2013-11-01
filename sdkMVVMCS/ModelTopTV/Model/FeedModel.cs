/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.ComponentModel;
using System.Xml.Linq;
using TopTV.Model;

namespace TopTV.Model
{
    public class FeedItemsModel : INotifyPropertyChanged
    {
        private string _id;
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                RaisePropertyChanged("Id");
            }
        }

        private AlarmModel _alarm;
        public AlarmModel Alarm
        {
            get
            {
                return _alarm;
            }
            set
            {
                _alarm = value;
                RaisePropertyChanged("Alarm");
            }
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
               _title = value;
                RaisePropertyChanged("Title");
            }
        }
        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                RaisePropertyChanged("Content");
            }
        }
        private string _publishDate;
        public string PublishDate
        {
            get
            {
                 return _publishDate;
            }
            set
            {
                _publishDate = value;
                RaisePropertyChanged("PublishDate");
            }
        }
        private string _canal;
        public string Canal
        {
            get
            {
                return _canal;
            }
            set
            {

                _canal = value;
                RaisePropertyChanged("Summary");
            }
        }
        private string _summary;
        public string Summary
        {
            get
            {
                return _summary;
            }
            set
            {
               _summary =  value;
                RaisePropertyChanged("Summary");
            }
        }

        // Whether a level has been completed or not
        private bool _completed;
        public bool Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                _completed = value;
                RaisePropertyChanged("Completed");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        // Create a copy of an accomplishment to save.
        // If your object is databound, this copy is not databound.
        public FeedItemsModel GetCopy()
        {
            FeedItemsModel copy = (FeedItemsModel)this.MemberwiseClone();
            return copy;
        }


        public XDocument ToXML()
        {
            XDocument docFeed = new XDocument(new XDeclaration("1.0", "utf8", "yes"));
            docFeed.Add(new XElement("feed",
                   new XAttribute("id", this.Id),
                   new XAttribute("title", this.Title),
                   new XAttribute("canal", this.Canal),
                   new XAttribute("content", this.Content),
                   new XAttribute("summary", this.Summary),
                   new XAttribute("date", this.PublishDate),
                   new XAttribute("has_alarm", this.Alarm != null)));

            return docFeed;
        }

        public string ToXMLString()
        {
            XDocument docFeed = new XDocument();
            return docFeed.ToString();
        }

    }
}
