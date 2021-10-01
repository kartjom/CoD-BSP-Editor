using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoD_BSP_Editor.BSP
{
    public class Lump
    {
        public int Length { get; set; }
        public int Offset { get; set; }

        public Lump(int _length, int _offset)
        {
            this.Length = _length;
            this.Offset = _offset;
        }

        public byte[] ToByteArray()
        {
            byte[] _lengthBytes = new byte[4];
            byte[] _offsetBytes = new byte[4];

            BinaryPrimitives.TryWriteInt32LittleEndian(_lengthBytes, this.Length);
            BinaryPrimitives.TryWriteInt32LittleEndian(_offsetBytes, this.Offset);

            byte[] _lumpData = _lengthBytes.Concat(_offsetBytes).ToArray();
            return _lumpData;
        }

        public static byte[] ListToByteArray(Lump[] lumpList)
        {
            List<byte> byteArray = new List<byte>();

            foreach (Lump lump in lumpList)
            {
                byte[] data = lump.ToByteArray();
                byteArray.AddRange(data);
            }

            return byteArray.ToArray();
        }
    }
}
