using Microsoft.Phone.Controls;
using System.Windows.Controls.Primitives;

namespace SplashLoading
{

    public partial class PhoneApplicationPageBase : PhoneApplicationPage
    {
        private Popup popup;
        #region LoadingSplash

        protected void ShowSplash()
        {
            this.ApplicationBar.IsVisible = false;
            this.popup = new Popup();
            this.popup.Child = new SplashScreenControl();
            this.popup.IsOpen = true;
        }

        protected void HideSplash()
        {
            this.Dispatcher.BeginInvoke(() =>
            {

                this.ApplicationBar.IsVisible = true;
                this.popup.IsOpen = false;

            }
            );
        }
        #endregion
    }
}