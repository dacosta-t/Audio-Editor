using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.ComponentModel;

namespace AudioProject {

    /// <summary>
    /// Adds cut, copy, paste, and delete functionality to the time domain.
    /// </summary>
    class FileEditor {

        /// <summary>
        /// Deletes the selected range in the time domain. If there is two channels, it will delete the selected range in both.
        /// </summary>
        /// <param name="window">The audio panel that the time domain is in.</param>
        /// <param name="dualChannel">Whether the audio stream has two channels or not.</param>
        public static void Delete(AudioPanel window, bool dualChannel) {
            // Gets the range of the selection
            int startPos = (int)Math.Min(window.LTimeChannel.ChartArea.CursorX.SelectionStart, window.LTimeChannel.ChartArea.CursorX.SelectionEnd);
            int endPos = (int)Math.Max(window.LTimeChannel.ChartArea.CursorX.SelectionStart, window.LTimeChannel.ChartArea.CursorX.SelectionEnd);

            // Maps the position of the start and end positions of the selection to indexes in the data
            if (endPos != startPos && window.LTimeChannel.Series.Points.Count != 0) {
                int startSample = (int)window.LTimeChannel.Series.Points[startPos].XValue;
                int endSample;
                if (endPos < window.LTimeChannel.Series.Points.Count) {
                    endSample = (int)window.LTimeChannel.Series.Points[endPos].XValue;
                } else {
                    endSample = (int)window.LTimeChannel.Series.Points[window.LTimeChannel.Series.Points.Count - 1].XValue;
                }

                // Sets the cursor back to where the selection started
                window.LTimeChannel.ChartArea.CursorX.SelectionStart = startPos;
                window.LTimeChannel.ChartArea.CursorX.SelectionEnd = startPos;
                window.RTimeChannel.ChartArea.CursorX.SelectionStart = startPos;
                window.RTimeChannel.ChartArea.CursorX.SelectionEnd = startPos;

                // Removes the range in the data
                window.LChannel.RemoveRange(startSample, endSample - startSample);
                if (dualChannel) {
                    window.RChannel.RemoveRange(startSample, endSample - startSample);
                }
                if (window.LChannel.Count <= 1) {
                    window.LChannel.RemoveAt(0);
                    if (dualChannel) {
                        window.RChannel.RemoveAt(0);
                    }
                }
            }
        }

        /// <summary>
        /// Copies the selected range in the time domain. If there is two channels, it will copy the selected range in both.
        /// </summary>
        /// <param name="window">The audio panel that the time domain is in.</param>
        /// <param name="dualChannel">Whether the audio stream has two channels or not.</param>
        public static void Copy(AudioPanel window, bool dualChannel) {
            List<short> lData = new List<short>();
            List<short> rData = new List<short>();
            ClipboardData data = new ClipboardData();

            // Gets the range of the selection
            int startPos = (int)Math.Min(window.LTimeChannel.ChartArea.CursorX.SelectionStart, window.LTimeChannel.ChartArea.CursorX.SelectionEnd);
            int endPos = (int)Math.Max(window.LTimeChannel.ChartArea.CursorX.SelectionStart, window.LTimeChannel.ChartArea.CursorX.SelectionEnd);

            // Maps the position of the start and end positions of the selection to indexes in the data
            if (endPos != startPos && window.LTimeChannel.Series.Points.Count != 0) {
                int startSample = (int)window.LTimeChannel.Series.Points[startPos].XValue;
                int endSample;
                if (endPos < window.LTimeChannel.Series.Points.Count) {
                    endSample = (int)window.LTimeChannel.Series.Points[endPos].XValue;
                } else {
                    endSample = (int)window.LTimeChannel.Series.Points[window.LTimeChannel.Series.Points.Count - 1].XValue;
                }

                // Copies the data to a temporary object
                data.LChannel = window.LChannel.GetRange(startSample, endSample - startSample);
                if (dualChannel) {
                    data.RChannel = window.RChannel.GetRange(startSample, endSample - startSample);
                }
                data.Header = window.Header;

                // Adds the copied object to the clipboard
                Clipboard.SetData("AudioPanel", data);
            }
        }

