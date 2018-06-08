using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioProject {
    /// <summary>
    /// Complex numbers used for the frequency domain.
    /// </summary>
    public class ComplexNumber {

        /// <summary>
        /// Calculates the amplitude of the complex number using Pythagoras.
        /// </summary>
        /// <returns>The amplitude of the complex number.</returns>
        public double Length() {
            return Math.Sqrt(Math.Pow(this.Real, 2) + Math.Pow(this.Imaginary, 2));
        }

        public double Real { get; set; }        // Real portion of the complex number
        public double Imaginary { get; set; }   // Imaginary portion of the complex number
    }
}
