using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioProject {
    /// <summary>
    /// Menustrip above each panel that adds functionality the user can use.
    /// </summary>
    public class PanelMenuStrip : MenuStrip {

        /// <summary>
        /// Constructs the menustrip.
        /// </summary>
        /// <param name="parent">The panel that the menustrip is in.</param>
        public PanelMenuStrip(AudioPanel parent) {
            this.window = parent;

            this.InitFile();
            this.InitEdit();
            this.InitTools();

            InfoLabel = new ToolStripLabel();
            InfoLabel.Margin = new Padding(50, 0, 0, 0);
            InfoLabel.ForeColor = Color.SpringGreen;
            this.Items.Add(InfoLabel);

            this.BackColor = ColorSettings.BLACKGRAY;
            this.ForeColor = ColorSettings.GRAY;
            this.Name = "PanelMenuStrip";
            this.Margin = new Padding(0);

            // Setup default selected windowing function
            this.WindowFunc = WindowFunction.RectangularWindow;

            this.ResumeLayout(false);
            this.PerformLayout();
            this.RenderMode = ToolStripRenderMode.Professional;
            this.Renderer = new MenuToolStripRenderer();
        }

        /// <summary>
        /// Initialize all options in the "File" dropdown menu.
        /// </summary>
        private void InitFile() {
            this.file = new ToolStripMenuItem();
            this.open = new ToolStripMenuItem();
            this.save = new ToolStripMenuItem();
            this.close = new ToolStripMenuItem();
            this.edit = new ToolStripMenuItem();

            this.Items.AddRange(new ToolStripItem[] {
                this.file
            });

            this.file.DropDownItems.AddRange(new ToolStripItem[] {
                this.open,
                this.save,
                this.close
            });

            this.file.ForeColor = ColorSettings.LIGHTGRAY;
            this.file.Name = "file";
            this.file.Text = "File";

            this.open.ForeColor = ColorSettings.LIGHTGRAY;
            this.open.Name = "open";
            this.open.Text = "Open";

            this.save.ForeColor = ColorSettings.LIGHTGRAY;
            this.save.Name = "save";
            this.save.Text = "Save";

            this.close.ForeColor = ColorSettings.LIGHTGRAY;
            this.close.Name = "close";
            this.close.Text = "Close";

            this.open.Click += new EventHandler(this.OpenFile);
            this.save.Click += new EventHandler(this.SaveFile);
            this.close.Click += new EventHandler(this.Close);
        }

        /// <summary>
        /// Initialize all options in the "Edit" dropdown menu.
        /// </summary>
        private void InitEdit() {
            this.edit = new ToolStripMenuItem();
            this.delete = new ToolStripMenuItem();
            this.cut = new ToolStripMenuItem();
            this.copy = new ToolStripMenuItem();
            this.paste = new ToolStripMenuItem();

            this.Items.AddRange(new ToolStripItem[] {
                this.edit
            });

            this.edit.DropDownItems.AddRange(new ToolStripItem[] {
                this.delete,
                this.cut,
                this.copy,
                this.paste
            });

            this.edit.ForeColor = ColorSettings.LIGHTGRAY;
            this.edit.Name = "edit";
            this.edit.Text = "Edit";

            this.delete.ForeColor = ColorSettings.LIGHTGRAY;
            this.delete.Name = "delete";
            this.delete.Text = "Delete";

            this.cut.ForeColor = ColorSettings.LIGHTGRAY;
            this.cut.Name = "cut";
            this.cut.Text = "Cut";

            this.copy.ForeColor = ColorSettings.LIGHTGRAY;
            this.copy.Name = "copy";
            this.copy.Text = "Copy";

            this.paste.ForeColor = ColorSettings.LIGHTGRAY;
            this.paste.Name = "paste";
            this.paste.Text = "Paste";

            this.delete.Click += new EventHandler(this.Delete);
            this.copy.Click += new EventHandler(this.Copy);
            this.paste.Click += new EventHandler(this.Paste);
            this.cut.Click += new EventHandler(this.Cut);
        }

        /// <summary>
        /// Initialize all options in the "Tools" dropdown menu.
        /// </summary>
        private void InitTools() {
            this.tools = new ToolStripMenuItem();
            this.dft = new ToolStripMenuItem();
            this.lChannelDFT = new ToolStripMenuItem();
            this.rChannelDFT = new ToolStripMenuItem();
            this.windowing = new ToolStripMenuItem();
            this.rectangular = new ToolStripMenuItem();
            this.triangular = new ToolStripMenuItem();
            this.welch = new ToolStripMenuItem();
            this.sine = new ToolStripMenuItem();
            this.filtering = new ToolStripMenuItem();
            this.lChannelFilter = new ToolStripMenuItem();
            this.rChannelFilter = new ToolStripMenuItem();

            this.Items.AddRange(new ToolStripItem[] {
                this.tools
            });

            this.tools.DropDownItems.AddRange(new ToolStripItem[] {
                this.dft,
                this.windowing,
                this.filtering
            });

            this.dft.DropDownItems.AddRange(new ToolStripItem[] {
                this.lChannelDFT,
                this.rChannelDFT
            });

            this.windowing.DropDownItems.AddRange(new ToolStripItem[] {
                this.rectangular,
                this.triangular,
                this.welch,
                this.sine
            });

            this.filtering.DropDownItems.AddRange(new ToolStripMenuItem[] {
                this.lChannelFilter,
                this.rChannelFilter
            });

            this.tools.ForeColor = ColorSettings.LIGHTGRAY;
            this.tools.Name = "tools";
            this.tools.Text = "Tools";

            this.dft.ForeColor = ColorSettings.LIGHTGRAY;
            this.dft.Name = "fourier";
            this.dft.Text = "Fourier";

            this.lChannelDFT.ForeColor = ColorSettings.LIGHTGRAY;
            this.lChannelDFT.Name = "lChannelDFT";
            this.lChannelDFT.Text = "Left Channel";

            this.rChannelDFT.ForeColor = ColorSettings.LIGHTGRAY;
            this.rChannelDFT.Name = "rChannelDFT";
            this.rChannelDFT.Text = "Right Channel";
            this.rChannelDFT.Enabled = false;

            this.windowing.ForeColor = ColorSettings.LIGHTGRAY;
            this.windowing.Name = "windowing";
            this.windowing.Text = "Windowing";

            this.rectangular.ForeColor = ColorSettings.LIGHTGRAY;
            this.rectangular.Name = "rectangular";
            this.rectangular.Text = "Rectangular";
            this.rectangular.Checked = true;

            this.triangular.ForeColor = ColorSettings.LIGHTGRAY;
            this.triangular.Name = "bartlett";
            this.triangular.Text = "Bartlett";

            this.welch.ForeColor = ColorSettings.LIGHTGRAY;
            this.welch.Name = "welch";
            this.welch.Text = "Welch";

            this.sine.ForeColor = ColorSettings.LIGHTGRAY;
            this.sine.Name = "sine";
            this.sine.Text = "Sine";

            this.filtering.ForeColor = ColorSettings.LIGHTGRAY;
            this.filtering.Name = "filtering";
            this.filtering.Text = "Filter";

            this.lChannelFilter.ForeColor = ColorSettings.LIGHTGRAY;
            this.lChannelFilter.Name = "lChannelFilter";
            this.lChannelFilter.Text = "Left Channel";

            this.rChannelFilter.ForeColor = ColorSettings.LIGHTGRAY;
            this.rChannelFilter.Name = "rChannelFilter";
            this.rChannelFilter.Text = "Right Channel";
            this.rChannelFilter.Enabled = false;

            this.lChannelDFT.Click += new EventHandler(this.DFT);
            this.rChannelDFT.Click += new EventHandler(this.DFT);
            this.rectangular.Click += new EventHandler(this.SetWindow);
            this.triangular.Click += new EventHandler(this.SetWindow);
            this.welch.Click += new EventHandler(this.SetWindow);
            this.sine.Click += new EventHandler(this.SetWindow);
            this.lChannelFilter.Click += new EventHandler(this.ApplyFilter);
            this.rChannelFilter.Click += new EventHandler(this.ApplyFilter);
        }

        /// <summary>
        /// Enables the options that are specific to the right channel.
        /// </summary>
        public void EnableRightChannelOptions() {
            this.rChannelDFT.Enabled = true;
            this.rChannelFilter.Enabled = true;
        }

        /// <summary>
        /// Disables the options that are specific to the right channel.
        /// </summary>
        public void DisableRightChannelOptions() {
            this.rChannelDFT.Enabled = false;
            this.rChannelFilter.Enabled = false;
        }

        /// <summary>
        /// Adds menu shortcuts to some options.
        /// </summary>
        public void AddMenuShortcuts() {
            this.delete.ShortcutKeys = Keys.Delete;
            this.cut.ShortcutKeys = Keys.Control | Keys.X;
            this.copy.ShortcutKeys = Keys.Control | Keys.C;
            this.paste.ShortcutKeys = Keys.Control | Keys.V;
            this.save.ShortcutKeys = Keys.Control | Keys.S;
        }

        /// <summary>
        /// Removes menu shortcuts to some options.
        /// </summary>
        public void RemoveMenuShortcuts() {
            this.delete.ShortcutKeys = Keys.None;
            this.cut.ShortcutKeys = Keys.None;
            this.copy.ShortcutKeys = Keys.None;
            this.paste.ShortcutKeys = Keys.None;
            this.save.ShortcutKeys = Keys.None;
        }

        /// <summary>
        /// Opens a wav file and loads the data int he time domain.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        public void OpenFile(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Open";
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Filter = "WAV File (*.wav) | *.wav";
            dialog.DefaultExt = "wav";

            if (dialog.ShowDialog() == DialogResult.OK) {
                // Parses the wave header
                WaveParser parser = new WaveParser(dialog.FileName);
                this.window.Header = parser.OpenWave();
                this.window.RawData = parser.Data;

                // Set the info label to display the sample rate and bit rate
                this.InfoLabel.Text = this.window.Header.SampleRate + " Sample rate" + " | " + this.window.Header.BitsPerSample + " Bit rate";
                
                // Split the wave channels
                this.window.SplitWavChannels();

                // Loads the data into the time domain charts
                this.window.LTimeChannel.LoadData(this.window.LChannel);
                if (this.window.Header.Channels == 2) {
                    if (!this.window.RTimeChannel.Chart.Visible) {
                        this.window.AddRChannel();
                    }
                    this.window.RTimeChannel.LoadData(this.window.RChannel);
                } else if (this.window.RTimeChannel.Chart.Visible) {
                    this.window.RemoveRChannel();
                }
            }
        }

        /// <summary>
        /// Saves the current audio stream that is opened.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void SaveFile(object sender, EventArgs e) {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Title = "Save ...";
            dialog.CheckPathExists = true;
            dialog.Filter = "WAV File (*.wav) | *.wav";
            dialog.DefaultExt = "wav";

            if (dialog.ShowDialog() == DialogResult.OK) {
                // Converts the data into bytes
                List<byte> byteData = this.window.DataToBytes();

                if (byteData == null) {
                    return;
                }

                // Writes the data to a file
                WaveParser parser = new WaveParser(dialog.FileName);
                parser.SaveWave(byteData, this.window.Header);
            }
        }

        /// <summary>
        /// Closes the panel that the menustrip is part of.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void Close(object sender, EventArgs e) {
            this.window.Dispose();
        }

        /// <summary>
        /// Deletes a selected range in the time domain.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void Delete(object sender, EventArgs e) {
            bool dualChannel = this.window.Header.Channels == 2;
            FileEditor.Delete(this.window, dualChannel);

            if (this.window.LChannel != null) {
                this.window.LTimeChannel.LoadData(this.window.LChannel);
                this.window.LTimeChannel.Chart.Refresh();
                if (this.window.LChannel.Count == 0) {
                    this.window.LChannel = null;
                }
            }

            if (dualChannel) {
                if (this.window.RChannel != null) {
                    this.window.RTimeChannel.LoadData(this.window.RChannel);
                    this.window.RTimeChannel.Chart.Refresh();
                    if (this.window.RChannel.Count == 0) {
                        this.window.RChannel = null;
                    }
                }
            }
        }

        /// <summary>
        /// Copies a selected range in the time domain.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void Copy(object sender, EventArgs e) {
            bool dualChannel = this.window.Header.Channels == 2;
            FileEditor.Copy(this.window, dualChannel);
        }

        /// <summary>
        /// Pastes copied data to the selected position in the time domain.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void Paste(object sender, EventArgs e) {
            // Checks if the window has two channels
            bool dualChannel = this.window.Header.Channels == 2;

            // Gets the data from the clipboard
            ClipboardData copyData = (ClipboardData)Clipboard.GetData("AudioPanel");
            if (copyData != null) {

                // Pastes the data and upsamples or downsamples if needed
                if (this.window.Header.SampleRate == copyData.Header.SampleRate || window.LChannel == null) {
                    FileEditor.Paste(this.window, copyData, dualChannel);
                    this.window.LTimeChannel.LoadData(this.window.LChannel);
                    this.window.LTimeChannel.Chart.Refresh();

                    dualChannel = this.window.Header.Channels == 2;
                    if (dualChannel) {
                        this.window.RTimeChannel.LoadData(this.window.RChannel);
                        this.window.RTimeChannel.Chart.Refresh();
                    }
                } else if (this.window.Header.SampleRate < copyData.Header.SampleRate) {
                    FileEditor.DownSamplePaste(this.window, copyData, dualChannel);
                } else {
                    FileEditor.UpSamplePaste(this.window, copyData, dualChannel);
                }
            }
        }

        /// <summary>
        /// Cuts a selected range in the time domain.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void Cut(object sender, EventArgs e) {
            this.Copy(null, EventArgs.Empty);
            this.Delete(null, EventArgs.Empty);
        }

        /// <summary>
        /// Starts DFT on the selected range in the time domain.
        /// </summary>
        /// <param name="sender">The menu option of which channel to perform DFT (left or right)</param>
        /// <param name="e">Unused.</param>
        private void DFT(object sender, EventArgs e) {
            // Gets the range of the selection
            int startPos = (int)Math.Min(this.window.LTimeChannel.ChartArea.CursorX.SelectionStart, this.window.LTimeChannel.ChartArea.CursorX.SelectionEnd);
            int endPos = (int)Math.Max(this.window.LTimeChannel.ChartArea.CursorX.SelectionStart, this.window.LTimeChannel.ChartArea.CursorX.SelectionEnd);
            if (endPos == startPos) {
                return;
            }

            // Maps the position of the start and end positions of the selection to indexes in the data
            int startSample = (int)this.window.LTimeChannel.Series.Points[startPos].XValue;
            int endSample;
            if (endPos < this.window.LTimeChannel.Series.Points.Count) {
                endSample = (int)this.window.LTimeChannel.Series.Points[endPos].XValue;
            } else {
                endSample = (int)this.window.LTimeChannel.Series.Points[this.window.LTimeChannel.Series.Points.Count - 1].XValue;
            }

            List<short> data = new List<short>();
            List<Thread> threads = new List<Thread>();
            List<List<ComplexNumber>> freqs = new List<List<ComplexNumber>>();
            ToolStripMenuItem currItem = (ToolStripMenuItem)sender;
            // Checks which time channel to perform DFT on
            if (currItem.Name == "lChannelDFT") {
                if (this.window.LChannel != null) {
                    data = new List<short>(this.window.LChannel.GetRange(startSample, endSample - startSample));
                    freqs = this.window.LFreqChannel.CreateDFTThreads(data, this.WindowFunc, out threads);
                }
            } else {
                if (this.window.RChannel != null) {
                    data = new List<short>(this.window.RChannel.GetRange(startSample, endSample - startSample));
                    freqs = this.window.RFreqChannel.CreateDFTThreads(data, this.WindowFunc, out threads);
                }
            }

            // Starts the DFT threads
            for (int i = 0; i < threads.Count; i++) {
                threads[i].Start();
            }

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate {
                // Waits for the DFT threads to finish
                for (int i = 0; i < threads.Count; i++) {
                    threads[i].Join();
                }

                // Combines the DFT data together
                List<ComplexNumber> freqData = new List<ComplexNumber>();
                for (int i = 0; i < threads.Count; i++) {
                    freqData.AddRange(freqs[i]);
                }

                // Displays the data in the frequency channel of the corresponding time channel
                if (currItem.Name == "lChannelDFT") {
                    if (!this.window.LFreqChannel.Chart.IsDisposed) {
                        this.window.LFreqChannel.Chart.Invoke(new MethodInvoker(() => {
                            this.window.LFreqChannel.LoadData(freqData, this.window.Header.SampleRate);
                        }));
                    }
                } else {
                    if (!this.window.RFreqChannel.Chart.IsDisposed) {
                        this.window.RFreqChannel.Chart.Invoke(new MethodInvoker(() => {
                            this.window.RFreqChannel.LoadData(freqData, this.window.Header.SampleRate);
                        }));
                    }
                }
            };
            bgw.RunWorkerAsync();
        }

        /// <summary>
        /// Sets the windowing function to use for DFT.
        /// </summary>
        /// <param name="sender">The selected windowing function.</param>
        /// <param name="e">Unused.</param>
        private void SetWindow(object sender, EventArgs e) {
            ToolStripMenuItem currItem = (ToolStripMenuItem)sender;
            if (currItem != null) {
                // Unchecks all other options except for the selected option
                ((ToolStripMenuItem)currItem.OwnerItem).DropDownItems
                    .OfType<ToolStripMenuItem>().ToList().ForEach(item => {
                        item.Checked = false;
                    });
                currItem.Checked = true;

                // Sets the active windowing function
                switch (currItem.Name) {
                    case "rectangular":
                        WindowFunc = WindowFunction.RectangularWindow;
                        break;
                    case "bartlett":
                        WindowFunc = WindowFunction.BartlettWindow;
                        break;
                    case "welch":
                        WindowFunc = WindowFunction.WelchWindow;
                        break;
                    case "sine":
                        WindowFunc = WindowFunction.SineWindow;
                        break;
                }
            }
        }

        /// <summary>
        /// Applies a filter to the time domain.
        /// </summary>
        /// <param name="sender">The menu option of which channel to apply the filter (left or right)</param>
        /// <param name="e"></param>
        private void ApplyFilter(object sender, EventArgs e) {
            List<Thread> IDFTThreads = new List<Thread>();
            List<List<short>> filterSamples = new List<List<short>>();
            ToolStripMenuItem currItem = (ToolStripMenuItem)sender;

            // Creates the filter and IDFT threads using the selected position on the frequency domain
            if (currItem.Name == "lChannelFilter") {
                if (this.window.LFreqChannel.Series != null) {
                    int n = this.window.LFreqChannel.Series.Points.Count;
                    double freq = this.window.LFreqChannel.ChartArea.CursorX.Position;
                    List<ComplexNumber> filter = this.window.LFreqChannel.CreateFilter(n, freq, this.window.Header);
                    filterSamples = this.window.LFreqChannel.CreateIDFTThreads(filter, out IDFTThreads);
                }
            } else {
                if (this.window.RFreqChannel.Series != null) {
                    int n = this.window.RFreqChannel.Series.Points.Count;
                    double freq = this.window.RFreqChannel.ChartArea.CursorX.Position;
                    List<ComplexNumber> filter = this.window.RFreqChannel.CreateFilter(n, freq, this.window.Header);
                    filterSamples = this.window.RFreqChannel.CreateIDFTThreads(filter, out IDFTThreads);
                }
            }

            // Starts the IDFT threads
            for (int i = 0; i < IDFTThreads.Count; i++) {
                IDFTThreads[i].Start();
            }

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += delegate {
                // Waits for IDFT threads to finish
                for (int i = 0; i < IDFTThreads.Count; i++) {
                    IDFTThreads[i].Join();
                }

                // Combines the IDFT data together
                List<short> filterData = new List<short>();
                for (int i = 0; i < IDFTThreads.Count; i++) {
                    filterData.AddRange(filterSamples[i]);
                }

                // Creates threads to convolve the data
                List<Thread> convolveThreads = null;
                if (currItem.Name == "lChannelFilter") {
                    List<short> data = new List<short>(this.window.LChannel);
                    convolveThreads = this.window.LTimeChannel.CreateConvolutionThreads(filterData, data, this.window.LChannel, false);
                } else {
                    List<short> data = new List<short>(this.window.RChannel);
                    convolveThreads = this.window.RTimeChannel.CreateConvolutionThreads(filterData, data, this.window.RChannel, false);
                }

                // Starts the convolution threads
                for (int i = 0; i < convolveThreads.Count; i++) {
                    convolveThreads[i].Start();
                }

                // Waits for the convolution threads to finish
                for (int i = 0; i < convolveThreads.Count; i++) {
                    convolveThreads[i].Join();
                }

                // Loads the new samples into the corresponding time domain
                if (currItem.Name == "lChannelFilter") {
                    if (!this.window.LTimeChannel.Chart.IsDisposed) {
                        this.window.LTimeChannel.Chart.Invoke(new MethodInvoker(() => {
                            this.window.LTimeChannel.LoadData(this.window.LChannel);
                        }));
                    }
                } else {
                    if (!this.window.RTimeChannel.Chart.IsDisposed) {
                        this.window.RTimeChannel.Chart.Invoke(new MethodInvoker(() => {
                            this.window.RTimeChannel.LoadData(this.window.RChannel);
                        }));
                    }
                }
            };
            bgw.RunWorkerAsync();
        }

        public Func<int, int, double> WindowFunc { get; set; }  // The selected windowing function
        public ToolStripLabel InfoLabel { get; set; }           // Displays the sample rate and bit rate of the current file

        private AudioPanel window;          // The panel that the menustip is in

        private ToolStripMenuItem file;
        private ToolStripMenuItem open;
        private ToolStripMenuItem save;
        private ToolStripMenuItem close;

        private ToolStripMenuItem edit;
        private ToolStripMenuItem delete;
        private ToolStripMenuItem cut;
        private ToolStripMenuItem copy;
        private ToolStripMenuItem paste;

        private ToolStripMenuItem tools;
        private ToolStripMenuItem dft;
        private ToolStripMenuItem windowing;
        private ToolStripMenuItem filtering;

        private ToolStripMenuItem lChannelDFT;
        private ToolStripMenuItem rChannelDFT;

        private ToolStripMenuItem rectangular;
        private ToolStripMenuItem triangular;
        private ToolStripMenuItem welch;
        private ToolStripMenuItem sine;

        private ToolStripMenuItem lChannelFilter;
        private ToolStripMenuItem rChannelFilter;
    }
}
