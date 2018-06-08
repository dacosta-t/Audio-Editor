using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AudioProject {

    /// <summary>
    /// The main window of the program. This holds all audio panels.
    /// </summary>
    public partial class AudioProgram : Form {

        /// <summary>
        /// Constructs the form.
        /// </summary>
        public AudioProgram() {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the audio program form.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void AudioProgram_Load(object sender, EventArgs e) {
            Size screen = Screen.PrimaryScreen.WorkingArea.Size;

            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(screen.Width / 3, screen.Height / 3);
            this.MenuStrip = new WindowMenuStrip(this);

            AudioPanels = new LinkedList<AudioPanel>();
            this.Controls.Add(this.MenuStrip);
            this.InitMainContainer();

            // Sets default sample rate and bit rate
            this.SampleRate = 8000;
            this.BitRate = 8;

            this.IsScrollable = false;

            this.Resize += new EventHandler(this.ResizePanels);
            this.MainContainer.Resize += new EventHandler(this.ResizePanels);
        }

        /// <summary>
        /// Initalizes the main container that holds all of the audio panels.
        /// </summary>
        private void InitMainContainer() {
            this.MainContainer = new TableLayoutPanel();
            this.MainContainer.Size = new Size(this.ClientSize.Width, 0);
            this.MainContainer.Margin = new Padding(0);
            this.MainContainer.BorderStyle = BorderStyle.FixedSingle;
            this.MainContainer.Top = this.MenuStrip.Bottom;
            this.Controls.Add(this.MainContainer);
            this.MainContainer.MouseEnter += new EventHandler(this.Focus);
        }

        /// <summary>
        /// Initializes the vertical scroll bar if it is not active.
        /// </summary>
        public void InitScrollBar() {
            if (this.ScrollPanel == null) {
                this.ScrollPanel = new Panel();
                this.ScrollPanel.Size = new Size(20, this.ClientSize.Height);
                this.ScrollPanel.Margin = new Padding(0);
                this.ScrollPanel.Dock = DockStyle.Right;

                this.VScrollBar = new VScrollBar();
                this.VScrollBar.Size = new Size(this.ScrollPanel.Width, this.ScrollPanel.Height);
                this.VScrollBar.Top = this.ScrollPanel.Top;
                this.VScrollBar.Minimum = 0;

                this.Controls.Add(this.ScrollPanel);
                this.ScrollPanel.Controls.Add(this.VScrollBar);

                // Trigger resize to resize charts for scrollbar
                this.Width--;

                this.MainContainer.MouseWheel += new MouseEventHandler(this.ScrollMouseContainer);
                this.VScrollBar.Scroll += new ScrollEventHandler(this.ScrollContainer);
            }
            this.IsScrollable = true;
        }

        /// <summary>
        /// Removes the scroll bar.
        /// </summary>
        private void RemoveScrollBar() {
            this.VScrollBar.Dispose();
            this.ScrollPanel.Dispose();
            this.VScrollBar = null;
            this.ScrollPanel = null;
            this.IsScrollable = false;
            this.MainContainer.MouseWheel -= new MouseEventHandler(this.ScrollMouseContainer);
        }

        /// <summary>
        /// Scrolls the main container when the mouse wheel is scrolled.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Mouse scroll wheel information.</param>
        private void ScrollMouseContainer(object sender, MouseEventArgs e) {
            // Do not scroll panel if control is held
            if ((ModifierKeys & Keys.Control) == Keys.Control) {
                return;
            }

            try {
                if (e.Delta < 0) {
                    int val = this.VScrollBar.Maximum - this.VScrollBar.Value;
                    if (val < SCROLL_SCALE) {
                        this.MainContainer.Top = this.MainContainer.Location.Y - val;
                        this.VScrollBar.Value += val;
                    } else {
                        this.MainContainer.Top = this.MainContainer.Location.Y - SCROLL_SCALE;
                        this.VScrollBar.Value += SCROLL_SCALE;
                    }
                } else {
                    if (this.VScrollBar.Value < SCROLL_SCALE) {
                        this.MainContainer.Top = this.MainContainer.Location.Y + this.VScrollBar.Value;
                        this.VScrollBar.Value -= this.VScrollBar.Value;
                    } else {
                        this.MainContainer.Top = this.MainContainer.Location.Y + SCROLL_SCALE;
                        this.VScrollBar.Value -= SCROLL_SCALE;
                    }
                }
            } catch { }
        }

        /// <summary>
        /// Scrolls the main container when the scroll bar is clicked and dragged.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Scrollbar information.</param>
        private void ScrollContainer(object sender, ScrollEventArgs e) {
            int val = this.VScrollBar.Maximum - this.VScrollBar.Value;
            if (val < SCROLL_SCALE && val + this.VScrollBar.Value > this.VScrollBar.Maximum) {
                this.MainContainer.Top = this.MainContainer.Location.Y - val;
                this.VScrollBar.Value += val;
            } else {
                this.MainContainer.Top = this.MainContainer.Location.Y + (e.OldValue - e.NewValue);
            }
        }

        /// <summary>
        /// Focuses the main container.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void Focus(object sender, EventArgs e) {
            this.MainContainer.Focus();
        }

        /// <summary>
        /// Resizes the panels inside the program when the window is resized.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void ResizePanels(Object sender, EventArgs e) {
            if (this.ClientSize.Height >= this.MainContainer.Size.Height) {
                if (this.IsScrollable) {
                    this.RemoveScrollBar();
                }
            } else {
                if (!this.IsScrollable) {
                    this.InitScrollBar();
                }
            }
            
            if (this.IsScrollable) {
                this.ScrollPanel.Visible = true;
                this.VScrollBar.Enabled = true;
                this.MainContainer.Width = this.ClientSize.Width - this.ScrollPanel.Width;
                this.ScrollPanel.Height = this.ClientSize.Height;
                this.VScrollBar.Height = this.ScrollPanel.Height;
                this.VScrollBar.Maximum = this.MainContainer.Height - this.ClientSize.Height + this.MenuStrip.Height;
            } else {
                this.MainContainer.Width = this.ClientSize.Width;
            }
        }

        public TableLayoutPanel MainContainer { get; set; }      // The main container that holds all the audio panels
        public Panel ScrollPanel { get; set; }                   // The scroll panel that holds the scroll bar
        public VScrollBar VScrollBar { get; set; }               // The vertical scroll bar
        public LinkedList<AudioPanel> AudioPanels { get; set; }  // The list of audio panels that are currently created
        public bool IsScrollable { get; set; }                   // Whether the program is scrollable or not
        public int SampleRate { get; set; }                      // The sample rate that is currently selected for recording
        public short BitRate { get; set; }                       // The bit rate that is current selected for recording
        public WindowMenuStrip MenuStrip { get; set; }           // The menustrip for the main program

        private const int SCROLL_SCALE = 10;                     // The scale of how much to move when scrolling
    }
}