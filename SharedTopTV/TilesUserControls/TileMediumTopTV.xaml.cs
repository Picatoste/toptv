using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace SharedTopTV
{
    public partial class TileMediumTopTV : UserControl
    {

        public String Canal
        {
            get { return this.tbCanal.Text; }
            set { this.tbCanal.Text = value; }
        }

        public String Descripcion
        {
            get { return this.tbDesc.Text; }
            set { this.tbDesc.Text = value; }
        }

        public String Fecha
        {
            get { return this.tbFecha.Text; }
            set { this.tbFecha.Text = value; }
        }

        public Brush Background
        {
            get { return this.gContent.Background; }
            set { this.gContent.Background = value; }
        }
 

        public TileMediumTopTV()
        {
            InitializeComponent();
        }
    }
}
