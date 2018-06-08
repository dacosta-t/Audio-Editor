using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioProject {

    /// <summary>
    /// Windowing functions used for DFT.
    /// </summary>
    class WindowFunction {
        public static double RectangularWindow(int N, int sample) {
            return 1;
        }

        public static double BartlettWindow(int N, int sample) {
            return 1 - Math.Abs((sample - ((N - 1) / 2)) / ((N - 1) / 2));
        }

        public static double WelchWindow(int N, int sample) {
            return 1 - Math.Pow((sample - ((N - 1) / 2)) / ((N - 1) / 2), 2);
        }

        public static double SineWindow(int N, int sample) {
            return Math.Sin((Math.PI * sample) / (N - 1));
        }
    }
}
