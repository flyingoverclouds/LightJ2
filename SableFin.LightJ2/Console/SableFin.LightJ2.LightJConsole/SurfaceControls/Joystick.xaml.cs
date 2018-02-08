using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
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
    internal sealed partial class Joystick : UserControl
    {
        public Joystick()
        {
            this.InitializeComponent();
        }

        private SliderSurfaceItem _xSI = null;
        private SliderSurfaceItem _ySI = null;
        public SliderSurfaceItem XSurfaceItem
        {
            get { return _xSI; }
            set 
            {
                if (_xSI!=null)
                    _xSI.PropertyChanged -= _xSI_PropertyChanged;
                _xSI = value;
                _xSI.PropertyChanged += _xSI_PropertyChanged;

            }
        }

        public SliderSurfaceItem YSurfaceItem
        {
            get { return _ySI; }
            set
            {
                if (_ySI!=null)
                    _ySI.PropertyChanged -= _ySI_PropertyChanged;
                _ySI = value;
                _ySI.PropertyChanged += _ySI_PropertyChanged;
            }
        }


        void _xSI_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshPositionFromDmx();
        }

        void _ySI_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshPositionFromDmx();
        }

        private uint pointerPressedInArea = 0;
        private void Border_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            pointerPressedInArea = e.Pointer.PointerId;
            brdTouchArea.CapturePointer(e.Pointer);
            brdTouchArea.Background = new SolidColorBrush(Colors.LightGray);
            MoveVirtualJoystick(e);
        }

        private void Border_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == pointerPressedInArea)
                pointerPressedInArea = 0;
            brdTouchArea.ReleasePointerCapture(e.Pointer);
            brdTouchArea.Background = new SolidColorBrush(Colors.DarkGray);
            MoveVirtualJoystick(e);
        }

        private void brdTouchArea_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!e.Pointer.IsInContact && e.Pointer.PointerId!=pointerPressedInArea)
                return;
            MoveVirtualJoystick(e);
        }

        void MoveVirtualJoystick(PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(brdTouchArea).Position;
            if (p.X < 0) p.X = 0;
            if (p.Y < 0) p.Y = 0;
            if (p.X > brdTouchArea.ActualWidth) p.X = brdTouchArea.ActualWidth;
            if (p.Y > brdTouchArea.ActualHeight) p.Y = brdTouchArea.ActualHeight;
            RefreshPosition(p);
        }

        void RefreshPositionFromDmx()
        {
            var x = brdTouchArea.ActualWidth * ( 1.0* (_xSI.DmxValue-_xSI.Min) / (_xSI.Max-_xSI.Min) );
            var y = brdTouchArea.ActualHeight * (1.0*(_ySI.DmxValue - _ySI.Min) / (_ySI.Max - _ySI.Min));
            joystickOffset.X = brdTouchArea.ActualWidth - x;
            joystickOffset.Y = brdTouchArea.ActualHeight - y;
        }

        void RefreshPosition(Point p)
        {
            var dx = (p.X / brdTouchArea.ActualWidth);
            var dy = (p.Y / brdTouchArea.ActualHeight);
            
            joystickOffset.X = p.X;
            joystickOffset.Y = p.Y;
            short dmxValue;

            dx = 1 - dx; // inversion pour compense la tete en bas :)
            dy = 1 - dy;

            if (_xSI != null)
            {
                dmxValue = (short) (_xSI.Min + dx*(_xSI.Max - _xSI.Min));
                _xSI.DmxValue = dmxValue;
            }
            if (_ySI != null)
            {
                dmxValue = (short)(_ySI.Min + dy * (_ySI.Max - _ySI.Min));
                _ySI.DmxValue = dmxValue;
            }
            if(_xSI!=null & _ySI!=null)
                txbPosition.Text = string.Format("{0} , {1}", _xSI.DmxValue, _ySI.DmxValue);
        }
    }
}
