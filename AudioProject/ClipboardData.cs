using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioProject {

    /// <summary>
    /// Holds the RIFF header, left channel, and right channel data for copied data.
    /// </summary>
    [Serializable]
    class ClipboardData {
        public RIFFHeader Header { get; set; }
        public List<short> LChannel {get; set; }
        public List<short> RChannel { get; set; }
    }
}
