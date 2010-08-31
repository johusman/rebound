using System;
using System.IO;
using System.Collections.Generic;
namespace Rebound
{
    interface ChunkDataReader
    {
        // Return false to abort processing
        bool ProcessChunk(BinaryReader reader, int length);
    }

    class TopChunkReader : ChunkDataReader
    {
        Dictionary<string, ChunkDataReader> chunkReaders;

        public TopChunkReader(Dictionary<string, ChunkDataReader> chunkReaders)
        {
            this.chunkReaders = chunkReaders;
        }

        #region ChunkDataReader Members

        public virtual bool ProcessChunk(BinaryReader reader, int length)
        {
            while (length > 0)
            {
                String chunkName = new string(reader.ReadChars(4));
                int subLength = reader.ReadInt32();
                if (chunkReaders.ContainsKey(chunkName))
                {
                    ChunkDataReader chunkReader = chunkReaders[chunkName];
                    if (!chunkReader.ProcessChunk(reader, subLength))
                    {
                        return false;
                    }
                }
                else
                {
                    reader.ReadBytes(subLength);
                }
                length -= (8 + subLength);
            }
            return true;
        }
        
        #endregion
    }

    class RIFFChunkReader : TopChunkReader
    {
        public RIFFChunkReader(Dictionary<string, ChunkDataReader> chunkReaders) : base(chunkReaders) {}

        public override bool ProcessChunk(BinaryReader reader, int length)
        {
            String format = new string(reader.ReadChars(4));
            if (format.Equals("WAVE"))
            {
                return base.ProcessChunk(reader, length);
            }
            else
            {
                return false;
            }
        }
    }

    class WAVFormatChunkReader : ChunkDataReader
    {
        WAVInputStream wavInputStream;

        public WAVFormatChunkReader(WAVInputStream wavInputStream)
        {
            this.wavInputStream = wavInputStream;
        }

        #region ChunkDataReader Members

        public bool ProcessChunk(BinaryReader reader, int length)
        {
            wavInputStream.CompressionCode = reader.ReadInt16();
            wavInputStream.NumberOfChannels = reader.ReadInt16();
            wavInputStream.SampleRate = reader.ReadInt32();
            int bytesPerSecond = reader.ReadInt32();
            int blockAlign = reader.ReadInt16();
            wavInputStream.BitsPerSample = reader.ReadInt16();
            if (length > 16)
            {
                reader.ReadBytes(length - 16);
            }
            return true;
        }

        #endregion
    }

    class WAVDataChunkReader : ChunkDataReader
    {
        WAVInputStream wavInputStream;

        public WAVDataChunkReader(WAVInputStream wavInputStream)
        {
            this.wavInputStream = wavInputStream;
        }
        
        #region ChunkDataReader Members

        public bool ProcessChunk(BinaryReader reader, int length)
        {
            wavInputStream.SetLengthInBytes(length);
            /*
             * Causes parsing to be aborted and stream left
             * pointing to the start of the wave data.
             */
            return false;
        }

        #endregion
    }

    class WAVInputStream : ReboundInputStream
    {
        private Stream inputStream;
        private BinaryReader reader;
        private long numberOfChannels = 0;
        private long bytesPerSample = 0;
        private long lengthInSamples = 0;
        private long compressionCode = 0;
        private long sampleRate = 0;

        public WAVInputStream(Stream inputStream)
        {
            this.inputStream = inputStream;
            this.reader = new BinaryReader(inputStream);

            ReadFormat();
        }

        private void ReadFormat()
        {
            Dictionary<string, ChunkDataReader> riffChunks = new Dictionary<string, ChunkDataReader>();
            riffChunks.Add("fmt ", new WAVFormatChunkReader(this));
            riffChunks.Add("data", new WAVDataChunkReader(this));
            RIFFChunkReader riffReader = new RIFFChunkReader(riffChunks);
            Dictionary<string, ChunkDataReader> topChunks = new Dictionary<string, ChunkDataReader>();
            topChunks.Add("RIFF", riffReader);
            TopChunkReader topReader = new TopChunkReader(topChunks);

            topReader.ProcessChunk(reader, (int) this.inputStream.Length);

            if (compressionCode != 1)
                throw new UnsupportedFormatException("Compressed WAVs are not supported.");

            if (bytesPerSample != 2)
                throw new UnsupportedFormatException("Only WAV files with 16-bit PCM is supported. This was " + bytesPerSample * 8 + " bits.");

        }

        #region ReboundInputStream Members

        public Int16[] ReadSamples()
        {
            Int16[] samples = new Int16[numberOfChannels];
            for (int i = 0; i < numberOfChannels; i++)
            {
                Int16? sample = ReadSample();
                if (sample.HasValue)
                {
                    samples[i] = sample.Value;
                }
                else
                {
                    return new Int16[0];
                }
            }

            return samples;
        }

        public Int16? ReadSample()
        {
            byte[] bytes = new byte[2];
            int bytesRead = inputStream.Read(bytes, 0, 2);
            if (bytesRead == 2)
            {
                return BitConverter.ToInt16(bytes, 0);
            }
            else
            {
                return null;
            }
        }

        #endregion

        public long LengthInSamples
        {
            get { return this.lengthInSamples; }
        }

        public long NumberOfChannels
        {
            get { return numberOfChannels; }
            set { numberOfChannels = value; }
        }

        public long CompressionCode
        {
            get { return compressionCode; }
            set { compressionCode = value; }
        }

        public long SampleRate
        {
            get { return sampleRate; }
            set { sampleRate = value; }
        }

        public long BitsPerSample
        {
            set { bytesPerSample = value >> 3; }
            get { return bytesPerSample << 3; }
        }

        public long BytesPerSample
        {
            set { bytesPerSample = value; }
            get { return bytesPerSample; }
        }

        public void SetLengthInBytes(int length)
        {
 	        lengthInSamples = length / (numberOfChannels * bytesPerSample);
        }
    }

    class WAVOutputStream : ReboundOutputStream
    {
        private Stream outputStream;
        private int numberOfChannels;
        private BinaryWriter writer;

        public WAVOutputStream(Stream outputStream, WAVInputStream inputStream, int numberOfChannels)
        {
            this.numberOfChannels = numberOfChannels;
            this.outputStream = outputStream;

            this.writer = new BinaryWriter(outputStream);

            long dataLength = inputStream.LengthInSamples * numberOfChannels * inputStream.BytesPerSample;

            writer.Write("RIFF".ToCharArray());
            writer.Write((Int32)(36 + dataLength));
            writer.Write("WAVEfmt ".ToCharArray());
            writer.Write((Int32)16); // Format Chunk Length
            writer.Write((Int16)1); // Compression Code
            writer.Write((Int16)numberOfChannels);
            writer.Write((Int32)inputStream.SampleRate);
            writer.Write((Int32)(inputStream.SampleRate * numberOfChannels * inputStream.BytesPerSample)); // Bytes / sec
            writer.Write((Int16)(numberOfChannels * inputStream.BytesPerSample)); // Block Align
            writer.Write((Int16)inputStream.BitsPerSample);
            writer.Write("data".ToCharArray());
            writer.Write((Int32)dataLength);
            writer.Flush();
        }

        #region ReboundOutputStream Members

        public void WriteSamples(Int16[] values)
        {
            foreach (Int16 value in values)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                outputStream.Write(bytes, 0, 2);
            }
        }

        #endregion
    }

    class UnsupportedFormatException : Exception
    {
        public UnsupportedFormatException(string message) : base(message) {}
    }
}