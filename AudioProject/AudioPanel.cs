using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Timers;

namespace AudioProject {

    /// <summary>
    /// This class holds all of the time domains, frequency domains, and buttons to play, pause, and stop the audio.
    /// When a file is loaded or recorded, it will display the signal on the time domains.
    /// </summary>
    public class AudioPanel {

        /// <summary>
        /// Constructs the Audio Panel.
        /// </summary>
        /// <param name="size">Size of the main panel.</param>
        /// <param name="parent">The main program Form that holds the panel.</param>
        public AudioPanel(Size size, AudioProgram parent) {
            this.Parent = parent;
            this.MenuStrip = new PanelMenuStrip(this);
            this.durationTimer = new System.Timers.Timer();
            this.playTimer = new System.Timers.Timer();

            this.recorder = Recorder.Instance;
            this.InitPanels(size);
            this.InitCharts(size);
            this.InitButtons();

            this.AddMouseEventsToChildControls(this.Panel);
            this.Parent.Resize += new EventHandler(this.ResizePanels);
        }

        /// <summary>
        /// Initializes the inner panels.
        /// </summary>
        /// <param name="size">Size of the main panel.</param>
        private void InitPanels(Size size) {
            this.Panel = new TableLayoutPanel();
            this.Panel.Size = new Size(size.Width, size.Height);
            this.Panel.BackColor = ColorSettings.DARKGRAY;
            this.Panel.Margin = new Padding(0);

            this.buttonPanel = new Panel();
            this.buttonPanel.Size = new Size(BUTTON_WIDTH * 3, 30);
            this.buttonPanel.BackColor = ColorSettings.DARKGRAY;
            this.buttonPanel.Margin = new Padding(0);

            // Set column spans for to menustrip and button panel and add them to the main panel
            this.Panel.SetColumnSpan(this.buttonPanel, 2);
            this.Panel.SetColumnSpan(this.MenuStrip, 2);
            this.Panel.Controls.Add(this.MenuStrip, 0, 0);
            this.Panel.Controls.Add(this.buttonPanel, 0, 3);

            // Center the button panel
            buttonPanel.Location = new Point(
                this.Panel.ClientSize.Width / 2 - buttonPanel.Size.Width / 2,
                this.buttonPanel.Location.Y);
            buttonPanel.Anchor = AnchorStyles.None;

            // Add mouse event handlers to enable menustrip shortcuts
            this.Panel.MouseEnter += new EventHandler(this.MouseEnter);
            this.Panel.MouseLeave += new EventHandler(this.MouseLeave);
        }

        /// <summary>
        /// Initializes the charts for the time and frequency domains.
        /// </summary>
        /// <param name="size">Size of the main panel.</param>
        private void InitCharts(Size size) {

            // Initialize time and frequency domain charts, and start Right Channel as not visible
            this.LTimeChannel = new TimeChannel("Left Channel", new Size(size.Width / 2, CHANNEL_HEIGHT - this.MenuStrip.Height - BUTTON_HEIGHT / 2), this);
            this.RTimeChannel = new TimeChannel("Right Channel", new Size(size.Width / 2, CHANNEL_HEIGHT - this.MenuStrip.Height - BUTTON_HEIGHT / 2), this);
            this.RTimeChannel.Chart.Visible = false;
            this.LFreqChannel = new FreqChannel("Frequency", new Size(size.Width / 2, CHANNEL_HEIGHT - this.MenuStrip.Height - BUTTON_HEIGHT / 2), this);
            this.RFreqChannel = new FreqChannel("Frequency", new Size(size.Width / 2, CHANNEL_HEIGHT - this.MenuStrip.Height - BUTTON_HEIGHT / 2), this);
            this.RFreqChannel.Chart.Visible = false;

            // Add all charts to the main panel
            this.Panel.Controls.Add(this.LTimeChannel.Chart, 0, 1);
            this.Panel.Controls.Add(this.LFreqChannel.Chart, 1, 1);
            this.Panel.Controls.Add(this.RTimeChannel.Chart, 0, 2);
            this.Panel.Controls.Add(this.RFreqChannel.Chart, 1, 2);
        }

        /// <summary>
        /// Initializes the play, pause, and stop buttons.
        /// </summary>
        private void InitButtons() {
            this.playButton = new Button();
            this.playButton.Text = "Play";

            this.pauseButton = new Button();
            this.pauseButton.Text = "Pause";
            this.pauseButton.Left = this.playButton.Right;
            this.pauseButton.Enabled = false;

            this.stopButton = new Button();
            this.stopButton.Text = "Stop";
            this.stopButton.Left = this.pauseButton.Right;

            // Add all buttons to the button panel
            this.buttonPanel.Controls.Add(this.playButton);
            this.buttonPanel.Controls.Add(this.pauseButton);
            this.buttonPanel.Controls.Add(this.stopButton);

            // Add click event handlers to buttons
            this.playButton.Click += new EventHandler(this.Play);
            this.pauseButton.Click += new EventHandler(this.Pause);
            this.stopButton.Click += new EventHandler(this.StopPlay);
        }

