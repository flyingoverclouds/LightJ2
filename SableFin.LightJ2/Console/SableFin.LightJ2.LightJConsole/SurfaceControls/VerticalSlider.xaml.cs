using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class VerticalSlider : UserControl
    {
        public VerticalSlider()
        {
            this.InitializeComponent();
            this.DataContextChanged += VerticalSlider_DataContextChanged;
        }

        void VerticalSlider_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var ssi = args.NewValue as SliderSurfaceItem;
            if (ssi == null)
                return;
            sld.Minimum = ssi.Min;
            sld.Maximum = ssi.Max;

            if (ssi != null && !string.IsNullOrEmpty(ssi.ImageResourceName))
            {
                imgPushArea.Background = Application.Current.Resources[ssi.ImageResourceName] as Brush;
                imgPushArea.Visibility = Visibility.Visible;
            }
        }

    }
}
