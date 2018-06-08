using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AudioProject {

    /// <summary>
    /// Abstract class of a time or frequency channel.
    /// </summary>
    public abstract class Channel {

        /// <summary>
        /// Constructs the channel.
        /// </summary>
        /// <param name="title">Title to display above the channel.</param>
        /// <param name="size">Size of the channel.</param>
        /// <param name="parent">Panel that holds the channel.</param>
        public Channel(String title, Size size, AudioPanel parent) {
            this.Chart = new Chart();
            this.ChartArea = new ChartArea();
            this.parent = parent;

            this.InitChart(title, size);
            this.InitSeries();

            // Adds event handlers to the chart
            this.Chart.KeyDown += new KeyEventHandler(this.KeyDown);
            this.Chart.KeyUp += new KeyEventHandler(this.KeyUp);
            this.Chart.MouseEnter += new EventHandler(this.Focus);
            this.Chart.MouseWheel += new MouseEventHandler(this.Zoom);
        }

        /// <summary>
        /// Initializes the chart.
        /// </summary>
        /// <param name="title">Title to display above the chart.</param>
        /// <param name="size">Size of the channel</param>
        private void InitChart(String title, Size size) {
            this.Chart.Size = new Size(size.Width, size.Height);
            this.Chart.Margin = new Padding(0);

            this.ChartArea.Name = "ChartArea";
            this.ChartArea.AxisX.Minimum = 0;
            this.ChartArea.AxisX.ScaleView.Zoomable = false;
            this.ChartArea.CursorX.IsUserSelectionEnabled = true;
            this.ChartArea.CursorX.IsUserEnabled = true;
            this.ChartArea.CursorX.Position = 0;
            this.ChartArea.AxisX.MajorGrid.Enabled = false;
            this.ChartArea.AxisY.MajorGrid.Enabled = false;
            this.ChartArea.AxisY.LabelStyle.IsStaggered = false;

            this.Chart.BackColor = ColorSettings.DARKGRAY;
            this.ChartArea.BackColor = ColorSettings.BLACKGRAY;
            this.ChartArea.AxisX.LabelStyle.ForeColor = ColorSettings.LIGHTGRAY;
            this.ChartArea.AxisY.LabelStyle.ForeColor = ColorSettings.LIGHTGRAY;
            this.Chart.Titles.Add(title);
            this.Chart.Titles[0].ForeColor = ColorSettings.LIGHTGRAY;

            this.Chart.ChartAreas.Add(this.ChartArea);
        }

        /// <summary>
        /// Initializes the series in the chart.
        /// </summary>
        protected void InitSeries() {
            this.Series = new Series();
            this.Series.Name = "Series";
            this.Series.Color = Color.SpringGreen;
            this.Series.ChartArea = this.ChartArea.Name;
            this.Chart.Series.Add(this.Series);
        }

        /// <summary>
        /// Focuses the chart.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void Focus(object sender, EventArgs e) {
            this.Chart.Focus();
        }

        /// <summary>
        /// Zooms in the the chart.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">The mouse cursor.</param>
        private void Zoom(object sender, MouseEventArgs e) {
            if (this.ctrlDown) {
                try {
                    double xMin = this.ChartArea.AxisX.ScaleView.ViewMinimum;
                    double xMax = this.ChartArea.AxisX.ScaleView.ViewMaximum;
                    if (e.Delta < 0) {
                        double posXStart = this.ChartArea.AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) * 2;
                        double posXFinish = this.ChartArea.AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) * 2;
                        if (Math.Round(xMin) <= 0 && Math.Round(xMax) >= this.Series.Points.Count) {
                            this.ChartArea.AxisX.ScaleView.ZoomReset();
                        } else {
                            this.ChartArea.AxisX.ScaleView.Zoom(posXStart, posXFinish);
                        }
                    } else {
                        double posXStart = this.ChartArea.AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                        double posXFinish = this.ChartArea.AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                        this.ChartArea.AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    }
                } catch { }
            }
        }

        /// <summary>
        /// Checks for the control key being held down.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">The key being pressed.</param>
        private void KeyDown(object sender, KeyEventArgs e) {
            this.ctrlDown = e.Control;
        }

        /// <summary>
        /// Sets control key being held to false.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void KeyUp(object sender, KeyEventArgs e) {
            this.ctrlDown = false;
        }

        public Chart Chart { get; set; }            // The main chart object
        public ChartArea ChartArea { get; set; }    // Inner area of the chart
        public Series Series { get; set; }          // Series that displays the signal

        protected AudioPanel parent;                // The panel that the channel is in
        private bool ctrlDown = false;              // Whether the control key is being held down or not
    }
}
