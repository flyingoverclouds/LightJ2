using Newtonsoft.Json;
using SableFin.LightJ2.SurfaceFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace SableFin.LightJ2.DmxGenerators
{
    class SequencePlayer : ITimerDmxGenerator, IDisposable
    {
        MediaElement currentSound = null;
        SequenceDefinition sequence = null;
        public SequencePlayer(string sequenceFileName)
        {
            Task.Run(async () => {
                await LoadSequence(sequenceFileName);
            });


            // compiling JSon in native data to play


        }

        async Task<bool> LoadSequence(string sequenceFileNAme)
        {
            try
            {
                var folder = KnownFolders.MusicLibrary;
                var file = await folder.GetFileAsync(sequenceFileNAme);
                string seqJson = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);

                var jtr = new JsonTextReader(new StringReader(seqJson));
                JsonSerializer serializer = new JsonSerializer();
                sequence = serializer.Deserialize<SequenceDefinition>(jtr);

                System.Diagnostics.Debug.WriteLine($"Loading sequence UID : {sequence.uid}");
                System.Diagnostics.Debug.WriteLine($"Loading sequence Name : {sequence.displayname}");
                System.Diagnostics.Debug.WriteLine($"Loading sequence MP3 : {sequence.soundfile}");
                await LoadMedia(sequence.soundfile);


                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EXCEPTION : " + ex.ToString());
                // TODO : generate a error message to user
                return false;
            }
        }

        async Task LoadMedia(string mediaFileName)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                async () =>
                {
                    var folder = KnownFolders.MusicLibrary;
                    var file = await folder.GetFileAsync(mediaFileName);
                    currentSound = new MediaElement();
                    currentSound.AutoPlay = false;
                    currentSound.SetSource(await file.OpenAsync(FileAccessMode.Read), file.ContentType);
                });
        }


        public void SetBpm(short currentBpm)
        {
            // don't care about BPM
        }

        int nextSequenceStepIndex = 0;
        bool isPlayingSequence = false;
        long startTime = 0;
        long lastRequestedTime = 0;
        public void FunctionOfTime(long currentTime)
        {
            lastRequestedTime = currentTime;
            if (sequence == null) // si aucune sequence chargé, on return;
                return;
            if (sequence.sequences == null) // si pas de sequences --> return;
                return;
            //currentTime is ms
            if (!isPlayingSequence) // si la sequence n'est pas en train de jouer -> return
                return;

            if (nextSequenceStepIndex >= sequence.sequences.Count) // si fin de sequence attention, on arret de repondre au demande
            {
                isPlayingSequence = false;
                return;
            }

            while (sequence.sequences[nextSequenceStepIndex].time <= currentTime - startTime)
            {
                foreach (var dmxv in sequence.sequences[nextSequenceStepIndex].values)
                {
                    FastRoutingMatrix.Current.SetCell(-1, dmxv.dmxValue, dmxv.dmxChannel);
                }
                nextSequenceStepIndex++;
                if (nextSequenceStepIndex >= sequence.sequences.Count)
                    break;
            }



            //short channel = (short)(currentTime%512);
            //byte value = (byte)(currentTime%255);
            //FastRoutingMatrix.Current.SetCell(0, value,channel);


        }

        bool _isActive = true;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }


        void PlaySound()
        {
            Task.Run(async () => {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                   () =>
                   {
                       startTime = lastRequestedTime;
                       nextSequenceStepIndex = 0;
                       currentSound?.Play();
                   });
            });
            isPlayingSequence = true;
        }


        void StopSound()
        {
            Task.Run(async () => {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                   () =>
                   {
                       currentSound?.Stop();
                       isPlayingSequence = false;
                   });
            });
        }

        public void SetParameterValue(string name, string value)
        {
            if (name.ToLower() == "action")
            {
                if (value.Trim().ToLower() == "play")
                    PlaySound();
                else if (value.Trim().ToLower() == "stop")
                    StopSound();
            }
        }

        public void Dispose()
        {
            if (currentSound != null)
            {
                currentSound = null;
                GC.SuppressFinalize(this);
            }
        }
    }



    #region JSon mapping classes
    public class DmxValue
    {
        public short dmxChannel { get; set; }
        public byte dmxValue { get; set; }
    }

    public class SequenceStep
    {
        public int time { get; set; }
        public List<DmxValue> values { get; set; }
    }

    public class SequenceDefinition
    {
        public string uid { get; set; }
        public string displayname { get; set; }
        public string soundfile { get; set; }
        public List<SequenceStep> sequences { get; set; }
    }
    #endregion


}
