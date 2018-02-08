using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.SurfaceFramework
{
    interface ITimerDmxGenerator
    {
        void SetParameterValue(string name, string value);
        bool IsActive { get; set; }
        void SetBpm(short currentBpm);
        void FunctionOfTime(long currentTime); // 
    }
}
