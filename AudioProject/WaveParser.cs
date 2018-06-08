using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AudioProject {

    /// <summary>
    /// Parses wave files. Used to open, save, and create RIFF headers for wave files.
    /// </summary>
    public class WaveParser {

        /// <summary>
        /// Constructs an empty wave parser.
        /// </summary>
        public WaveParser() {
            this.fileName = null;
        }

        /// <summary>
        /// Constructs the wave parser with a filename.
        /// </summary>
        /// <param name="file">Filename of the wave file.</param>
        public WaveParser(String file) {
            this.fileName = file;
        }

        /// <summary>
        /// Reads and parses information of a wave file.
        /// </summary>
        /// <returns>The RIFF header of the wave file.</returns>
        public RIFFHeader OpenWave() {
            RIFFHeader hdr = new RIFFHeader();

            using (FileStream fs = new FileStream(fileName, FileMode.Open)) {
                using (BinaryReader reader = new BinaryReader(fs)) {
                    hdr.ChunkID = new byte[4];
                    reader.Read(hdr.ChunkID, 0, 4);
                    hdr.FileSize = reader.ReadInt32();
                    hdr.RiffType = new byte[4];
                    reader.Read(hdr.RiffType, 0, 4);
                    hdr.FmtID = new byte[4];
                    reader.Read(hdr.FmtID, 0, 4);
                    hdr.FmtSize = reader.ReadInt32();
                    hdr.FmtCode = reader.ReadInt16();
                    hdr.Channels = reader.ReadInt16();
                    hdr.SampleRate = reader.ReadInt32();
                    hdr.ByteRate = reader.ReadInt32();
                    hdr.BlockAlign = reader.ReadInt16();
                    hdr.BitsPerSample = reader.ReadInt16();

                    // Keep checking until "DATA" subchunk is found
                    hdr.SubChunkID = new byte[4];
                    while (!Encoding.Default.GetString(hdr.SubChunkID).ToUpper().Equals("DATA")) {
                        reader.Read(hdr.SubChunkID, 0, 4);
                        hdr.SubChunkSize = reader.ReadInt32();
                        this.Data = new List<byte>(reader.ReadBytes(hdr.SubChunkSize));
                    }
                }
            }

            return hdr;
        }

        /// <summary>
        /// Saves a wave file.
        /// </summary>
        /// <param name="data">The data to save.</param>
        /// <param name="hdr">The RIFF header of the data.</param>
        public void SaveWave(List<byte> data, RIFFHeader hdr) {
            using (FileStream fs = new FileStream(fileName, FileMode.Create)) {
                using (BinaryWriter writer = new BinaryWriter(fs)) {
                    writer.Write(hdr.ChunkID);
                    writer.Write(Marshal.SizeOf(hdr) + data.Count);
                    writer.Write(hdr.RiffType);
                    writer.Write(hdr.FmtID);
                    writer.Write(hdr.FmtSize);
                    writer.Write(hdr.FmtCode);
                    writer.Write(hdr.Channels);
                    writer.Write(hdr.SampleRate);
                    writer.Write(hdr.ByteRate);
                    writer.Write(hdr.BlockAlign);
                    writer.Write(hdr.BitsPerSample);
                    writer.Write(hdr.SubChunkID);
                    writer.Write(data.Count);
                    writer.Write(data.ToArray());
                }
            }
        }

        /// <summary>
        /// Creates a new RIFF header for an audio stream.
        /// </summary>
        /// <param name="sampleRate">The sample rate for the header.</param>
        /// <param name="bitRate">The bit rate for the header.</param>
        /// <returns>The constructed RIFF header.</returns>
        public RIFFHeader CreateHeader(int sampleRate, short bitRate) {
            RIFFHeader hdr = new RIFFHeader();
            hdr.ChunkID = Encoding.ASCII.GetBytes("RIFF");
            hdr.RiffType = Encoding.ASCII.GetBytes("WAVE");
            hdr.FmtID = Encoding.ASCII.GetBytes("fmt ");
            hdr.SubChunkID = Encoding.ASCII.GetBytes("data");
            hdr.SubChunkSize = 0;
            hdr.FileSize = 0;
            hdr.FmtSize = 16;
            hdr.FmtCode = 1;
            hdr.Channels = 1;
            hdr.SampleRate = sampleRate;
            hdr.BitsPerSample = bitRate;
            hdr.BlockAlign = (short)(hdr.Channels * bitRate / 8);
            hdr.ByteRate = sampleRate * hdr.BlockAlign;
            return hdr;
        }

        private String fileName;                // Filename of a wave file
        public List<byte> Data { get; set; }    // The data read from a wave file
    }
}
