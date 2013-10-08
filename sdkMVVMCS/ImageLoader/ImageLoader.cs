using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;

namespace ImageLoader
{
    public sealed partial class ImageLoader : UserControl
    {
        public static DependencyProperty IsLoadedProperty =
          DependencyProperty.Register("IsLoaded",
            typeof(bool),
            typeof(ImageLoader), new PropertyMetadata(false));

        public static DependencyProperty IsLoadingProperty =
          DependencyProperty.Register("IsLoading",
            typeof(bool),
            typeof(ImageLoader), new PropertyMetadata(false));

        public static DependencyProperty IsFailedProperty =
          DependencyProperty.Register("IsFailed",
            typeof(bool),
            typeof(ImageLoader), new PropertyMetadata(false));

        public static DependencyProperty LoadingContentProperty =
          DependencyProperty.Register("LoadingContent",
            typeof(object),
            typeof(ImageLoader), null);

        public static DependencyProperty FailedContentProperty =
          DependencyProperty.Register("FailedContent",
            typeof(object),
            typeof(ImageLoader), null);

        public static DependencyProperty SourceProperty =
          DependencyProperty.Register("Source",
            typeof(ImageSource),
            typeof(ImageLoader),
            new PropertyMetadata(null, OnSourceChanged));

        public ImageLoader()
        {
            this.InitializeComponent();
        }
        public bool IsLoading
        {
            get
            {
                return ((bool)base.GetValue(IsLoadingProperty));
            }
            set
            {
                base.SetValue(IsLoadingProperty, value);
            }
        }
        public bool IsLoaded
        {
            get
            {
                return ((bool)base.GetValue(IsLoadedProperty));
            }
            set
            {
                base.SetValue(IsLoadedProperty, value);
            }
        }
        public bool IsFailed
        {
            get
            {
                return ((bool)base.GetValue(IsFailedProperty));
            }
            set
            {
                base.SetValue(IsFailedProperty, value);
            }
        }
        public object LoadingContent
        {
            get
            {
                return (base.GetValue(LoadingContentProperty));
            }
            set
            {
                base.SetValue(LoadingContentProperty, value);
            }
        }
        public object FailedContent
        {
            get
            {
                return (base.GetValue(FailedContentProperty));
            }
            set
            {
                base.SetValue(FailedContentProperty, value);
            }
        }
        public ImageSource Source
        {
            get
            {
                return ((ImageSource)base.GetValue(SourceProperty));
            }
            set
            {
                base.SetValue(SourceProperty, value);
            }
        }
        static void OnSourceChanged(DependencyObject sender,
          DependencyPropertyChangedEventArgs args)
        {
            ImageLoader loader = (ImageLoader)sender;
            loader.IsFailed = false;
            loader.IsLoaded = false;
            loader.IsLoading = true;
        }
        void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            this.IsLoading = false;
            this.IsFailed = true;
        }
        void OnImageOpened(object sender, RoutedEventArgs e)
        {
            this.IsLoading = false;
            this.IsLoaded = true;
        }

      
    }

}

namespace Common
{

    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }

    }
    

}