        /// <summary>
        /// Adds the mouse event handlers that enables menustrip shortcuts to all children of the main panel.
        /// </summary>
        /// <param name="parent">Control that called the function.</param>
        public void AddMouseEventsToChildControls(Control parent) {
            foreach (Control child in parent.Controls) {
                child.MouseEnter += new EventHandler(this.MouseEnter);
                child.MouseLeave += new EventHandler(this.MouseLeave);
            }
        }

        /// <summary>
        /// Adds menustrip shortcuts when mouse enters the panel.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        public void MouseEnter(object sender, EventArgs e) {
            this.MenuStrip.AddMenuShortcuts();
        }

        /// <summary>
        /// Removes menustrip shortcuts when mouse leaves the panel.
        /// </summary>
        /// <param name="sender">Object that called the event handler.</param>
        /// <param name="e">Event arguments.</param>
        public void MouseLeave(object sender, EventArgs e) {
            if (this.Panel.ClientRectangle.Contains(this.Panel.PointToClient(Control.MousePosition)))
                return;
            else {
                this.MenuStrip.RemoveMenuShortcuts();
            }
        }

        /// <summary>
        /// Resizes all panels when the window is resized.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void ResizePanels(object sender, EventArgs e) {
            this.Panel.Width = this.Parent.MainContainer.Width;
            this.LTimeChannel.Chart.Width = this.Panel.Width / 2;
            this.RTimeChannel.Chart.Width = this.Panel.Width / 2;
            this.LFreqChannel.Chart.Width = this.Panel.Width / 2;
            this.RFreqChannel.Chart.Width = this.Panel.Width / 2;
        }

        /// <summary>
        /// Displays the Right Channel time and frequency domains to the main panel.
        /// </summary>
        public void AddRChannel() {
            this.RTimeChannel.Chart.Visible = true;
            this.RFreqChannel.Chart.Visible = true;
            this.Panel.Height += CHANNEL_HEIGHT;
            this.Parent.MainContainer.Height += CHANNEL_HEIGHT;
            this.MenuStrip.EnableRightChannelOptions();
            this.LTimeChannel.Chart.CursorPositionChanging += new EventHandler<CursorEventArgs>(this.MoveSelection);
            this.RTimeChannel.Chart.CursorPositionChanging += new EventHandler<CursorEventArgs>(this.MoveSelection);
            this.LTimeChannel.Chart.CursorPositionChanged += new EventHandler<CursorEventArgs>(this.MoveSelection);
            this.RTimeChannel.Chart.CursorPositionChanged += new EventHandler<CursorEventArgs>(this.MoveSelection);
        }
        
        /// <summary>
        /// Hides the Right Channel time and frequency domains from the main panel.
        /// </summary>
        public void RemoveRChannel() {
            this.RChannel = null;
            this.RTimeChannel.Chart.Visible = false;
            this.RTimeChannel.Series.Points.Clear();
            this.RFreqChannel.Chart.Visible = false;
            this.RFreqChannel.Series.Points.Clear();
            this.Panel.Height -= CHANNEL_HEIGHT;
            this.Parent.MainContainer.Height -= CHANNEL_HEIGHT;
            this.MenuStrip.DisableRightChannelOptions();
            this.LTimeChannel.Chart.CursorPositionChanging -= new EventHandler<CursorEventArgs>(this.MoveSelection);
            this.RTimeChannel.Chart.CursorPositionChanging -= new EventHandler<CursorEventArgs>(this.MoveSelection);
            this.LTimeChannel.Chart.CursorPositionChanged -= new EventHandler<CursorEventArgs>(this.MoveSelection);
            this.RTimeChannel.Chart.CursorPositionChanged -= new EventHandler<CursorEventArgs>(this.MoveSelection);
        }

        /// <summary>
        /// Moves the selection on the opposite time domain to the same position as the active one.
        /// </summary>
        /// <param name="sender">The time domain channel that is calling the event handler (Left or Right channel).</param>
        /// <param name="e">Unused.</param>
        private void MoveSelection(object sender, CursorEventArgs e) {
            if (sender == this.LTimeChannel.Chart) {
                this.RTimeChannel.ChartArea.CursorX.SelectionStart = this.LTimeChannel.ChartArea.CursorX.SelectionStart;
                this.RTimeChannel.ChartArea.CursorX.SelectionEnd = this.LTimeChannel.ChartArea.CursorX.SelectionEnd;
                this.RTimeChannel.ChartArea.CursorX.Position = this.LTimeChannel.ChartArea.CursorX.Position;
            } else {
                this.LTimeChannel.ChartArea.CursorX.SelectionStart = this.RTimeChannel.ChartArea.CursorX.SelectionStart;
                this.LTimeChannel.ChartArea.CursorX.SelectionEnd = this.RTimeChannel.ChartArea.CursorX.SelectionEnd;
                this.LTimeChannel.ChartArea.CursorX.Position = this.RTimeChannel.ChartArea.CursorX.Position;
            }
        }

        /// <summary>
        /// Plays the audio stream that is currently loaded in.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        public void Play(object sender, EventArgs e) {
            // Gets the byte data of the file
            byte[] data = this.DataToBytes()?.ToArray();
            if (data == null) {
                return;
            }
            this.fileDurationMs = (data.Length / (double)(this.Header.SampleRate * this.Header.Channels * this.Header.BitsPerSample / 8)) * 1000;

            // Resets timers and cursor position
            this.RemoveTimers();
            this.playPos = 0;
            this.LTimeChannel.ChartArea.CursorX.Position = 0;
            this.RTimeChannel.ChartArea.CursorX.Position = 0;
            this.durationTimer = new System.Timers.Timer(fileDurationMs);
            this.playTimer = new System.Timers.Timer(PLAY_INTERVAL_MILLISECONDS);
            this.durationTimer.Elapsed += new ElapsedEventHandler(this.durationTick);
            this.playTimer.Elapsed += new ElapsedEventHandler(this.playTimerTick);

            // Sets button text and disables pause button
            this.playButton.Enabled = false;
            this.pauseButton.Enabled = true;
            this.pauseButton.Text = "Pause";
            this.isPaused = false;

            // Starts the audio stream
            this.recorder.SetWaveFormat(this.Header);
            this.recorder.PlayAudio(data, data.Length);
            this.playTimer.Start();
            this.durationTimer.Start();
        }

        /// <summary>
        /// Stops all timers and and resets buttons when audio stream ends.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void durationTick(object sender, ElapsedEventArgs e) {
            this.RemoveTimers();
            this.LTimeChannel.Chart.Invoke(new MethodInvoker(() => {
                this.LTimeChannel.ChartArea.CursorX.Position = this.LTimeChannel.Series.Points.Count;
                this.pauseButton.Text = "Pause";
                this.playButton.Enabled = true;
                this.pauseButton.Enabled = false;
            }));

            if (this.Header.Channels == 2) {
                this.RTimeChannel.Chart.Invoke(new MethodInvoker(() => {
                    this.RTimeChannel.ChartArea.CursorX.Position = this.RTimeChannel.Series.Points.Count;
                }));
            }
        }

        /// <summary>
        /// Moves the cursor on the time domain to seek with the audio stream.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void playTimerTick(object sender, ElapsedEventArgs e) {
            this.playPos += PLAY_INTERVAL_MILLISECONDS;
            double cursorPos = playPos / fileDurationMs;
            this.LTimeChannel.Chart.Invoke(new MethodInvoker(() => {
                this.LTimeChannel.ChartArea.CursorX.Position = cursorPos * this.LTimeChannel.Series.Points.Count;
            }));
            if (this.Header.Channels == 2) {
                this.RTimeChannel.Chart.Invoke(new MethodInvoker(() => {
                    this.RTimeChannel.ChartArea.CursorX.Position = cursorPos * this.RTimeChannel.Series.Points.Count;
                }));
            }
        }

        /// <summary>
        /// Removes the play and duration timers.
        /// </summary>
        private void RemoveTimers() {
            this.playTimer.Dispose();
            this.durationTimer.Dispose();
        }

        /// <summary>
        /// Pauses the audio stream.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        public void Pause(object sender, EventArgs e) {
            this.isPaused = !this.isPaused;
            if (this.isPaused) {
                this.playButton.Enabled = true;
                this.pauseButton.Text = "Unpause";
                this.playTimer.Stop();
                this.durationTimer.Stop();
            } else {
                this.playButton.Enabled = false;
                this.pauseButton.Text = "Pause";
                this.playTimer.Start();
                this.durationTimer.Start();
            }
            this.recorder.PauseAudio();
        }

        /// <summary>
        /// Stops the audio stream.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        public void StopPlay(object sender, EventArgs e) {
            this.playButton.Enabled = true;
            this.pauseButton.Enabled = false;
            this.pauseButton.Text = "Pause";
            this.RemoveTimers();
            this.recorder.StopAudio();
        }

        /// <summary>
        /// Converts the data back into bytes.
        /// </summary>
        /// <returns>Array of data in bytes.</returns>
        public List<byte> DataToBytes() {
            if (this.LChannel == null) {
                return null;
            }

            List<byte> byteData = new List<byte>();
            List<short> data;
            if (this.Header.Channels == 2) {
                data = new List<short>(this.CombineWavChannels());
            } else {
                data = new List<short>(this.LChannel);
            }

            switch (this.Header.BitsPerSample) {
                case 8:
                    for (int i = 0; i < this.LChannel.Count; i++) {
                        byteData.Add((byte)data[i]);
                    }
                    break;
                case 16:
                    for (int i = 0; i < data.Count; i++) {
                        byte[] bytes = BitConverter.GetBytes(data[i]);
                        for (int j = 0; j < bytes.Length; j++) {
                            byteData.Add(bytes[j]);
                        }
                    }
                    break;
            }
            return byteData;
        }

        /// <summary>
        /// Splits the audio into 1 or to channels depending on the format.
        /// </summary>
        public void SplitWavChannels() {
            byte[] data = this.RawData.ToArray();
            List<short> file = new List<short>();
            switch (this.Header.BitsPerSample) {
                case 8:
                    for (int i = 0; i < data.Length; i++) {
                        file.Add(data[i]);
                    }
                    break;
                case 16:
                    for (int i = 0; i < data.Length; i += 2) {
                        file.Add(BitConverter.ToInt16(data, i));
                    }
                    break;
            }

            switch (this.Header.Channels) {
                case 1:
                    this.LChannel = new List<short>(file);
                    break;
                case 2:
                    this.LChannel = new List<short>(file.Where<short>((elem, idx) => idx % 2 == 0));
                    this.RChannel = new List<short>(file.Where<short>((elem, idx) => idx % 2 != 0));
                    break;
            }
        }

        /// <summary>
        /// Interleaves the two channels into one channel.
        /// </summary>
        /// <returns>Array of data as one channel</returns>
        public List<short> CombineWavChannels() {
            List<short> file = new List<short>(this.LChannel.Count + this.RChannel.Count);

            for (int i = 0; i < file.Capacity; i++) {
                if (i < this.LChannel.Count) {
                    file.Add(this.LChannel[i]);
                }

                if (i < this.RChannel.Count) {
                    file.Add(this.RChannel[i]);
                }
            }
            return file;
        }

        /// <summary>
        /// Removes the audio panel from the main form.
        /// </summary>
        public void Dispose() {
            if (durationTimer.Enabled) {
                this.StopPlay(null, EventArgs.Empty);
            }

            // Fix current scroll position on main panel
            if (this.Parent.IsScrollable) {
                if (this.Parent.VScrollBar.Value - this.Panel.Height >= 0) {
                    this.Parent.VScrollBar.Value -= this.Panel.Height;
                } else {
                    this.Parent.VScrollBar.Value = 0;
                }
            }

            // Resizes main form
            if (this.Parent.MainContainer.Top + this.Panel.Height > this.Parent.MenuStrip.Bottom) {
                this.Parent.MainContainer.Top = this.Parent.MenuStrip.Bottom;
            } else {
                this.Parent.MainContainer.Top += this.Panel.Height;
            }
            this.Parent.MainContainer.Height -= this.Panel.Height;
            this.Parent.Width--;
            this.Parent.Resize -= new EventHandler(this.ResizePanels);
            this.Panel.Dispose();
        }

        public const int CHANNEL_HEIGHT = 200; // Height of the charts channels
        public const int BUTTON_HEIGHT = 23;   // Height of the buttons
        public const int BUTTON_WIDTH = 75;    // Width of the buttons
        public const double PLAY_INTERVAL_MILLISECONDS = 100;  // Interval between timer ticks while audio is playing

        public AudioProgram Parent { get; set; }        // Main form of program
        public TableLayoutPanel Panel { get; set; }     // Main panel containing other panels
        public PanelMenuStrip MenuStrip { get; set; }   // Top strip containing dropdowns and extra fucntionality
        public TimeChannel LTimeChannel { get; set; }   // Left channel of time domain
        public TimeChannel RTimeChannel { get; set; }   // Right channel of time domain
        public FreqChannel LFreqChannel { get; set; }   // Left channel of frequency domain
        public FreqChannel RFreqChannel { get; set; }   // Right channel of frequency domain
        public List<byte> RawData { get; set; }         // Raw byte data
        public RIFFHeader Header { get; set; }          // Wave RIFF header information
        public List<short> LChannel { get; set; }       // Converted left channel data
        public List<short> RChannel { get; set; }       // Converted right channel data
        
        private Panel buttonPanel;      // Panel that holds the buttons
        private Recorder recorder;      // Handles audio playback and recording
        private Button playButton;      // Plays audio
        private Button pauseButton;     // Pauses and unpauses audio
        private Button stopButton;      // Stops audio
        private bool isPaused;          // Whether the audio is currently paused or not
        private double fileDurationMs;  // Duration of the file in milliseconds
        private double playPos;         // Current position of playback
        private System.Timers.Timer playTimer;      // Timer while playing
        private System.Timers.Timer durationTimer;  // Timer to stop playing
    }
}