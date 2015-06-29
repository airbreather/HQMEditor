using System.IO;
using System.Text;

using fNbt;

namespace HQMFileConverter
{
    public sealed class BitStreamWriter
    {
        private readonly BinaryWriter stream;
        private byte curr;
        private int idx;

        public BitStreamWriter(BinaryWriter stream)
        {
            this.stream = stream;
        }

        public void WriteInt32(int value, int bitCount)
        {
            for (int i = 0; i < bitCount; i++)
            {
                this.WriteBoolean(0 != (value & (1 << i)));
            }
        }

        public void WriteString(string value, int lengthCount)
        {
            this.WriteInt32(value.Length, lengthCount);
            foreach (byte b in Encoding.ASCII.GetBytes(value))
            {
                this.WriteInt32(b, 8);
            }
        }

        public void WriteBoolean(bool value)
        {
            if (value)
            {
                this.curr |= (byte)(1 << this.idx);
            }

            if (++this.idx == 8)
            {
                this.stream.Write(this.curr);
                this.curr = 0;
                this.idx = 0;
            }
        }

        public void WriteNBT(NbtWrapper value)
        {
            if (value == null)
            {
                this.WriteBoolean(false);
                return;
            }

            this.WriteBoolean(true);

            // Keep the original byte array if it's unchanged.
            byte[] data = value.Changed
                ? new NbtFile(value.RootTag).SaveToBuffer(NbtCompression.GZip)
                : value.OriginalData;

            this.WriteInt32(data.Length, 15);
            for (int i = 0; i < data.Length; i++)
            {
                this.WriteInt32(data[i], 8);
            }
        }

        public void WriteFinalBits()
        {
            if (this.idx > 0)
            {
                this.stream.Write(this.curr);
            }
        }
    }
}
