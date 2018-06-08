using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioProject {

    /// <summary>
    /// Main menustrip that resides at the top of the main form.
    /// </summary>
    public class WindowMenuStrip : MenuStrip {

        /// <summary>
        /// Constructs the menustrip.
        /// </summary>
        /// <param name="parent">The program that the menustrip is in.</param>
        public WindowMenuStrip(AudioProgram parent) {
            this.window = parent;
            this.recorder = Recorder.Instance;

            this.InitFile();
            this.InitEncoding();
            this.InitRecord();

            // Setup default selected sample rate and bit rate
            this.window.SampleRate = 8000;
            this.window.BitRate = 8;

            this.BackColor = ColorSettings.BLACKGRAY;
            this.ForeColor = ColorSettings.GRAY;
            this.Name = "WindowMenuStrip";
            this.Margin = new Padding(0);

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
            this.newPanel = new ToolStripMenuItem();
            this.open = new ToolStripMenuItem();
            this.closeAll = new ToolStripMenuItem();

            this.Items.AddRange(new ToolStripItem[] {
                this.file
            });

            this.file.DropDownItems.AddRange(new ToolStripItem[] {
                this.newPanel,
                this.open,
                this.closeAll
            });

            this.file.ForeColor = ColorSettings.LIGHTGRAY;
            this.file.Name = "file";
            this.file.Text = "File";

            this.newPanel.ForeColor = ColorSettings.LIGHTGRAY;
            this.newPanel.Name = "new";
            this.newPanel.Text = "New";
            this.newPanel.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N;

            this.open.ForeColor = ColorSettings.LIGHTGRAY;
            this.open.Name = "open";
            this.open.Text = "Open";

            this.closeAll.ForeColor = ColorSettings.LIGHTGRAY;
            this.closeAll.Name = "closeAll";
            this.closeAll.Text = "Close All";

            this.newPanel.Click += new EventHandler(this.NewPanel);
            this.open.Click += new EventHandler(this.OpenFile);
            this.closeAll.Click += new EventHandler(this.CloseAll);
        }

        /// <summary>
        /// Initialize all options in the "Encoding" dropdown menu.
        /// </summary>
        private void InitEncoding() {
            this.encoding = new ToolStripMenuItem();
            this.sampleRate = new ToolStripMenuItem();
            this.kHz8000 = new ToolStripMenuItem();
            this.kHz11025 = new ToolStripMenuItem();
            this.kHz22050 = new ToolStripMenuItem();
            this.kHz44100 = new ToolStripMenuItem();
            this.bitRate = new ToolStripMenuItem();
            this.bitRate8 = new ToolStripMenuItem();
            this.bitRate16 = new ToolStripMenuItem();

            this.Items.AddRange(new ToolStripItem[] {
                this.encoding
            });

            this.encoding.DropDownItems.AddRange(new ToolStripItem[] {
                this.sampleRate,
                this.bitRate
            });

            this.sampleRate.DropDownItems.AddRange(new ToolStripItem[] {
                this.kHz8000,
                this.kHz11025,
                this.kHz22050,
                this.kHz44100
            });

            this.bitRate.DropDownItems.AddRange(new ToolStripItem[] {
                this.bitRate8,
                this.bitRate16
            });

            this.encoding.ForeColor = ColorSettings.LIGHTGRAY;
            this.encoding.Name = "encoding";
            this.encoding.Text = "Encoding";

            this.sampleRate.ForeColor = ColorSettings.LIGHTGRAY;
            this.sampleRate.Name = "sampleRate";
            this.sampleRate.Text = "Sample Rate";

            this.bitRate.ForeColor = ColorSettings.LIGHTGRAY;
            this.bitRate.Name = "bitRate";
            this.bitRate.Text = "Bit Rate";

            this.kHz8000.ForeColor = ColorSettings.LIGHTGRAY;
            this.kHz8000.Name = "8000";
            this.kHz8000.Text = "8000 kHz";
            this.kHz8000.Checked = true;

            this.kHz11025.ForeColor = ColorSettings.LIGHTGRAY;
            this.kHz11025.Name = "11025";
            this.kHz11025.Text = "11025 kHz";

            this.kHz22050.ForeColor = ColorSettings.LIGHTGRAY;
            this.kHz22050.Name = "22050";
            this.kHz22050.Text = "22050 kHz";

            this.kHz44100.ForeColor = ColorSettings.LIGHTGRAY;
            this.kHz44100.Name = "44100";
            this.kHz44100.Text = "44100 kHz";

            this.bitRate8.ForeColor = ColorSettings.LIGHTGRAY;
            this.bitRate8.Name = "8";
            this.bitRate8.Text = "8-Bit";
            this.bitRate8.Checked = true;

            this.bitRate16.ForeColor = ColorSettings.LIGHTGRAY;
            this.bitRate16.Name = "16";
            this.bitRate16.Text = "16-Bit";

            this.kHz8000.Click += new EventHandler(this.SetEncoding);
            this.kHz11025.Click += new EventHandler(this.SetEncoding);
            this.kHz22050.Click += new EventHandler(this.SetEncoding);
            this.kHz44100.Click += new EventHandler(this.SetEncoding);
            this.bitRate8.Click += new EventHandler(this.SetEncoding);
            this.bitRate16.Click += new EventHandler(this.SetEncoding);
        }

        /// <summary>
        /// Initialize the record menustrip option.
        /// </summary>
        private void InitRecord() {
            this.record = new ToolStripMenuItem();
            this.stopRecord = new ToolStripMenuItem();

            this.Items.AddRange(new ToolStripItem[] {
                this.record,
                this.stopRecord
            });

            this.record.ForeColor = ColorSettings.LIGHTGRAY;
            this.record.Name = "record";
            this.record.Text = "Record";

            this.stopRecord.ForeColor = ColorSettings.LIGHTGRAY;
            this.stopRecord.Name = "stopRecord";
            this.stopRecord.Text = "Stop Record";
            this.stopRecord.Visible = false;

            this.record.Click += new EventHandler(this.StartRecord);
            this.stopRecord.Click += new EventHandler(this.StopRecord);
        }

        /// <summary>
        /// Creates a new audio panel that holds its own time and frequency domain.
        /// </summary>
        /// <returns>The newly created audio panel.</returns>
        public AudioPanel NewAudioPanel() {
            AudioPanel newPanel = new AudioPanel(new Size(this.window.MainContainer.Width, AudioPanel.CHANNEL_HEIGHT + AudioPanel.BUTTON_HEIGHT), this.window);
            this.window.MainContainer.Controls.Add(newPanel.Panel);
            this.window.MainContainer.Height += newPanel.Panel.Height;

            // Initializes the scrollbar if needed and is not already initialized
            if (this.window.MainContainer.Height > this.window.ClientSize.Height && !this.window.IsScrollable) {
                this.window.InitScrollBar();
            }

            // Increases the scrollbar range if it is available
            if (this.window.IsScrollable) {
                this.window.VScrollBar.Maximum = this.window.MainContainer.Height - this.Parent.ClientSize.Height + this.Height;
            }

            // Adds the panel to the programs linked list
            this.window.AudioPanels.AddLast(newPanel);
            return newPanel;
        }

        /// <summary>
        /// Creates a new audio panel.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void NewPanel(object sender, EventArgs e) {
            NewAudioPanel();
        }

        /// <summary>
        /// Creates a new audio panel and opens a wave file in it.
        /// </summary>
        /// <param name="sender">Unused/</param>
        /// <param name="e">Unused.</param>
        private void OpenFile(object sender, EventArgs e) {
            AudioPanel newPanel = NewAudioPanel();
            newPanel.MenuStrip.OpenFile(null, EventArgs.Empty);
        }

        /// <summary>
        /// Closes all audio panels.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void CloseAll(object sender, EventArgs e) {
            foreach (AudioPanel panel in this.window.AudioPanels) {
                panel.Dispose();
            }
            this.window.AudioPanels.Clear();
        }

        /// <summary>
        /// Sets the encoding options (sample rate or bit rate).
        /// </summary>
        /// <param name="sender">The selected encoding option.</param>
        /// <param name="e">Unused/</param>
        private void SetEncoding(object sender, EventArgs e) {
            ToolStripMenuItem currItem = sender as ToolStripMenuItem;
            if (currItem != null) {
                // Uncheck all options except for the selected option
                ((ToolStripMenuItem)currItem.OwnerItem).DropDownItems
                    .OfType<ToolStripMenuItem>().ToList().ForEach(item => {
                        item.Checked = false;
                    });
                currItem.Checked = true;

                // Changes sample rate or bit rate to selected value
                switch (currItem.OwnerItem.Name) {
                    case "sampleRate":
                        this.window.SampleRate = Convert.ToInt32(currItem.Name);
                        break;
                    case "bitRate":
                        this.window.BitRate = Convert.ToInt16(currItem.Name);
                        break;
                }
            }
        }

        /// <summary>
        /// Creates a new audio panel and starts an audio recording.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void StartRecord(object sender, EventArgs e) {
            this.record.Visible = false;
            this.stopRecord.Visible = true;
            WaveParser parser = new WaveParser();
            AudioPanel newPanel = this.NewAudioPanel();
            if (this.window.IsScrollable) {
                int val = this.window.VScrollBar.Maximum - this.window.VScrollBar.Value;
                this.window.VScrollBar.Value = this.window.VScrollBar.Maximum;
                this.window.MainContainer.Top = this.window.MainContainer.Location.Y - val;
            }
            newPanel.Header = parser.CreateHeader(this.window.SampleRate, this.window.BitRate);
            newPanel.MenuStrip.InfoLabel.Text = newPanel.Header.SampleRate + " Sample rate" + " | " + newPanel.Header.BitsPerSample + " Bit rate";
            recorder.RecordAudio(newPanel);
        }

        /// <summary>
        /// Stops the recording of the audio panel that is recording.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void StopRecord(object sender, EventArgs e) {
            this.stopRecord.Visible = false;
            this.record.Visible = true;
            recorder.StopRecordAudio();
        }

        private AudioProgram window;        // The main program window
        private Recorder recorder;          // The recorder for audio playback and recording

        private ToolStripMenuItem file;
        private ToolStripMenuItem newPanel;
        private ToolStripMenuItem open;
        private ToolStripMenuItem closeAll;

        private ToolStripMenuItem encoding;

        private ToolStripMenuItem sampleRate;
        private ToolStripMenuItem kHz8000;
        private ToolStripMenuItem kHz11025;
        private ToolStripMenuItem kHz22050;
        private ToolStripMenuItem kHz44100;

        private ToolStripMenuItem bitRate;
        private ToolStripMenuItem bitRate8;
        private ToolStripMenuItem bitRate16;

        private ToolStripMenuItem record;
        private ToolStripMenuItem stopRecord;
    }
}
