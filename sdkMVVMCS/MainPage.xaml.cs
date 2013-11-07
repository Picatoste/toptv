
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
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using sdkMVVMCS.ViewModelNS;
using TopTV.Utils;
using SplashLoading;
using System.ServiceModel.Syndication;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using Microsoft.Phone.Shell;
using System.Threading;
using System.Net;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.IO;
using Microsoft.Phone.Scheduler;
using ScheduledTaskAgentTV;


namespace sdkMVVMCS
{
    public partial class MainPage : PhoneApplicationPageBase
    {

        const String URL_FEED = "http://www.sincroguia.tv/rss/rss.php?types=relevant";
        private ViewModel vm;
        // Constructor

        public MainPage()
        {
            InitializeComponent();
            vm = new ViewModel();
            vm.grabber_FeedRetrieved += new EventHandler<FeedRetrievedEventArgs>(grabber_FeedRetrieved);
            vm.grabber_FeedError += new EventHandler<FeedErrorEventArgs>(grabber_FeedError);
            ShowSplash();
            ScheduledTaskHelper.ActiveTask();
        }


        private void BindFeeds()
        {
            vm.CheckAlarms();
            if (vm.Feeds != null && vm.Feeds.Count > 0)
            {
                FeedViewOnPage.DataContext = from feeds in vm.Feeds select feeds;
            }
            txtSincro.Text = "Fecha actualización: " + vm.LastUpdate.ToString("dd-MM-yyyy HH:mm:ss");
            HideSplash();
            //ShellTileHelper.UpdateCycleTile((from feeds in vm.Feeds select feeds).ToList());
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            vm.GetFeeds();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (vm.Feeds != null)
            {
                SettingsFeedHelper.SaveFeeds((from f in vm.Feeds select f).ToList(), vm.LastUpdate);
            }
            StateUtilities.IsLaunching = false;
        }

        #region Load Feeds

        //private int counttry = 0;
        void grabber_FeedError(object sender, FeedErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ERROR - " + e.Error.Message);
            HideSplash();
        }

        void grabber_FeedRetrieved(object sender, FeedRetrievedEventArgs e)
        {
            foreach (SyndicationItem item in e.Feed.Items)
            {
                System.Diagnostics.Debug.WriteLine(item.Title.Text);
            }
            vm.SetFeeds(e.Feed.Items.ToArray());
            BindFeeds();
        }
        #endregion

        private void AppBarSave_Click(object sender, EventArgs e)
        {
            SettingsFeedHelper.SaveFeeds((from f in vm.Feeds select f).ToList(), vm.LastUpdate);
        }

        private void AppBarSincro_Click(object sender, EventArgs e)
        {
            ShowSplash();
            DateTime lastUpdateFeed = new DateTime();
            SettingsFeedHelper.UpdateFeeds(grabber_FeedRetrieved, grabber_FeedError, ref lastUpdateFeed);
            vm.LastUpdate = lastUpdateFeed;
        }

    }
}
