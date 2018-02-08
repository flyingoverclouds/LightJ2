using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SableFin.LightJ2.SurfaceFramework;

namespace SableFin.LightJ2.DmxGenerators
{
    class CosinusDmxGenerator : ITimerDmxGenerator
    {
        private short _currentBpm = 60; // by defaault 60bpm
        private short _minValue;
        private short _maxValue;
        public CosinusDmxGenerator(short minValue = 0, short maxValue = 255)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            IsActive = false;
        }

        public void SetBpm(short currentBpm)
        {
            _currentBpm = currentBpm;
            cyclePeriod = (short)(2 * (60 * 1000) / currentBpm);
        }

        private short cyclePeriod = 2000; // Default for 60bpm : 2 bpm for 1 cycle

        Random rnd=new Random((int)DateTime.Now.Ticks);
        private double randomPower = 1.0;


        private long computeTime;
        public void FunctionOfTime(long currentTime)
        {
            computeTime = currentTime % cyclePeriod;
            if (computeTime < 150)
                randomPower = rnd.NextDouble();

            var r = (2.0 * computeTime / cyclePeriod * Math.PI) - Math.PI; // OK
            var t = Math.Cos(r) * 0.5 * (_maxValue - _minValue); // OK

            var v = (byte)(_minValue + (_maxValue - _minValue) / 2.0 + t*randomPower);

            //Debug.WriteLine("   -- f({0}) = {1}", computeTime, v);
            FastRoutingMatrix.Current.SetCell(-1, v, 98); // TODO : Add support of XML configuration
        }

        public bool IsActive
        {
            get;
            set;
        }


        public void SetParameterValue(string name, string value)
        {
          
        }
    }
}
