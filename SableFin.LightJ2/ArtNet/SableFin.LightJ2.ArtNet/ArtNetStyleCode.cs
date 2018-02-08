using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.ArtNet
{
    public sealed class ArtNetStyleCode
    {
        /// <summary>
        /// A DMX to / from Art-Net device 
        /// </summary>
        const byte StNode = 0x00;

        /// <summary>
        /// lightning console
        /// </summary>
        const byte StControler = 0x01;

        /// <summary>
        /// A media server
        /// </summary>
        const byte StMedia = 0x02;

        /// <summary>
        /// a network routing device
        /// </summary>
        const byte StRoute = 0x03;

        /// <summary>
        /// A backup device
        /// </summary>
        const byte StBackup = 0x04;

        /// <summary>
        /// a configuration or diagnostic tool
        /// </summary>
        const byte StConfig = 0x05;

        /// <summary>
        /// A visualiser
        /// </summary>
        const byte StVisual = 0x06;
    }
}
