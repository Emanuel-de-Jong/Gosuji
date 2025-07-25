﻿using System.Buffers.Binary;
using System.Text;

namespace Gosuji.API.Helpers
{
    // Int
    // Bytes | Signed Min                 | Signed Max                | Unsigned Max
    // ------|----------------------------|---------------------------|---------------------------
    // 1     | -128                       | 127                       | 255
    // 2     | -32,768                    | 32,767                    | 65,535
    // 3     | -8,388,608                 | 8,388,607                 | 16,777,215
    // 4     | -2,147,483,648             | 2,147,483,647             | 4,294,967,295
    // 5     | -549,755,813,888           | 549,755,813,887           | 1,099,511,627,775
    // 6     | -140,737,488,355,328       | 140,737,488,355,327       | 281,474,976,710,655
    // 7     | -36,028,797,018,963,968    | 36,028,797,018,963,967    | 72,057,594,037,927,935
    // 8     | -9,223,372,036,854,775,808 | 9,223,372,036,854,775,807 | 18,446,744,073,709,551,615
    // 
    // Double fractional bytes
    // Bytes | Certain Precision | Likely Precision
    // ------|-------------------|------------------
    // 1     | ~3                | ~5
    // 2     | ~4                | ~6
    // 3     | ~5                | ~7
    // 4     | ~6                | ~8
    // 5     | ~7                | ~9
    // 6     | ~8                | ~10
    // 7     | ~9                | ~11
    // 8     | ~10               | ~12
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

        public void AddBool(bool value)
        {
            EnsureCapacity(1);
            encodeBuffer.Add(value ? (byte)1 : (byte)0);
        }

        public void AddString(string value, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            byte[] stringBytes = encoding.GetBytes(value);
            AddInt(stringBytes.Length, 4, true);
            EnsureCapacity(stringBytes.Length);
            encodeBuffer.AddRange(stringBytes);
        }

        public void AddByte(byte value)
        {
            EnsureCapacity(1);
            encodeBuffer.Add(value);
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

        public void AddFloat(float value, int precision = 4, bool isUnsigned = false)
        {
            int factor = (int)Math.Pow(10, precision);
            int intValue = (int)(value * factor);
            AddInt(intValue, 4, isUnsigned);
        }

        public void AddDouble(double value, int integerBytes = 4, int fractionalBytes = 4, bool isUnsigned = false)
        {
            EnsureCapacity(integerBytes + fractionalBytes);

            long integerPart = (long)value;
            double fractionalPart = value - integerPart;

            Span<byte> intSpan = stackalloc byte[integerBytes];
            Span<byte> fracSpan = stackalloc byte[fractionalBytes];

            if (isUnsigned)
            {
                BinaryPrimitives.WriteUInt64LittleEndian(intSpan, (ulong)integerPart);
            }
            else
            {
                BinaryPrimitives.WriteInt64LittleEndian(intSpan, integerPart);
            }

            long fractionalEncoded = (long)(fractionalPart * Math.Pow(10, fractionalBytes));
            BinaryPrimitives.WriteInt64LittleEndian(fracSpan, fractionalEncoded);

            encodeBuffer.AddRange(intSpan[..integerBytes].ToArray());
            encodeBuffer.AddRange(fracSpan[..fractionalBytes].ToArray());
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

        public bool ExtractBool()
        {
            return decodeBuffer[decodeIndex++] == 1;
        }

        public string ExtractString(Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            int length = ExtractInt(4, true);
            string result = encoding.GetString(decodeBuffer.AsSpan(decodeIndex, length));
            decodeIndex += length;
            return result;
        }

        public byte ExtractByte()
        {
            return decodeBuffer[decodeIndex++];
        }

        public short ExtractShort(int byteCount, bool isUnsigned = false)
        {
            Span<byte> span = decodeBuffer.AsSpan(decodeIndex, byteCount);
            decodeIndex += byteCount;
            return isUnsigned
                ? (short)BinaryPrimitives.ReadUInt16LittleEndian(span)
                : BinaryPrimitives.ReadInt16LittleEndian(span);
        }

        public int ExtractInt(int byteCount, bool isUnsigned = false)
        {
            Span<byte> span = decodeBuffer.AsSpan(decodeIndex, byteCount);
            decodeIndex += byteCount;
            return isUnsigned
                ? (int)BinaryPrimitives.ReadUInt32LittleEndian(span)
                : BinaryPrimitives.ReadInt32LittleEndian(span);
        }

        public long ExtractLong(int byteCount, bool isUnsigned = false)
        {
            Span<byte> span = decodeBuffer.AsSpan(decodeIndex, byteCount);
            decodeIndex += byteCount;
            return isUnsigned
                ? (long)BinaryPrimitives.ReadUInt64LittleEndian(span)
                : BinaryPrimitives.ReadInt64LittleEndian(span);
        }

        public float ExtractFloat(int precision = 4, bool isUnsigned = false)
        {
            int factor = (int)Math.Pow(10, precision);
            int intValue = ExtractInt(4, isUnsigned);
            return (float)intValue / factor;
        }

        public double ExtractDouble(int integerBytes = 4, int fractionalBytes = 4, bool isUnsigned = false)
        {
            long integerPart = ExtractLong(integerBytes, isUnsigned);
            long fractionalPart = ExtractLong(fractionalBytes, isUnsigned);

            return integerPart + (fractionalPart / Math.Pow(10, fractionalBytes));
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
