using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AudioProject {

    /// <summary>
    /// Singleton class that handles audio playback and recording from Recorder.dll.
    /// </summary>
    unsafe class Recorder {

        private Recorder() {}

        /// <summary>
        /// Plays the audio stream that is loaded.
        /// </summary>
        /// <param name="data">The data to play.</param>
        /// <param name="dataLength">The length of the data.</param>
        public void PlayAudio(byte[] data, int dataLength) {
            IntPtr pData = Marshal.AllocHGlobal(dataLength);
            Marshal.Copy(data, 0, pData, dataLength);
            SetData(pData, dataLength);
            Play();
        }

        /// <summary>
        /// Pauses the audio stream.
        /// </summary>
        public void PauseAudio() {
            Pause();
        }

        /// <summary>
        /// Stops the audio stream.
        /// </summary>
        public void StopAudio() {
            StopPlay();
        }

        /// <summary>
        /// Records audio.
        /// </summary>
        /// <param name="panel">The panel to record data into.</param>
        public void RecordAudio(AudioPanel panel) {
            recordData = new IntPtr();
            SetData(recordData, 0);
            this.RecordPanel = panel;
            this.RecordPanel.Header = panel.Header;
            this.SetWaveFormat(this.RecordPanel.Header);
            Record();
        }

        /// <summary>
        /// Stops the audio recording.
        /// </summary>
        public void StopRecordAudio() {
            StopRecord();
            this.recordData = GetData();
            int dataLength = GetDataLength();
            byte[] data = new byte[dataLength];
            Marshal.Copy(recordData, data, 0, dataLength);
            this.RecordPanel.RawData = new List<byte>(data);

            this.RecordPanel.SplitWavChannels();
            this.RecordPanel.LTimeChannel.LoadData(this.RecordPanel.LChannel);
        }

        /// <summary>
        /// Sets the wave format of the audio stream.
        /// </summary>
        /// <param name="header">The RIFF header of the audio stream.</param>
        public void SetWaveFormat(RIFFHeader header) {
            WaveFormatEx* waveFormat = GetWaveFormat();
            waveFormat->nChannels = (ushort)header.Channels;
            waveFormat->nSamplesPerSec = (uint)header.SampleRate;
            waveFormat->nBlockAlign = (ushort)((header.Channels * header.BitsPerSample) / 8);
            waveFormat->nAvgBytesPerSec = (uint)(header.SampleRate * waveFormat->nBlockAlign);
            waveFormat->wBitsPerSample = (ushort)header.BitsPerSample;
        }

        // Singleton instance of the Recorder
        private static Recorder instance;
        public static Recorder Instance {
            get {
                if (instance == null) {
                    instance = new Recorder();
                }
                return instance;
            }
        }

        public AudioPanel RecordPanel { get; set; }  // The panel that is being recorded
        private IntPtr recordData;                   // The data being recorded

        // Wave format struct for audio stream
        [StructLayout(LayoutKind.Sequential)]
        private struct WaveFormatEx {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }

        [DllImport("Record.dll")]
        static extern IntPtr GetData();
        [DllImport("Record.dll")]
        static extern void SetData(IntPtr data, int dataLength);
        [DllImport("Record.dll")]
        static extern int GetDataLength();
        [DllImport("Record.dll")]
        static extern WaveFormatEx* GetWaveFormat();
        [DllImport("Record.dll")]
        static extern void Play();
        [DllImport("Record.dll")]
        static extern void Pause();
        [DllImport("Record.dll")]
        static extern void StopPlay();
        [DllImport("Record.dll")]
        static extern void Record();
        [DllImport("Record.dll")]
        static extern void StopRecord();
    }
}
