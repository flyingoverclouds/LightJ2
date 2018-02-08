using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using SableFin.LightJ2.SurfaceFramework;

namespace SableFin.LightJ2.SurfaceControls
{
    public sealed partial class PushButton : UserControl
    {
        private PushSurfaceItem pushSurfaceItem = null;
        bool pressed = false;

        public PushButton()
        {
            this.InitializeComponent();
            this.DataContextChanged += PushButton_DataContextChanged;
        }

        void PushButton_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.pushSurfaceItem = args.NewValue as PushSurfaceItem;
            if (this.pushSurfaceItem!=null && !string.IsNullOrEmpty(this.pushSurfaceItem.ImageResourceName))
            {
                imgPushArea.Background = Application.Current.Resources[this.pushSurfaceItem.ImageResourceName] as Brush;
                imgPushArea.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void pushButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            pressed = true;
            ledGreen.Visibility = Visibility.Visible;
            pushArea.Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
            if (pushSurfaceItem != null)
                pushSurfaceItem.IsPressed = true;
        }

        private void pushButton_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            pressed = false;
            ledGreen.Visibility = Visibility.Collapsed;
            pushArea.Background = null;
            if (pushSurfaceItem != null)
                pushSurfaceItem.IsPressed = false;
        }

        private void pushButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            pressed = false;
            ledGreen.Visibility = Visibility.Collapsed;
            pushArea.Background = null;
            if (pushSurfaceItem != null)
                pushSurfaceItem.IsPressed = false;
        }

    }
}
