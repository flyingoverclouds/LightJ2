using SableFin.LightJ2.SurfaceFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace SableFin.LightJ2.LightJConsole.SurfaceControls
{
    public sealed partial class BindingReverserButton : UserControl
    {
        private ValueReverserSurfaceItem brSurfaceItem = null;

        public BindingReverserButton()
        {
            this.InitializeComponent();
            this.DataContextChanged += OnOffButton_DataContextChanged;
        }

        void OnOffButton_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.brSurfaceItem = args.NewValue as ValueReverserSurfaceItem;
            if (this.brSurfaceItem != null && !string.IsNullOrEmpty(this.brSurfaceItem.ImageResourceName))
            {
                imgPushArea.Background = Application.Current.Resources[this.brSurfaceItem.ImageResourceName] as Brush;
                imgPushArea.Visibility = Visibility.Visible;
            }
        }


        bool pressed = false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void pushButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            pressed = !pressed;
            if (pressed)
            {
                ledGreen.Visibility = Visibility.Visible;
                if (brSurfaceItem != null)
                    brSurfaceItem.IsReverse = true;
            }
            else
            {
                ledGreen.Visibility = Visibility.Collapsed;
                if (brSurfaceItem != null)
                    brSurfaceItem.IsReverse = false;
            }
            pushArea.Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
        }

        private void pushButton_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            pushArea.Background = null;
        }


        private void pushButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            pushArea.Background = null;
        }
    }
}
