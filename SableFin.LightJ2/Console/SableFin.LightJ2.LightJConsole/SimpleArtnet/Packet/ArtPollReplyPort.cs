using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SableFin.LightJ2.LightJConsole.SimpleArtnet.Packet
{
    enum PortProtocol
    {
        Dmx512=0,
        Midi = 1,
        Avab=2,
        ColortranCMX=3,
        ADB62_5=4,
        ArtNet=5
    }

    [Flags]
    enum PortType
    {
        ArtnetOutput=128,
        ArtnetInput=64
    }
        

    [Flags]
    enum PortInputs
    {
        Unused1 = 1,
        Unused2 = 2,
        ReceiveErrorsDectected = 4,
        Disabled = 8,
        Dmx512TextSupported = 16,
        Dmx512SIPSupported =32,
        Dmx512TestSupported =64,
        DataReceived = 128
    }

    [Flags]
    enum PortOutputs
    {
        Unused = 1,
        MergeModeIsLTP=2,
        DmxOutputShortCircuit=4,
        MergingArtnetData =8,
        Dmx512TextSupported=16,
        Dmx512SIPSupported = 32,
        Dmx512TestSupported = 64,
        DataTransmitted = 128
    }

    class ArtPollReplyPort
    {
        PortType _portType;
        PortInputs _input = 0;
        PortOutputs _output = 0;
        byte _inputUniverse = 0;
        byte _outputUniverse = 0;

        public ArtPollReplyPort(byte portType,byte goodInput, byte goodOutput,byte swIn, byte swOut)
        {
            _portType = (PortType)portType;
            if ((_portType & PortType.ArtnetOutput) ==PortType.ArtnetOutput )
                this.IsArnetOutput = true;

            if ((_portType & PortType.ArtnetInput) == PortType.ArtnetInput)
                this.IsArtnetInput = true;

            var prot = (PortProtocol)(portType & 63);

            this._input = (PortInputs)goodInput;
            this._output = (PortOutputs)goodOutput;
            this._inputUniverse = (byte)(swIn & 0x0F);
            this._outputUniverse = (byte)(swOut & 0x0F);
        }
        public bool IsArnetOutput { get; set; } = false;
        public bool IsArtnetInput { get; set; } = false;
        public PortProtocol Protocol { get; set; } = PortProtocol.Dmx512;

       

        public bool InputDataReceived { get { return (_input & PortInputs.DataReceived)==PortInputs.DataReceived; }  } 
        public bool InputDmx512TestPacket { get { return (_input & PortInputs.Dmx512TestSupported ) == PortInputs.Dmx512TestSupported ; } }
        public bool InputDmx512SIP { get { return (_input & PortInputs.Dmx512SIPSupported ) == PortInputs.Dmx512SIPSupported ; } } 
        public bool InputDmx512Text { get { return (_input & PortInputs.Dmx512TextSupported) == PortInputs.Dmx512TextSupported; } }
        public bool InputDisabled { get { return (_input & PortInputs.Disabled) == PortInputs.Disabled; } } 
        public bool InputReceivedErrorDetected { get { return (_input & PortInputs.ReceiveErrorsDectected) == PortInputs.ReceiveErrorsDectected; } }


        public bool OutputDataTransmitted { get { return (_output & PortOutputs.DataTransmitted) == PortOutputs.DataTransmitted; } }
        public bool OutputDmx512TestPacket { get { return (_output & PortOutputs.Dmx512TestSupported) == PortOutputs.Dmx512TestSupported; } }
        public bool OutputDmx512SIP { get { return (_output & PortOutputs.Dmx512SIPSupported) == PortOutputs.Dmx512SIPSupported; } }
        public bool OutputDmx512Text { get { return (_output & PortOutputs.Dmx512TextSupported) == PortOutputs.Dmx512TextSupported; } }
        public bool OutputMergeModeIsLTP { get { return (_output & PortOutputs.MergeModeIsLTP) == PortOutputs.MergeModeIsLTP; } }
        public bool OutputMergArtnetData { get { return (_output & PortOutputs.MergingArtnetData) == PortOutputs.MergingArtnetData; } }

        public bool DmxOutputShortCircuit { get { return (_output & PortOutputs.DmxOutputShortCircuit) == PortOutputs.DmxOutputShortCircuit; } }


        public byte InputUniverse { get { return _inputUniverse; } }
        public byte OutputUniverse { get { return _outputUniverse; } }

    }
}
