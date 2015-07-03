using System.IO;
using System.Text;

using fNbt;

namespace HQMFileConverter
{
    public sealed class BitStreamReader
    {
        private readonly BinaryReader stream;
        private byte curr;
        private byte idx;

        public BitStreamReader(BinaryReader stream)
        {
            this.stream = stream;
            this.curr = stream.ReadByte();
        }

        public int ReadInt32(int bitCount)
        {
            int value = 0;
            for (int i = 0; i < bitCount; i++)
            {
                if (this.ReadBoolean())
                {
                    value |= 1 << i;
                }
            }

            return value;
        }

        public string ReadString(int lengthCount)
        {
            int length = this.ReadInt32(lengthCount);
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = (byte)this.ReadInt32(8);
            }

            return Encoding.ASCII.GetString(bytes);
        }

        public bool ReadBoolean()
        {
            bool result = 0 != (this.curr & (1 << this.idx));

            if (++this.idx == 8)
            {
                try
                {
                    this.curr = this.stream.ReadByte();
                }
                catch (EndOfStreamException)
                {
                    // I think this works around a bug in HQM proper...
                    System.Diagnostics.Debug.WriteLine("working around premature end of stream, stack trace: " + new System.Diagnostics.StackTrace());
                    this.curr = 0;
                }

                this.idx = 0;
            }

            return result;
        }

        public NbtWrapper ReadNBT()
        {
            if (!this.ReadBoolean())
            {
                return null;
            }

            byte[] nbt = new byte[this.ReadInt32(15)];
            for (int nbtIndex = 0; nbtIndex < nbt.Length; nbtIndex++)
            {
                nbt[nbtIndex] = (byte)this.ReadInt32(8);
            }

            var nbtFile = new NbtFile();
            nbtFile.LoadFromBuffer(nbt, 0, nbt.Length, NbtCompression.GZip);
            NbtCompound rootTag = nbtFile.RootTag;

            return new NbtWrapper { OriginalData = nbt, RootTag = rootTag };
        }
    }
}
