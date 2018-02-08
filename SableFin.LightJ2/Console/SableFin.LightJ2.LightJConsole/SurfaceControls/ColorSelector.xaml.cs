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
    internal sealed partial class ColorSelector : UserControl
    {
        public ColorSelector()
        {
            this.InitializeComponent();
        }

        private SliderSurfaceItem _redSI = null;
        private SliderSurfaceItem _greenSI = null;
        private SliderSurfaceItem _blueSI = null;


        public SliderSurfaceItem RedSurfaceItem
        {
            get { return _redSI; }
            set
            {
                if (_redSI != null)
                    _redSI.PropertyChanged -= _redSI_PropertyChanged;
                _redSI = value;
                _redSI.PropertyChanged += _redSI_PropertyChanged;

            }
        }

        public SliderSurfaceItem GreenSurfaceItem
        {
            get { return _greenSI; }
            set
            {
                if (_greenSI != null)
                    _greenSI.PropertyChanged -= _greenSI_PropertyChanged;
                _greenSI = value;
                _greenSI.PropertyChanged += _greenSI_PropertyChanged;
            }
        }

        public SliderSurfaceItem BlueSurfaceItem
        {
            get { return _blueSI; }
            set
            {
                if (_blueSI != null)
                    _blueSI.PropertyChanged -= _blueSI_PropertyChanged;
                _blueSI = value;
                _blueSI.PropertyChanged += _blueSI_PropertyChanged;
            }
        }


        void _redSI_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshPositionFromDmx();
        }

        void _greenSI_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshPositionFromDmx();
        }

        void _blueSI_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshPositionFromDmx();
        }


     
      

        void RefreshPositionFromDmx()
        {
            // TODO : Get the the dmx value and move the pointer to the righ position

            //var red = brdTouchArea.ActualWidth * (1.0 * (_redSI.DmxValue - _redSI.Min) / (_redSI.Max - _redSI.Min));
            //var green = brdTouchArea.ActualHeight * (1.0 * (_greenSI.DmxValue - _greenSI.Min) / (_greenSI.Max - _greenSI.Min));
            //joystickOffset.X = brdTouchArea.ActualWidth - red;
            //joystickOffset.Y = brdTouchArea.ActualHeight - green;
        }

      
        private uint pointerPressedInArea = 0;

        private void imgTriangleRVB_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            pointerPressedInArea = e.Pointer.PointerId;
            imgTriangleRVB.CapturePointer(e.Pointer);
            MoveVirtualJoystick(e);
        }

     
        private void Image_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!e.Pointer.IsInContact && e.Pointer.PointerId != pointerPressedInArea)
                return;
            MoveVirtualJoystick(e);
        }

        private void imgTriangleRVB_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == pointerPressedInArea)
                pointerPressedInArea = 0;
            imgTriangleRVB.ReleasePointerCapture(e.Pointer);
            ellTouched.Stroke = new SolidColorBrush(Colors.Transparent);
            MoveVirtualJoystick(e);
        }

        void MoveVirtualJoystick(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(imgTriangleRVB).Position;
            if (p.X < 0) p.X = 0;
            if (p.Y < 0) p.Y = 0;
            if (p.X > imgTriangleRVB.ActualWidth) p.X = imgTriangleRVB.ActualWidth;
            if (p.Y > imgTriangleRVB.ActualHeight) p.Y = imgTriangleRVB.ActualHeight;
            RefreshPosition(p);
        }

        void RefreshPosition(Point p)
        {
            double center = imgTriangleRVB.ActualWidth / 2;
            var rayon = Math.Sqrt((p.X - center) * (p.X - center) + (p.Y - center) * (p.Y - center));

            if (rayon > center) // le curseur est hors du cercle
            {
                //txbInOut.Text = "OUT";
                return;
            }
            //txbInOut.Text = "IN";
            ttfCursor.X = p.X;
            ttfCursor.Y = p.Y;

            var distHoriz = p.X - center; // r * cos(angle)
            var distVert = -(p.Y - center); // r * sin(angle)
            //txbRVB.Text = $"{distHoriz}    {distVert}";
            double cosAngle = 0;
            if (rayon != 0)
            {
                cosAngle = distHoriz / rayon;
            }

            //txbRVB.Text = $"Cos(angle) = {cosAngle}";
            var angleRad = Math.Acos(cosAngle);
            if (distVert < 0)
                angleRad = (Math.PI - angleRad) + Math.PI;

            var angleDeg = angleRad * (180.0 / Math.PI);


            var h = angleDeg;
            var s = (rayon / (center - 1));
            var v = 1.0; // TODO : add a slider for V value

            //txbPos.Text = $"H={h}  S={s}   V={v}";

            byte r, g, b;
            ColorHSVToRGB(h, s, v, out r, out g, out b);
            //txbRGB.Text = $"R={r}   G={g}   B={b}";
            ellTouched.Stroke = new SolidColorBrush(Color.FromArgb(255, r, g, b));

            short dmxValue;
            if (_redSI != null)
            {
                dmxValue = (short)(_redSI.Min + (r / 255.0) * (_redSI.Max - _redSI.Min));
                _redSI.DmxValue = dmxValue;
            }
            if (_greenSI != null)
            {
                dmxValue = (short)(_greenSI.Min + (g / 255.0) * (_greenSI.Max - _greenSI.Min));
                _greenSI.DmxValue = dmxValue;
            }
            if (_blueSI != null)
            {
                dmxValue = (short)(_blueSI.Min + (b / 255.0) * (_blueSI.Max - _blueSI.Min));
                _blueSI.DmxValue = dmxValue;
            }
           
        }



        public void ColorHSVToRGB(double h, double s, double v, out byte red, out byte green, out byte blue)
        {

            double r = 0;
            double g = 0;
            double b = 0;

            if (s == 0)
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                int i;
                double f, p, q, t;

                if (h == 360)
                    h = 0;
                else
                    h = h / 60;

                i = (int)Math.Truncate(h);
                f = h - i;

                p = v * (1.0 - s);
                q = v * (1.0 - (s * f));
                t = v * (1.0 - (s * (1.0 - f)));

                switch (i)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;

                    default:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }

            }
            red = (byte)(r * 255);
            green = (byte)(g * 255);
            blue = (byte)(b * 255);
        }


    }
}
