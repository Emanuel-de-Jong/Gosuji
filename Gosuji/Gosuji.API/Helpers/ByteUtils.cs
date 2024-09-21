using System.Buffers.Binary;

namespace Gosuji.API.Helpers
{
    public class ByteUtils
    {
        private List<byte> encodeBuffer;
        private byte[] decodeBuffer;
        private int decodeIndex;

        public void EncodeInit(List<byte> encodeBuffer)
        {
            this.encodeBuffer = encodeBuffer;
        }

        public void DecodeInit(byte[] decodeBuffer)
        {
            this.decodeBuffer = decodeBuffer;
            decodeIndex = 0;
        }

        public void AddInt(int value, int byteCount, bool isUnsigned = false)
        {
            EnsureCapacity(byteCount);
            Span<byte> span = stackalloc byte[4];
            if (isUnsigned)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(span, (uint)value);
            }
            else
            {
                BinaryPrimitives.WriteInt32LittleEndian(span, value);
            }

            encodeBuffer.AddRange(span[..byteCount].ToArray());
        }

        public void AddShort(short value, int byteCount, bool isUnsigned = false)
        {
            EnsureCapacity(byteCount);
            Span<byte> span = stackalloc byte[2];
            if (isUnsigned)
            {
                BinaryPrimitives.WriteUInt16LittleEndian(span, (ushort)value);
            }
            else
            {
                BinaryPrimitives.WriteInt16LittleEndian(span, value);
            }

            encodeBuffer.AddRange(span[..byteCount].ToArray());
        }

        public void AddLong(long value, int byteCount, bool isUnsigned = false)
        {
            EnsureCapacity(byteCount);
            Span<byte> span = stackalloc byte[8];
            if (isUnsigned)
            {
                BinaryPrimitives.WriteUInt64LittleEndian(span, (ulong)value);
            }
            else
            {
                BinaryPrimitives.WriteInt64LittleEndian(span, value);
            }

            encodeBuffer.AddRange(span[..byteCount].ToArray());
        }

        public void AddDouble(double value, int byteCount)
        {
            EnsureCapacity(byteCount);
            Span<byte> span = stackalloc byte[8];
            BitConverter.TryWriteBytes(span, value);
            encodeBuffer.AddRange(span[..byteCount].ToArray());
        }

        public void AddChar(char value)
        {
            EnsureCapacity(2);
            Span<byte> span = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(span, value);
            encodeBuffer.AddRange(span.ToArray());
        }

        public void AddEnum<TEnum>(TEnum value, int byteCount, bool isUnsigned = false) where TEnum : Enum
        {
            AddInt(Convert.ToInt32(value), byteCount, isUnsigned);
        }

        public int ExtractInt(int byteCount, bool isUnsigned = false)
        {
            Span<byte> span = decodeBuffer.AsSpan(decodeIndex, byteCount);
            decodeIndex += byteCount;
            return isUnsigned
                ? (int)BinaryPrimitives.ReadUInt32LittleEndian(span)
                : BinaryPrimitives.ReadInt32LittleEndian(span);
        }

        public short ExtractShort(int byteCount, bool isUnsigned = false)
        {
            Span<byte> span = decodeBuffer.AsSpan(decodeIndex, byteCount);
            decodeIndex += byteCount;
            return isUnsigned
                ? (short)BinaryPrimitives.ReadUInt16LittleEndian(span)
                : BinaryPrimitives.ReadInt16LittleEndian(span);
        }

        public long ExtractLong(int byteCount, bool isUnsigned = false)
        {
            Span<byte> span = decodeBuffer.AsSpan(decodeIndex, byteCount);
            decodeIndex += byteCount;
            return isUnsigned
                ? (long)BinaryPrimitives.ReadUInt64LittleEndian(span)
                : BinaryPrimitives.ReadInt64LittleEndian(span);
        }

        public double ExtractDouble(int byteCount)
        {
            Span<byte> span = decodeBuffer.AsSpan(decodeIndex, byteCount);
            decodeIndex += byteCount;
            return BitConverter.ToDouble(span);
        }

        public char ExtractChar()
        {
            Span<byte> span = decodeBuffer.AsSpan(decodeIndex, 2);
            decodeIndex += 2;
            return (char)BinaryPrimitives.ReadUInt16LittleEndian(span);
        }

        public TEnum ExtractEnum<TEnum>(int byteCount, bool isUnsigned = false) where TEnum : Enum
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), ExtractInt(byteCount, isUnsigned));
        }

        private void EnsureCapacity(int additionalBytes)
        {
            if (encodeBuffer.Count + additionalBytes > encodeBuffer.Capacity)
            {
                int newCapacity = Math.Max(encodeBuffer.Capacity * 2, encodeBuffer.Count + additionalBytes);
                encodeBuffer.Capacity = newCapacity;
            }
        }
    }
}
