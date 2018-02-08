using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SableFin.LightJ2.SurfaceFramework;

namespace SableFin.LightJ2.DmxGenerators
{
    class RandomPerBeatGenerator : ITimerDmxGenerator
    {
        private short dmxPanChanel = 0;
        private short dmxTiltChannel = 0;
        private short _currentBpm = 60; // by defaault 60bpm
        private short _minX;
        private short _maxX;
        private short _minY;
        private short _maxY;


        private bool _lockX = false;
        private bool _lockY = false;
        private bool _lockGobo = false;

        public RandomPerBeatGenerator(short dmxPanChannel,short dmxTiltChannel, short minX = 0, short maxX = 255, short minY = 0, short maxY = 255)
        {
            this.dmxPanChanel = dmxPanChannel;
            this.dmxTiltChannel = dmxTiltChannel;
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;
            IsActive = false;
        }

        Random rnd = new Random((int)DateTime.Now.Ticks);

        public void SetParameterValue(string name, string value)
        {
            switch (name.ToLower())
            {
                case "lockx":
                    _lockX = (value == "1");
                    break;
                case "locky":
                    _lockY = (value == "1");
                    break;
                case "lockgobo":
                    _lockGobo = (value == "1");
                    break;
            }
        }

        public bool IsActive
        {
            get;
            set;
        }



        public void SetBpm(short currentBpm)
        {
            _currentBpm = currentBpm;
            cyclePeriod = (short)((60 * 1000) / currentBpm);
            previousTime = -1;
        }
        private short cyclePeriod = 1000; // Default for 60bpm --> une iteration complete dure 1000 ms;

        private long previousTime = -1;
        private short originX;
        private short originY;
        private short targetX;
        private short targetY;

        public void FunctionOfTime(long currentTime)
        {
            if (previousTime == -1 || (currentTime - previousTime >= cyclePeriod))
            {
                previousTime = currentTime;
                originX = targetX;
                originY = targetY;

                targetX =  (short)(_minX + rnd.Next(_maxX - _minX));
                targetY = (short)(_minY + rnd.Next(_maxY - _minY));

                //Debug.WriteLine("*** TARGET -> x={0}   y={1}", targetX, targetY);
            }

            // position sur la tranche de temps ( de 0.0 à 1.0 ) : 0=position orogin, 1=position target
            var pos  = 1.0 * (currentTime - previousTime)/cyclePeriod;

            byte currentPan =(byte)( originX + (targetX - originX) * pos);
            byte currentTilt = (byte)(originY + (targetY - originY) * pos);

            //Debug.WriteLine("    x={0}   y={1}",currentX,currentY);

            // HACK : hardcode value to target fixture

            if (!_lockX)
            {
                FastRoutingMatrix.Current.SetCell(-1, currentPan, this.dmxPanChanel); // TODO : Add support of XML configuration
            }
            if (!_lockY)
            {
                FastRoutingMatrix.Current.SetCell(-1, currentTilt, this.dmxTiltChannel); // TODO : Add support of XML configuration
            }

        }
    }
}
