using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioProject {
    /// <summary>
    /// Maps samples from 8-bit unsigned to 16-bit signed and vice versa.
    /// </summary>
    class BitMapper {

        /// <summary>
        /// Maps a 16-bit signed value to an 8-bit unsigned value of a sample.
        /// </summary>
        /// <param name="sample">The sample to map.</param>
        /// <returns>The 8-bit sample value.</returns>
        public static short To8Bit(short sample) {
            return (short)(((sample / (double)short.MaxValue + 1) / 2.0) * Byte.MaxValue);
        }

        /// <summary>
        /// Maps an 8-bit unsigned value to a 16-bit signed value of a sample.
        /// </summary>
        /// <param name="sample">The sample to map.</param>
        /// <returns>The 16-bit sample value.</returns>
        public static short To16Bit(short sample) {
            double normalized = sample / (double)Byte.MaxValue;
            if (normalized >= 0.5) {
                return (short)((normalized - 0.5) * short.MaxValue);
            } else {
                return (short)(-1 * ((1 - (normalized * 2)) * short.MaxValue));
            }
        }
    }
}
