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
    /// Frequency domain channel that holds the frequency data of an audio stream.
    /// </summary>
    public class FreqChannel : Channel {

        /// <summary>
        /// Constructs the frequency channel.
        /// </summary>
        /// <param name="title">Title to display above the channel.</param>
        /// <param name="size">Size of the channel.</param>
        /// <param name="parent">Panel that holds the channel.</param>
        public FreqChannel(String title, Size size, AudioPanel parent) : base(title, size, parent) {
            this.InitFreqChart();
        }

        /// <summary>
        /// Inistalizes the chart.
        /// </summary>
        private void InitFreqChart() {
            this.ChartArea.AxisX.LabelStyle.IsStaggered = false;
            this.ChartArea.AxisY.Minimum = 0;
        }

        /// <summary>
        /// Loads the data onto the chart.
        /// </summary>
        /// <param name="data">The data to display.</param>
        /// <param name="sampleRate">The sample rate of the audio file.</param>
        public void LoadData(List<ComplexNumber> data, int sampleRate) {
            // Removes the previous data
            this.Chart.Series.Remove(this.Series);
            this.InitSeries();
            for (int i = 0; i < data.Count; i++) {
                double f = i * sampleRate / (double)data.Count;
                this.Series.Points.Add(new DataPoint(f, data[i].Length()));
            }

            // Set the chart area interval to the buckets.
            this.ChartArea.CursorX.Interval = sampleRate / this.Series.Points.Count;
        }

        /// <summary>
        /// Fourier that performs DFT on a range of frequency bins. Converts data from the time domain to the frequency domain.
        /// </summary>
        /// <param name="data">The data from the time domain.</param>
        /// <param name="windowFunc">The windowing function to use.</param>
        /// <param name="binStart">The bin to start calculations on.</param>
        /// <param name="binEnd">The bin to stop calculations on.</param>
        /// <returns>A list of complex numbers for the frequency domain.</returns>
        public List<ComplexNumber> DFT(List<short> data, Func<int, int, double> windowFunc, int binStart, int binEnd) {
            List<ComplexNumber> A = new List<ComplexNumber>();
            int n = data.Count;
            for (int f = binStart; f < binEnd; f++) {
                ComplexNumber complex = new ComplexNumber();
                complex.Real = 0;
                complex.Imaginary = 0;
                for (int t = 0; t < n; t++) {
                    double angle = 2 * Math.PI * t * f / n;
                    complex.Real += data[t] * Math.Cos(angle) * windowFunc(n, t);
                    complex.Imaginary -= data[t] * Math.Sin(angle) * windowFunc(n, t);
                }
                complex.Real /= n;
                complex.Imaginary /= n;
                A.Add(complex);
            }
            return A;
        }

        /// <summary>
        /// Inverse fourier that performs IDFT on a range of time. Converts data from the frequency domain to the time domain.
        /// </summary>
        /// <param name="data">The data from the frequency domain.</param>
        /// <param name="timeStart">The sample time to start at.</param>
        /// <param name="timeEnd">The sample time to end at.</param>
        /// <returns>A list of samples for the time domain.</returns>
        public List<short> IDFT(List<ComplexNumber> data, int timeStart, int timeEnd) {
            int n = data.Count;
            List<short> samples = new List<short>(data.Count);
            for (int t = timeStart; t < timeEnd; t++) {
                double sum = 0;
                for (int f = 0; f < n; f++) {
                    double angle = 2 * Math.PI * t * f / n;
                    sum += data[f].Real * Math.Cos(angle) - data[f].Imaginary * Math.Sin(angle);
                }
                samples.Add((short)sum);
            }
            return samples;
        }

        /// <summary>
        /// Creates a filter from the frequency domain to apply to the time domain.
        /// </summary>
        /// <param name="n">The size of the filter.</param>
        /// <param name="freq">The frequency specified to filter.</param>
        /// <param name="header">The RIFF header of the audio file.</param>
        /// <returns>The filter to be applied.</returns>
        public List<ComplexNumber> CreateFilter(int n, double freq, RIFFHeader header) {
            int f = (int)Math.Round(freq * n / header.SampleRate);

            List<ComplexNumber> filter = new List<ComplexNumber>(n);
            for (int i = 0; i < n; i++) {
                ComplexNumber a = new ComplexNumber();
                a.Real = 0;
                filter.Add(a);
            }

            for (int i = 0; i <= f; i++) {
                filter[i].Real = 1;
            }

            for (int i = n - f; i < n; i++) {
                filter[i].Real = 1;
            }

            return filter;
        }

        /// <summary>
        /// Splits DFT into 4 threads that run in parallel.
        /// </summary>
        /// <param name="data">The data to perfrom DFT on.</param>
        /// <param name="windowFunc">The windowing function to use.</param>
        /// <param name="threads">The list of threads to save to.</param>
        /// <returns>A double array containing the split DFT frequency data.</returns>
        public List<List<ComplexNumber>> CreateDFTThreads(List<short> data, Func<int, int, double> windowFunc, out List<Thread> threads) {
            int n = data.Count;
            int numThreads = 4;

            List<List<ComplexNumber>> freqs = new List<List<ComplexNumber>>(numThreads);
            for (int i = 0; i < numThreads; i++) {
                freqs.Add(new List<ComplexNumber>());
            }

            threads = new List<Thread>(numThreads);

            threads.Add(new Thread(() => {
                freqs[0] = this.DFT(data, windowFunc, 0, n / numThreads);
            }));
            threads.Add(new Thread(() => {
                freqs[1] = this.DFT(data, windowFunc, n / numThreads, 2 * n / numThreads);
            }));
            threads.Add(new Thread(() => {
                freqs[2] = this.DFT(data, windowFunc, 2 * n / numThreads, 3 * n / numThreads);
            }));
            threads.Add(new Thread(() => {
                freqs[3] = this.DFT(data, windowFunc, 3 * n / numThreads, n);
            }));

            return freqs;
        }

        /// <summary>
        /// Splits IDFT into 4 threads that run in parallel.
        /// </summary>
        /// <param name="data">The data to perform IDFT on.</param>
        /// <param name="threads">The list of threads to save to.</param>
        /// <returns>A double array containing the split IDFT sample data.</returns>
        public List<List<short>> CreateIDFTThreads(List<ComplexNumber> data, out List<Thread> threads) {
            int numThreads = 4;
            int n = data.Count;

            List<List<short>> filterSamples = new List<List<short>>(numThreads);
            for (int i = 0; i < numThreads; i++) {
                filterSamples.Add(new List<short>());
            }

            threads = new List<Thread>(numThreads);

            threads.Add(new Thread(() => {
                filterSamples[0] = this.IDFT(data, 0, n / numThreads);
            }));
            threads.Add(new Thread(() => {
                filterSamples[1] = this.IDFT(data, n / numThreads, 2 * n / numThreads);
            }));
            threads.Add(new Thread(() => {
                filterSamples[2] = this.IDFT(data, 2 * n / numThreads, 3 * n / numThreads);
            }));
            threads.Add(new Thread(() => {
                filterSamples[3] = this.IDFT(data, 3 * n / numThreads, n);
            }));

            return filterSamples;
        }
    }
}
