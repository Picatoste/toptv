using System;
using System.Globalization;
using System.Windows;
/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.Windows.Controls;
using System.Windows.Data;
using TopTV.Model;

namespace sdkMVVMCS.View
{
    public partial class FeedView : UserControl
    {
        public FeedView()
        {
            InitializeComponent();
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            bool visibility = (AlarmModel)value != null;
        return visibility ? Visibility.Visible : Visibility.Collapsed;
    }
     
    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        Visibility visibility = (Visibility)value;
        return (visibility == Visibility.Visible);
    }

    }

    public class VisibilityConverterFalse : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            bool visibility = (AlarmModel)value == null;
            return visibility ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return (visibility == Visibility.Visible);
        }

    }

   
}