        /// <summary>
        /// Pastes the copied data from the clipboard to the selected position in the time domain.
        /// If the copied data has two channels, it will paste the right channel to the right channel if the window to paste in has a right channel.
        /// If the copied data has one channel, and the window to paste in has two channels, it will paste the single channel into both channels.
        /// </summary>
        /// <param name="window">The audio panel that the time domain is in.</param>
        /// <param name="copyData">The copied data on the clipboard.</param>
        /// <param name="dualChannel">Whether the audio stream has two channels or not.</param>
        public static void Paste(AudioPanel window, ClipboardData copyData, bool dualChannel) {
            // Paste to the selected position if the time domain is not empty
            if (window.LChannel != null) {
                // Gets the position of the selection
                int cursorPos = (int)window.LTimeChannel.ChartArea.CursorX.Position;

                // Maps the selected position to and index in the data
                int pastePos;
                if (cursorPos < window.LTimeChannel.Series.Points.Count) {
                    pastePos = (int)window.LTimeChannel.Series.Points[cursorPos].XValue;
                } else {
                    pastePos = (int)window.LTimeChannel.Series.Points[window.LTimeChannel.Series.Points.Count - 1].XValue;
                }

                List<short> data = new List<short>(copyData.LChannel);
                // Converts bit rate if needed
                if (copyData.Header.BitsPerSample > window.Header.BitsPerSample) {
                    for (int i = 0; i < data.Count; i++) {
                        data[i] = BitMapper.To8Bit(data[i]);
                    }
                } else if (copyData.Header.BitsPerSample < window.Header.BitsPerSample) {
                    for (int i = 0; i < data.Count; i++) {
                        data[i] = BitMapper.To16Bit(data[i]);
                    }
                }

                // Inserts the data at the selected position
                window.LChannel.InsertRange(pastePos, data);
                if (dualChannel) {
                    if (copyData.RChannel != null) {
                        data = new List<short>(copyData.RChannel);
                        if (copyData.Header.BitsPerSample > window.Header.BitsPerSample) {
                            for (int i = 0; i < data.Count; i++) {
                                data[i] = BitMapper.To8Bit(data[i]);
                            }
                        } else if (copyData.Header.BitsPerSample < window.Header.BitsPerSample) {
                            for (int i = 0; i < data.Count; i++) {
                                data[i] = BitMapper.To16Bit(data[i]);
                            }
                        }
                    }
                    window.RChannel.InsertRange(pastePos, data);
                }
            // Create a new header and paste the data if the time domain is empty
            } else {
                // Creates the new RIFF header for the data
                RIFFHeader hdr = copyData.Header;

                // Copies the data to the channels
                window.LChannel = copyData.LChannel;
                if (copyData.RChannel != null) {
                    if (!dualChannel) {
                        window.AddRChannel();
                    }
                    window.RChannel = copyData.RChannel;
                } else {
                    if (dualChannel) {
                        window.RChannel = copyData.LChannel;
                        hdr.Channels = 2;
                    }
                }

                window.RawData = window.DataToBytes();
                window.MenuStrip.InfoLabel.Text = hdr.SampleRate + " Sample rate" + " | " + hdr.BitsPerSample + " Bit rate";

                //Assigns new header to the window
                hdr.SubChunkSize = window.RawData.Count;
                window.Header = hdr;
            }
        }

