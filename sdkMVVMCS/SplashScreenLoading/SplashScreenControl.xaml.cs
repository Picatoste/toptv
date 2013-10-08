using System.Windows.Controls;

namespace SplashLoading
{
    public partial class SplashScreenControl : UserControl
    {
        public SplashScreenControl()
        {
            InitializeComponent();
            this.progressBar1.IsIndeterminate = true;
        }
    }
}
