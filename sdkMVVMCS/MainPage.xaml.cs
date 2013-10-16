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
using WindowsBlogReader;
using SplashLoading;

using System.ServiceModel.Syndication;
using System.Windows.Controls.Primitives;
using System.ComponentModel;


namespace sdkMVVMCS
{
    public partial  class MainPage : PhoneApplicationPageBase
    {
        private ViewModel vm;

        public MainPage()
        {
            InitializeComponent();
            vm = new ViewModel();
            vm.grabber_FeedRetrieved += new EventHandler<FeedRetrievedEventArgs>(grabber_FeedRetrieved);
            vm.grabber_FeedError += new EventHandler<FeedErrorEventArgs>(grabber_FeedError);
            ShowSplash();
        }

        private void BindFeeds()
        {
            if (vm.Feeds != null && vm.Feeds.Count > 0)
            {
                FeedViewOnPage.DataContext = from feeds in vm.Feeds select feeds;
            }

            txtSincro.Text = "Fecha actualización: " + vm.LastUpdate.ToString("dd-MM-yyyy HH:mm:ss");
            HideSplash();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //if (!StateUtilities.IsLaunching && this.State.ContainsKey("Feeds"))
            //{
            //    vm = (ViewModel)this.State["Feeds"];
            //    BindFeeds();
            //}
            //else
            //{
                vm.GetFeeds();
            //}
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            vm.SaveFeeds();

            //if (this.State.ContainsKey("Feeds"))
            //{
            //    this.State["Feeds"] = vm;
            //}
            //else
            //{
            //    this.State.Add("Feeds", vm);
            //}
            StateUtilities.IsLaunching = false;
        }

        #region Load Feeds

        private int counttry = 0;
        void grabber_FeedError(object sender, FeedErrorEventArgs e)
        {
            counttry++;
            MessageBox.Show("Error download feeds. Try download intention " + counttry + " of 5 try. " + e.Error.Message);
            if (counttry < 5){
                vm.GetFeeds();
            }else
            {
                HideSplash();
            }
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
            vm.SaveFeeds();
        }

        private void AppBarSincro_Click(object sender, EventArgs e)
        {
            ShowSplash();
            vm.UpdateFeeds();
        }

    }
}
