using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.System.Threading;

namespace SableFin.LightJ2.SurfaceFramework
{
    class TimeDmxClockEngine
    {
        static TimeDmxClockEngine()
        {
            _instance = new TimeDmxClockEngine(); 
        }

        static TimeDmxClockEngine _instance  = null;

        public static TimeDmxClockEngine Current
        {
            get { return _instance; }
        }


        private const int nbComputePerSecond = 40;
        private Dictionary<string,ITimerDmxGenerator> _generators = new Dictionary<string, ITimerDmxGenerator>();
        //public void SetGeneratorsCollection(Dictionary<string, ITimerDmxGenerator>  generators)
        //{
        //    if (generators != null)
        //        _generators = generators;
        //}


        public void AddGenerator(string generatorId, ITimerDmxGenerator generator)
        {
            _generators.Add(generatorId,generator); 
        }

        //private Thread currentThread = null;
        public void Start()
        {
            ThreadPool.RunAsync(delegate { DmxClockRunThread(); });
        }

        public void SetBpm(short bpm)
        {
            foreach (var timerDmxGenerator in _generators.Values)
            {
                timerDmxGenerator.SetBpm(bpm);
            }
        }

        public void SetParameterValue(string generatorID,String parameterName, string value)
        {
            _generators[generatorID].SetParameterValue(parameterName,value);
        }
        async void DmxClockRunThread()
        {
            int pauseBetweenCompute = 1000 / nbComputePerSecond;
            
            DateTime startTime = DateTime.Now;
            long currentms;
            while (true)
            {
                currentms = Convert.ToInt64((DateTime.Now - startTime).TotalMilliseconds);

                foreach (var timerDmxGenerator in _generators.Values)
                {
                    if (timerDmxGenerator.IsActive)
                        timerDmxGenerator.FunctionOfTime(currentms);
                }
                //Debug.WriteLine("DMX time generaor : {0}" , currentms);
                await Task.Delay(pauseBetweenCompute);
            }
        }
    }
}
