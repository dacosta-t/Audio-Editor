using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Threading;

namespace AudioProject {

    /// <summary>
    /// Frequency domain channel that displays audio stream in time.
    /// </summary>
    public class TimeChannel : Channel {

        /// <summary>
        /// Constructs the time channel.
        /// </summary>
        /// <param name="title">Title to display above the channel.</param>
        /// <param name="size">Size of the channel.</param>
        /// <param name="parent">Panel that holds the channel.</param>
        public TimeChannel(String title, Size size, AudioPanel parent) : base(title, size, parent) {
            this.InitTimeChart();
        }

        /// <summary>
        /// Inistalizes the chart.
        /// </summary>
        private void InitTimeChart() {
            this.ChartArea.AxisY.LabelStyle.IsStaggered = false;
        }

        /// <summary>
        /// Loads the data onto the chart. Only displays the maximum and minimum values in specific intervals determined by the width of the chart.
        /// </summary>
        /// <param name="data">The data to load.</param>
        public void LoadData(List<short> data) {
            if (data == null) {
                return;
            }

            // Removes the previous data
            this.Chart.Series.Remove(this.Series);
            this.InitSeries();
            this.Series.IsXValueIndexed = true;
            this.Series.ChartType = SeriesChartType.Line;

            // Sets the interval of the data to display
            int interval = data.Count / (this.Chart.Width * 2);
            if (interval < 1) {
                interval = 1;
            }

            // The index of the chart series
            int index = 0;

            // Loops through the data and increments by the interval
            for (int i = 0; i < data.Count; i += interval) {
                short min = short.MaxValue;
                short max = short.MinValue;
                long maxIndex = 0;
                long minIndex = 0;

                // Loops through the data in the invterval to determin the max and min value in the range
                for (int j = i; j < interval + i && j < data.Count; j++) {
                    if (data[j] > max) {
                        max = data[j];
                        maxIndex = j;
                    }

                    if (data[j] < min) {
                        min = data[j];
                        minIndex = j;
                    }
                }

                // Adds the max and min value to the series in the correct position and sets the time stamp axis label.
                if (maxIndex < minIndex) {
                    this.Series.Points.AddXY(maxIndex, max);
                    this.Series.Points[index++].AxisLabel = string.Format("{0:0.##}", maxIndex / (double)(parent.Header.SampleRate)) + " s";
                    this.Series.Points.AddXY(minIndex, min);
                    this.Series.Points[index++].AxisLabel = string.Format("{0:0.##}", minIndex / (double)(parent.Header.SampleRate)) + " s";
                } else {
                    this.Series.Points.AddXY(minIndex, min);
                    this.Series.Points[index++].AxisLabel = string.Format("{0:0.##}", minIndex / (double)(parent.Header.SampleRate)) + " s";
                    this.Series.Points.AddXY(maxIndex, max);
                    this.Series.Points[index++].AxisLabel = string.Format("{0:0.##}", maxIndex / (double)(parent.Header.SampleRate)) + " s";
                }
            }
        }

        /// <summary>
        /// Convolves the data from a filter and applied bit offsets for bit rate changes if needed.
        /// </summary>
        /// <param name="filterSamples">The filter to apply.</param>
        /// <param name="samples">The original samples.</param>
        /// <param name="output">The samples array to save to.</param>
        /// <param name="startPos">The start position in the samples array.</param>
        /// <param name="endPos">The end position in the samples array.</param>
        /// <param name="bitOffset">Whether a bit offset needs to be done for 8-bit.</param>
        public void Convolution(List<short> filterSamples, List<short> samples, List<short> output, int startPos, int endPos, bool bitOffset) {
            int n = filterSamples.Count;

            for (int i = startPos; i < endPos; i++) {
                double sum = 0;
                for (int j = 0; j < filterSamples.Count; j++) {
                    short sample = (i + j) < samples.Count ? samples[i + j] : (short)0;
                    if (i + j < samples.Count) {
                        sample = samples[i + j];
                        if (bitOffset) {
                            sample -= SByte.MaxValue;
                        }
                    } else {
                        sample = 0;
                    }
                    sum += sample * filterSamples[j];
                }
                output[i] = (short)(sum / n);
                if (bitOffset) {
                    output[i] += SByte.MaxValue;
                }
            }
        }

        /// <summary>
        /// Downsamples data and changes bit rate if needed.
        /// </summary>
        /// <param name="samples">The original samples.</param>
        /// <param name="highSampleRate">The sample rate of the original samples.</param>
        /// <param name="lowSampleRate">The sample rate to downsample to.</param>
        /// <param name="bitMapFunc">The bit rate mapping function to use.</param>
        /// <returns>The downsampled samples.</returns>
        public List<short> DownSample(List<short> samples, int highSampleRate, int lowSampleRate, Func<short, short> bitMapFunc) {
            List<short> data = new List<short>();
            double ratio = lowSampleRate / (double)highSampleRate;
            double count = 0;

            for (int i = 0; i < samples.Count; i++) {
                count += ratio;

                // Skips some samples
                if (count >= 1) {
                    short sample = samples[i];
                    if (bitMapFunc != null) {
                        sample = bitMapFunc(sample);
                    }
                    data.Add(sample);
                    count--;
                }
            }
            return data;
        }

        /// <summary>
        /// Downsamples data and changes bit rate if needed.
        /// </summary>
        /// <param name="samples">The original samples.</param>
        /// <param name="highSampleRate">The sample rate to upsample to.</param>
        /// <param name="lowSampleRate">The sample rate of the original samples.</param>
        /// <param name="bitMapFunc">The bit rate mapping function to use.</param>
        /// <returns>The upsampled samples.</returns>
        public List<short> UpSample(List<short> samples, int highSampleRate, int lowSampleRate, Func<short, short> bitMapFunc) {
            List<short> data = new List<short>();
            double ratio = highSampleRate / (double)lowSampleRate;
            double count = ratio;

            for (int i = 0; i < samples.Count; i++) {

                // Keep repeating samples
                while (count >= 1) {
                    short sample = samples[i];
                    if (bitMapFunc != null) {
                        sample = bitMapFunc(sample);
                    }
                    data.Add(sample);
                    count--;
                }
                count += ratio;
            }
            return data;
        }

        /// <summary>
        /// Creates 4 threads to run convolution.
        /// </summary>
        /// <param name="filterSamples">The filter to apply.</param>
        /// <param name="samples">The original samples.</param>
        /// <param name="output">The samples array to save to.</param>
        /// <param name="bitOffset">Whether a bit offset needs to be done for 8-bit.</param>
        /// <returns>The list of threads that will run convolution.</returns>
        public List<Thread> CreateConvolutionThreads(List<short> filterSamples, List<short> samples, List<short> output, bool bitOffset) {
            int numThreads = 4;
            int n = samples.Count;

            // Checks which convolution function to use

            List<Thread> threads = new List<Thread>(numThreads);

            threads.Add(new Thread(() => {
                this.Convolution(filterSamples, samples, output, 0, n / numThreads, bitOffset);
            }));
            threads.Add(new Thread(() => {
                this.Convolution(filterSamples, samples, output, n / numThreads, 2 * n / numThreads, bitOffset);
            }));
            threads.Add(new Thread(() => {
                this.Convolution(filterSamples, samples, output, 2 * n / numThreads, 3 * n / numThreads, bitOffset);
            }));
            threads.Add(new Thread(() => {
                this.Convolution(filterSamples, samples, output, 3 * n / numThreads, n, bitOffset);
            }));

            return threads;
        }
    }
}
