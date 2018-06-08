using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioProject {

    /// <summary>
    /// RIFF header information for an audio stream.
    /// </summary>
    [Serializable]
    public struct RIFFHeader {
        public byte[] ChunkID { get; set; }
        public int FileSize { get; set; }
        public byte[] RiffType { get; set; }
        public byte[] FmtID { get; set; }
        public int FmtSize { get; set; }
        public short FmtCode { get; set; }
        public short Channels { get; set; }
        public int SampleRate { get; set; }
        public int ByteRate { get; set; }
        public short BlockAlign { get; set; }
        public short BitsPerSample { get; set; }
        public byte[] SubChunkID { get; set; }
        public int SubChunkSize { get; set; }
    }
}
