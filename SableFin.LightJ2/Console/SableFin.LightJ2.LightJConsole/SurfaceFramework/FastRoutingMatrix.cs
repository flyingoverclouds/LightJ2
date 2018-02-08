/*
 * FastRoutingMatrix - (c) SableFin 2014-2016
 * 
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SableFin.LightJ2.Network;

namespace SableFin.LightJ2.SurfaceFramework
{

    /// <summary>
    /// Implémentation de la matric de routage DMX
    /// </summary>
    class FastRoutingMatrix
    {
        private static FastRoutingMatrix _instance;
        public static FastRoutingMatrix Current
        {
            get { return _instance; }
            set { _instance = value; }
        }

        private DmxServiceProxy dmxServiceProxy;
        public FastRoutingMatrix(DmxServiceProxy dmxProxy,IList<SurfaceItem>  items)
        {
            this.dmxServiceProxy = dmxProxy;
            InitializeMatrix(items);
        }

        private SurfaceItem[] surfaceItems;

        private byte[] dmxChannelsValue=new byte[512]; // tableau contenant les valeurs des 512 canaux DMX

        private short[][] routingIdsPerChannel; // ex : routingIdsPerChannel[10] renvoi le tableau des routingId abonné au channel 10


        void InitializeMatrix(IList<SurfaceItem> items)
        {
            // creation d'une liste temporaire pour la création de la table de routage
            List<short>[] tempRoutingTable = new List<short>[512];
            for(int channel=0;channel<tempRoutingTable.Length;channel++)
                tempRoutingTable[channel]=new List<short>();


            surfaceItems=new SurfaceItem[items.Count];

            short fastRoutingIdGenerator = 0;
            foreach (var item in items)
            {
                surfaceItems[fastRoutingIdGenerator] = item; // on mémorise le surfaceItem. sa position dans le tablea est le FastRoutingId
                item.FastRoutingId = fastRoutingIdGenerator; // on lui affection sont Id de routing

                var subscribedChannels = item.GetSubscribedDmxChannel();
                foreach (var subscribedChannel in subscribedChannels)
                {
                    tempRoutingTable[subscribedChannel].Add(fastRoutingIdGenerator); // on mémorise l'abonne d'un surface item à un channel DMX
                }
                fastRoutingIdGenerator++; // on incremente l'id de routing
            }

            routingIdsPerChannel = new short[512][]; // allocation des 512 canaux DMX
            for (int n = 0; n < 512; n++)
            {
                // pour chaque canal DMX, on alloue le tableau pour stoker les Id des SurfaceItem abonné
                routingIdsPerChannel[n] = tempRoutingTable[n].ToArray();
            }

            // La matrice de routage est prete :)
        }


        /// <summary>
        /// Cette méthoide est utilisé par les SurfaceItem pour positioner une valeur sur 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        public async Task SetCells(int senderFastRoutingId, byte value, short[] channels)
        {
            for (int n = 0; n < channels.Length; n++)  // pour chaque canal ciblé
            {

                // on met à jour la valeur des canaux DMX ciblé.
                dmxChannelsValue[channels[n]] = value;
                SendDmxMessage(channels[n],value); // on emet la trame DMX vers l'agent openusb

                // pour chaque canal DMX ciblé, on récpercute la modification de la valeur aux surfaceItem abonnés
                var propagateIds = routingIdsPerChannel[channels[n]];
                for (int i = 0;i< propagateIds.Length; i++)
                {
                    if (propagateIds[i] == senderFastRoutingId) // on ne renvoie pas à l'emmeteur la nouvelle valeur
                        continue;

                    //Debug.WriteLine("set " + surfaceItems[propagateIds[i]].Text + " to " + value);
                    surfaceItems[propagateIds[i]].SetDmxValue(new FastRoutingFrame() { SourceFRId = senderFastRoutingId,Value = value }); // on affecte la nouvelle valeur au surface item

                }
            }

        }

        /// <summary>
        /// Cette méthoide est utilisé par les SurfaceItem pour positioner une valeur sur 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        public async Task SetCells(int senderFastRoutingId, byte value, List<DmxBinding> channels)
        {
            byte valueToSend = value;
            for (int n = 0; n < channels.Count; n++)  // pour chaque canal ciblé
            {
                if (channels[n].Reverse)
                {
                    value = (byte) ( channels[n].DmxMax - (value - channels[n].DmxMin) );
                }
                // on met à jour la valeur des canaux DMX ciblé.
                dmxChannelsValue[channels[n].Channel] = value;
                SendDmxMessage(channels[n].Channel, value); // on emet la trame DMX vers l'agent openusb

                // pour chaque canal DMX ciblé, on récpercute la modification de la valeur aux surfaceItem abonnés
                var propagateIds = routingIdsPerChannel[channels[n].Channel];
                for (int i = 0; i < propagateIds.Length; i++)
                {
                    if (propagateIds[i] == senderFastRoutingId) // on ne renvoie pas à l'emmeteur la nouvelle valeur
                        continue;

                    //Debug.WriteLine("set " + surfaceItems[propagateIds[i]].Text + " to " + value);
                    surfaceItems[propagateIds[i]].SetDmxValue(new FastRoutingFrame() { SourceFRId = senderFastRoutingId, Value = value }); // on affecte la nouvelle valeur au surface item

                }
            }

        }

        public async Task SetCell(int senderFastRoutingId, byte value, short channel)
        {
            // on met à jour la valeur des canaux DMX ciblé.
            dmxChannelsValue[channel] = value;
            SendDmxMessage(channel, value); // on emet la trame DMX vers l'agent openusb

            // pour chaque canal DMX ciblé, on récpercute la modification de la valeur aux surfaceItem abonnés
            var propagateIds = routingIdsPerChannel[channel];
            for (int i = 0; i < propagateIds.Length; i++)
            {
                if (propagateIds[i] == senderFastRoutingId) // on ne renvoie pas à l'emmeteur la nouvelle valeur
                    continue;
                surfaceItems[propagateIds[i]].SetDmxValue(new FastRoutingFrame() { SourceFRId = senderFastRoutingId,Value=value}); // on affecte la nouvelle valeur au surface item
            }
       }



        async Task SendDmxMessage(short channel,byte value)
        {

            dmxBuffer[channel-1] = value; // set the value in the buffer for ArnetSend

            if (dmxServiceProxy != null)
                dmxServiceProxy.SetChannelValue(channel, value);
        }

        #region DMX 512 buffer management (only 1 universe !)
        byte[] dmxBuffer = new byte[512]; // allocation du buffer Dmx

        public void CopyDmxBufferTo(byte[] targetBuf,int offset)
        {
            dmxBuffer.CopyTo(targetBuf, offset);
        }

        #endregion





    }
}
