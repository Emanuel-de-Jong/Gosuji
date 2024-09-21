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

        public void DecodeInit(byte[] ddecodeBuffer)
        {
            this.decodeBuffer = ddecodeBuffer;

            decodeIndex = 0;
        }

        public void AddInt(int value, int byteCount, bool isUnsigned = false)
        {
            Span<byte> span = stackalloc byte[4]; // 4 bytes for int

            if (isUnsigned)
                BinaryPrimitives.WriteUInt32LittleEndian(span, (uint)value);
            else
                BinaryPrimitives.WriteInt32LittleEndian(span, value);

            encodeBuffer.AddRange(span.Slice(0, byteCount).ToArray());
        }

        public void AddShort(short value, int byteCount, bool isUnsigned = false)
        {
            Span<byte> span = stackalloc byte[2]; // 2 bytes for short

            if (isUnsigned)
                BinaryPrimitives.WriteUInt16LittleEndian(span, (ushort)value);
            else
                BinaryPrimitives.WriteInt16LittleEndian(span, value);

            encodeBuffer.AddRange(span.Slice(0, byteCount).ToArray());
        }

        public void AddLong(long value, int byteCount, bool isUnsigned = false)
        {
            Span<byte> span = stackalloc byte[8]; // 8 bytes for long

            if (isUnsigned)
                BinaryPrimitives.WriteUInt64LittleEndian(span, (ulong)value);
            else
                BinaryPrimitives.WriteInt64LittleEndian(span, value);

            encodeBuffer.AddRange(span.Slice(0, byteCount).ToArray());
        }

        public void AddChar(char value)
        {
            Span<byte> span = stackalloc byte[2]; // Char is 2 bytes
            BinaryPrimitives.WriteUInt16LittleEndian(span, value);
            encodeBuffer.AddRange(span.ToArray());
        }

        public void AddEnum<TEnum>(TEnum value, int byteCount, bool isUnsigned = false) where TEnum : Enum
        {
            AddInt(Convert.ToInt32(value), byteCount, isUnsigned);
        }

        public int ExtractInt(int byteCount, bool isUnsigned = false)
        {
            var span = new ReadOnlySpan<byte>(decodeBuffer, decodeIndex, byteCount);
            decodeIndex += byteCount;

            return isUnsigned
                ? (int)BinaryPrimitives.ReadUInt32LittleEndian(span)
                : BinaryPrimitives.ReadInt32LittleEndian(span);
        }

        public short ExtractShort(int byteCount, bool isUnsigned = false)
        {
            var span = new ReadOnlySpan<byte>(decodeBuffer, decodeIndex, byteCount);
            decodeIndex += byteCount;

            return isUnsigned
                ? (short)BinaryPrimitives.ReadUInt16LittleEndian(span)
                : BinaryPrimitives.ReadInt16LittleEndian(span);
        }

        public long ExtractLong(int byteCount, bool isUnsigned = false)
        {
            var span = new ReadOnlySpan<byte>(decodeBuffer, decodeIndex, byteCount);
            decodeIndex += byteCount;

            return isUnsigned
                ? (long)BinaryPrimitives.ReadUInt64LittleEndian(span)
                : BinaryPrimitives.ReadInt64LittleEndian(span);
        }

        public char ExtractChar()
        {
            var span = new ReadOnlySpan<byte>(decodeBuffer, decodeIndex, 2); // Char is 2 bytes
            decodeIndex += 2;

            return (char)BinaryPrimitives.ReadUInt16LittleEndian(span);
        }

        public TEnum ExtractEnum<TEnum>(int byteCount, bool isUnsigned = false) where TEnum : Enum
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), ExtractInt(byteCount, isUnsigned));
        }
    }
}
