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

namespace SableFin.LightJ2.SurfaceControls
{
    public sealed partial class TouchBpmArea : UserControl
    {
        public TouchBpmArea()
        {
            this.InitializeComponent();
        }

        List<int> lastBpms = new List<int>();
        private int bpm = 0;
        private long lastTick = 0;
        private long currentTick = 0;
        TimeSpan ts;

        private void Border_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            lastTick = currentTick;
            currentTick = DateTime.Now.Ticks;
            bdrTouchArea.Background = new SolidColorBrush(Colors.DarkSeaGreen);
            if (lastTick == 0)
            {
                pgReliability.Value = pgReliability.Maximum;
                lastBpms.Clear();
                txbBpm.Text = "";
                return;
            }
            ts = new TimeSpan(currentTick - lastTick);
            bpm = (int)(60000 / ts.TotalMilliseconds);
            if (bpm < 30)
            {
                pgReliability.Value = pgReliability.Maximum;
                lastBpms.Clear();
                lastTick = 0;
                txbBpm.Text = "";
                return;
            }
            lastBpms.Add(bpm);
            bpm = (int)lastBpms.Average();
            RefreshDisplay();
        }

        void RefreshDisplay()
        {

            pgReliability.Value = pgReliability.Maximum - ((lastBpms.Count < pgReliability.Maximum) ? lastBpms.Count : pgReliability.Maximum);
            txbBpm.Text = bpm.ToString();
            if (pgReliability.Value > 7)
                TimeDmxClockEngine.Current.SetBpm((short) bpm);
        }

        private void Border_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            bdrTouchArea.Background = new SolidColorBrush(Colors.Black);
        }

        private void butBpmDec_Click(object sender, RoutedEventArgs e)
        {
            if (bpm < 10)
                return;
            bpm--;
            RefreshDisplay();
        }

        private void butBpmInc_Click(object sender, RoutedEventArgs e)
        {
            bpm++;
            RefreshDisplay();
        }

    }
}