        /// <summary>
        /// Pastes the copied data from the clipboard to the selected position in the time domain and downsamples the copied data.
        /// If the copied data has one channel, and the window to paste in has two channels, it will paste the single channel into both channels.
        /// </summary>
        /// <param name="window">The audio panel that the time domain is in.</param>
        /// <param name="copyData">The copied data on the clipboard.</param>
        /// <param name="dualChannel">Whether the audio stream has two channels or not.</param>
        public static void DownSamplePaste(AudioPanel window, ClipboardData copyData, bool dualChannel) {
            // Gets the position of the selection
            int cursorPos = (int)window.LTimeChannel.ChartArea.CursorX.Position;

            // Maps the selected position to and index in the data
            int pastePos;
            if (cursorPos < window.LTimeChannel.Series.Points.Count) {
                pastePos = (int)window.LTimeChannel.Series.Points[cursorPos].XValue;
            } else {
                pastePos = (int)window.LTimeChannel.Series.Points[window.LTimeChannel.Series.Points.Count - 1].XValue;
            }

            double freq = (double)window.Header.SampleRate / 2;
            List<Thread> IDFTThreads = new List<Thread>();

            // Creates the filter and IDFT threads and starts them
            List<ComplexNumber> filter = window.LFreqChannel.CreateFilter(copyData.LChannel.Count, freq, copyData.Header);
            List<List<short>> filterSamples = window.LFreqChannel.CreateIDFTThreads(filter, out IDFTThreads);
            for (int i = 0; i < IDFTThreads.Count; i++) {
                IDFTThreads[i].Start();
            }

            // Check if the copied data is a different bitrate than the channel to pate in
            bool bitOffset = copyData.Header.BitsPerSample == 8;

            BackgroundWorker lbgw = new BackgroundWorker();
            lbgw.DoWork += delegate {
                // Wait for IDFT threads to finish
                for (int i = 0; i < IDFTThreads.Count; i++) {
                    IDFTThreads[i].Join();
                }

                // Combine the IDFT data together
                List<short> filterData = new List<short>();
                for (int i = 0; i < IDFTThreads.Count; i++) {
                    filterData.AddRange(filterSamples[i]);
                }

                // Creates and starts threads to convolve the data
                List<short> data = new List<short>(copyData.LChannel);
                List<Thread> convolveThreads = window.LTimeChannel.CreateConvolutionThreads(filterData, copyData.LChannel, data, bitOffset);
                for (int i = 0; i < convolveThreads.Count; i++) {
                    convolveThreads[i].Start();
                }
                // Wait for convolution to finish on all threads
                for (int i = 0; i < convolveThreads.Count; i++) {
                    convolveThreads[i].Join();
                }

                // Convert bit rate if needed and downsample
                List<short> samples;
                if (copyData.Header.BitsPerSample > window.Header.BitsPerSample) {
                    samples = window.LTimeChannel.DownSample(data, copyData.Header.SampleRate, window.Header.SampleRate, BitMapper.To8Bit);
                } else if (copyData.Header.BitsPerSample < window.Header.BitsPerSample) {
                    samples = window.LTimeChannel.DownSample(data, copyData.Header.SampleRate, window.Header.SampleRate, BitMapper.To16Bit);
                } else {
                    samples = window.LTimeChannel.DownSample(data, copyData.Header.SampleRate, window.Header.SampleRate, null);
                }

                // Paste the data at the selected position
                window.LChannel.InsertRange(pastePos, samples);
                if (dualChannel && copyData.RChannel == null) {
                    window.RChannel.InsertRange(pastePos, samples);
                }

                // Reload the data in the time domain charts
                window.LTimeChannel.Chart.Invoke(new MethodInvoker(() => {
                    window.LTimeChannel.LoadData(window.LChannel);
                    window.LTimeChannel.Chart.Refresh();
                }));

                if (dualChannel && copyData.RChannel == null) {
                    window.RTimeChannel.Chart.Invoke(new MethodInvoker(() => {
                        window.RTimeChannel.LoadData(window.RChannel);
                        window.RTimeChannel.Chart.Refresh();
                    }));
                }
                    
            };
            lbgw.RunWorkerAsync();

            // Paste right channel copy data in right channel domain if it exists
            if (dualChannel && copyData.RChannel != null) {
                BackgroundWorker rbgw = new BackgroundWorker();
                rbgw.DoWork += delegate {
                    // Wait for IDFT threads to finish
                    for (int i = 0; i < IDFTThreads.Count; i++) {
                        IDFTThreads[i].Join();
                    }

                    // Combine the IDFT data together
                    List<short> filterData = new List<short>();
                    for (int i = 0; i < IDFTThreads.Count; i++) {
                        filterData.AddRange(filterSamples[i]);
                    }

                    // Creates and starts threads to convolve the data
                    List<short> data = new List<short>(copyData.RChannel);
                    List<Thread> convolveThreads = window.LTimeChannel.CreateConvolutionThreads(filterData, copyData.RChannel, data, bitOffset);
                    for (int i = 0; i < convolveThreads.Count; i++) {
                        convolveThreads[i].Start();
                    }
                    // Wait for convolution to finish on all threads
                    for (int i = 0; i < convolveThreads.Count; i++) {
                        convolveThreads[i].Join();
                    }

                    // Convert bit rate if needed and downsample
                    List<short> samples;
                    if (copyData.Header.BitsPerSample > window.Header.BitsPerSample) {
                        samples = window.RTimeChannel.DownSample(data, copyData.Header.SampleRate, window.Header.SampleRate, BitMapper.To8Bit);
                    } else if (copyData.Header.BitsPerSample < window.Header.BitsPerSample) {
                        samples = window.RTimeChannel.DownSample(data, copyData.Header.SampleRate, window.Header.SampleRate, BitMapper.To16Bit);
                    } else {
                        samples = window.RTimeChannel.DownSample(data, copyData.Header.SampleRate, window.Header.SampleRate, null);
                    }

                    // Paste the data at the selected position
                    window.RChannel.InsertRange(pastePos, samples);

                    // Reload the data in the time domain chart
                    window.RTimeChannel.Chart.Invoke(new MethodInvoker(() => {
                        window.RTimeChannel.LoadData(window.RChannel);
                        window.RTimeChannel.Chart.Refresh();
                    }));
                };
                rbgw.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Pastes the copied data from the clipboard to the selected position in the time domain and upsamples the copied data.
        /// If the copied data has one channel, and the window to paste in has two channels, it will paste the single channel into both channels.
        /// </summary>
        /// <param name="window">The audio panel that the time domain is in.</param>
        /// <param name="copyData">The copied data on the clipboard.</param>
        /// <param name="dualChannel">Whether the audio stream has two channels or not.</param>
        public static void UpSamplePaste(AudioPanel window, ClipboardData copyData, bool dualChannel) {
            // Gets the position of the selection
            int cursorPos = (int)window.LTimeChannel.ChartArea.CursorX.Position;

            // Maps the selected position to and index in the data
            int pastePos;
            if (cursorPos < window.LTimeChannel.Series.Points.Count) {
                pastePos = (int)window.LTimeChannel.Series.Points[cursorPos].XValue;
            } else {
                pastePos = (int)window.LTimeChannel.Series.Points[window.LTimeChannel.Series.Points.Count - 1].XValue;
            }

            double freq = (double)copyData.Header.SampleRate / 2;
            List<Thread> IDFTThreads = new List<Thread>();

            // Creates the filter and IDFT threads and starts them
            List<ComplexNumber> filter = window.LFreqChannel.CreateFilter(copyData.LChannel.Count, freq, window.Header);
            List<List<short>> filterSamples = window.LFreqChannel.CreateIDFTThreads(filter, out IDFTThreads);
            for (int i = 0; i < IDFTThreads.Count; i++) {
                IDFTThreads[i].Start();
            }

            // Check if the copied data is a different bitrate than the channel to pate in
            bool bitOffset = window.Header.BitsPerSample == 8;

            BackgroundWorker lbgw = new BackgroundWorker();
            lbgw.DoWork += delegate {
                // Wait for IDFT threads to finish
                for (int i = 0; i < IDFTThreads.Count; i++) {
                    IDFTThreads[i].Join();
                }

                // Combine the IDFT data together
                List<short> filterData = new List<short>();
                for (int i = 0; i < IDFTThreads.Count; i++) {
                    filterData.AddRange(filterSamples[i]);
                }

                // Convert bit rate if needed and upsample
                List<short> upSampled;
                if (copyData.Header.BitsPerSample > window.Header.BitsPerSample) {
                    upSampled = window.LTimeChannel.UpSample(copyData.LChannel, window.Header.SampleRate, copyData.Header.SampleRate, BitMapper.To8Bit);
                } else if (copyData.Header.BitsPerSample < window.Header.BitsPerSample) {
                    upSampled = window.LTimeChannel.UpSample(copyData.LChannel, window.Header.SampleRate, copyData.Header.SampleRate, BitMapper.To16Bit);
                } else {
                    upSampled = window.LTimeChannel.UpSample(copyData.LChannel, window.Header.SampleRate, copyData.Header.SampleRate, null);
                }

                // Creates and starts threads to convolve the data
                List<short> samples = new List<short>(upSampled);
                List<Thread> convolveThreads = window.LTimeChannel.CreateConvolutionThreads(filterData, upSampled, samples, bitOffset);
                for (int i = 0; i < convolveThreads.Count; i++) {
                    convolveThreads[i].Start();
                }
                // Wait for convolution to finish on all threads
                for (int i = 0; i < convolveThreads.Count; i++) {
                    convolveThreads[i].Join();
                }

                // Paste the data at the selected position
                window.LChannel.InsertRange(pastePos, samples);
                if (dualChannel && copyData.RChannel == null) {
                    window.RChannel.InsertRange(pastePos, samples);
                }

                // Reload the data in the time domain charts
                window.LTimeChannel.Chart.Invoke(new MethodInvoker(() => {
                    window.LTimeChannel.LoadData(window.LChannel);
                    window.LTimeChannel.Chart.Refresh();
                }));

                if (dualChannel && copyData.RChannel == null) {
                    window.RTimeChannel.Chart.Invoke(new MethodInvoker(() => {
                        window.RTimeChannel.LoadData(window.RChannel);
                        window.RTimeChannel.Chart.Refresh();
                    }));
                }

            };
            lbgw.RunWorkerAsync();

            // Paste right channel copy data in right channel domain if it exists
            if (dualChannel && copyData.RChannel != null) {
                BackgroundWorker rbgw = new BackgroundWorker();
                rbgw.DoWork += delegate {
                    // Wait for IDFT threads to finish
                    for (int i = 0; i < IDFTThreads.Count; i++) {
                        IDFTThreads[i].Join();
                    }

                    // Combine the IDFT data together
                    List<short> filterData = new List<short>();
                    for (int i = 0; i < IDFTThreads.Count; i++) {
                        filterData.AddRange(filterSamples[i]);
                    }

                    // Convert bit rate if needed and upsample
                    List<short> upSampled;
                    if (copyData.Header.BitsPerSample > window.Header.BitsPerSample) {
                        upSampled = window.RTimeChannel.UpSample(copyData.RChannel, window.Header.SampleRate, copyData.Header.SampleRate, BitMapper.To8Bit);
                    } else if (copyData.Header.BitsPerSample < window.Header.BitsPerSample) {
                        upSampled = window.RTimeChannel.UpSample(copyData.RChannel, window.Header.SampleRate, copyData.Header.SampleRate, BitMapper.To16Bit);
                    } else {
                        upSampled = window.RTimeChannel.UpSample(copyData.RChannel, window.Header.SampleRate, copyData.Header.SampleRate, null);
                    }

                    // Creates and starts threads to convolve the data
                    List<short> samples = new List<short>(upSampled);
                    List<Thread> convolveThreads = window.LTimeChannel.CreateConvolutionThreads(filterData, upSampled, samples, bitOffset);
                    for (int i = 0; i < convolveThreads.Count; i++) {
                        convolveThreads[i].Start();
                    }
                    // Wait for convolution to finish on all threads
                    for (int i = 0; i < convolveThreads.Count; i++) {
                        convolveThreads[i].Join();
                    }

                    // Paste the data at the selected position
                    window.RChannel.InsertRange(pastePos, samples);

                    // Reload the data in the time domain charts
                    window.RTimeChannel.Chart.Invoke(new MethodInvoker(() => {
                        window.RTimeChannel.LoadData(window.RChannel);
                        window.RTimeChannel.Chart.Refresh();
                    }));
                };
                rbgw.RunWorkerAsync();
            }
        }
    }
}
