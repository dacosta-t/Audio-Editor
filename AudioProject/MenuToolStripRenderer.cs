using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace AudioProject {

    /// <summary>
    /// Custom renderer for the Menustrip to change the UI.
    /// </summary>
    class MenuToolStripRenderer : ToolStripProfessionalRenderer {
        public MenuToolStripRenderer() : base(new MenuColourTable()) {
        }

        /// <summary>
        /// Colors the dropdown arrow.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e) {
            var tsMenuItem = e.Item as ToolStripMenuItem;
            if (tsMenuItem != null) {
                e.ArrowColor = ColorSettings.LIGHTGRAY;
            }
            base.OnRenderArrow(e);
        }
    }

    /// <summary>
    /// Custom color table for properties of the MenuStrip.
    /// </summary>
    class MenuColourTable : ProfessionalColorTable {

        // Background color of menustrip options
        public override Color MenuItemSelectedGradientBegin {
            get { return ColorSettings.DARKGRAY; }
        }

        // Background color of menustrip options
        public override Color MenuItemSelectedGradientEnd {
            get { return ColorSettings.DARKGRAY; }
        }

        // Background color of selected menustrip options
        public override Color MenuItemPressedGradientBegin {
            get { return ColorSettings.DARKGRAY; }
        }

        // Background color of selected menustrip options
        public override Color MenuItemPressedGradientMiddle {
            get { return ColorSettings.DARKGRAY; }
        }

        // Background color of selected menustrip options
        public override Color MenuItemPressedGradientEnd {
            get { return ColorSettings.DARKGRAY; }
        }

        // Background color of dropdown options
        public override Color MenuItemSelected {
            get { return ColorSettings.BLACKGRAY; }
        }

        // Hover border color of menustrip options
        public override Color MenuItemBorder {
            get { return ColorSettings.GRAY; }
        }

        // Border color of dropdown options
        public override Color MenuBorder {
            get { return ColorSettings.GRAY; }
        }

        // Background color of menustrip dropdowns
        public override Color ToolStripDropDownBackground {
            get { return ColorSettings.DARKGRAY; }
        }

        // Background color of left space in dropdown options
        public override Color ImageMarginGradientBegin {
            get { return ColorSettings.DARKGRAY; }
        }

        // Background color of left space in dropdown options
        public override Color ImageMarginGradientMiddle {
            get { return ColorSettings.DARKGRAY; }
        }

        // Background color of left space in dropdown options
        public override Color ImageMarginGradientEnd {
            get { return ColorSettings.DARKGRAY; }
        }

        // Border color around checkmark in dropdown options
        public override Color ButtonSelectedBorder {
            get { return ColorSettings.DARKGRAY; }
        }

        // Background color of checkmark in dropdown options
        public override Color CheckBackground {
            get { return ColorSettings.GRAY; }
        }

        // Background color of checkmark in selected dropdown options
        public override Color CheckSelectedBackground {
            get { return ColorSettings.GRAY; }
        }

        // Background color of checkmark in clicked dropdown options
        public override Color CheckPressedBackground {
            get { return ColorSettings.GRAY; }
        }
    }
}