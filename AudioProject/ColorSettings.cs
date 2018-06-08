using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AudioProject {

    /// <summary>
    /// Custom color options commonly used that were not provided in the default Color class.
    /// </summary>
    public struct ColorSettings {
        public static readonly Color BLACKGRAY = Color.FromArgb(35, 35, 35);
        public static readonly Color DARKGRAY = Color.FromArgb(60, 60, 60);
        public static readonly Color GRAY = Color.FromArgb(135, 135, 135);
        public static readonly Color LIGHTGRAY = Color.FromArgb(200, 200, 200);
    }
}
