using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fasttool_modern.Services.Interfaces;
using System.Timers;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Threading;

namespace fasttool_modern.Services
{
    public class AudioDeviceMonitor : IBackgroundTask
    {

        private CoreAudioController audioController;
        private CoreAudioDevice defaultPlaybackDevice;

        

        private static AudioDeviceMonitor instance;
        System.Timers.Timer checkTime;
        public AudioDeviceMonitor() {
            audioController = new CoreAudioController();
            defaultPlaybackDevice = audioController.DefaultPlaybackDevice;
            checkTime = new System.Timers.Timer(1000);
            checkTime.Elapsed += CheckAudioDevice;
        }

        public static AudioDeviceMonitor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AudioDeviceMonitor();
                }
                return instance;
            }
        }

        private void CheckAudioDevice(object sender, ElapsedEventArgs e)
        {
            if (audioController != null)
            {
                var currentDefaultDevice = audioController.DefaultPlaybackDevice;

                if (currentDefaultDevice.Id != defaultPlaybackDevice.Id)
                {
                    defaultPlaybackDevice = currentDefaultDevice;
                    Console.WriteLine("Domyślne urządzenie odtwarzania zmienione.");
                }
            }
        }

        public CoreAudioDevice GetDefaultPlaybackDevice()
        {
            return defaultPlaybackDevice;
        }

        public void Start()
        {
            checkTime.Start();
        }
        public void Stop()
        {
            checkTime.Stop();
        }
    }
}
