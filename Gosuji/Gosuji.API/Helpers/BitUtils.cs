namespace Gosuji.API.Helpers
{
    public class BitUtils
    {
        private List<byte> encodeBuffer;
        private int encodeBitPosition;

        private byte currentByte;
        private int currentByteBits;

        private byte[] decodeBuffer;
        private int decodeBitPosition;

        public void EncodeInit()
        {
            encodeBuffer = new List<byte>();
            encodeBitPosition = 0;
            currentByte = 0;
            currentByteBits = 0;
        }

        public void DecodeInit(byte[] buffer)
        {
            decodeBuffer = buffer;
            decodeBitPosition = 0;
        }

        public void AddInt(int value, int bitCount, bool isSigned = false)
        {
            if (isSigned)
            {
                value = (value << (32 - bitCount)) >> (32 - bitCount);
            }

            WriteBits((uint)value, bitCount);
        }

        public int ExtractInt(int bitCount, bool isSigned = false)
        {
            uint rawValue = ReadBits(bitCount);
            if (isSigned && (rawValue & (1U << (bitCount - 1))) != 0)
            {
                return (int)(rawValue | (~0U << bitCount));
            }

            return (int)rawValue;
        }

        public void AddDouble(double value, int bitCount, int fractionalBits, bool isSigned = false)
        {
            double scale = Math.Pow(10, fractionalBits);
            int scaledValue = (int)Math.Round(value * scale);
            AddInt(scaledValue, bitCount, isSigned);
        }

        public double ExtractDouble(int bitCount, int fractionalBits, bool isSigned = false)
        {
            int scaledValue = ExtractInt(bitCount, isSigned);
            double scale = Math.Pow(10, fractionalBits);
            return scaledValue / scale;
        }

        public void AddEnum<TEnum>(TEnum value, int bitCount, bool isSigned = false) where TEnum : Enum
        {
            AddInt(Convert.ToInt32(value), bitCount, isSigned);
        }

        public TEnum ExtractEnum<TEnum>(int bitCount, bool isSigned = false) where TEnum : Enum
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), ExtractInt(bitCount, isSigned));
        }

        private void WriteBits(uint value, int bitCount)
        {
            while (bitCount > 0)
            {
                int bitsToWrite = Math.Min(bitCount, 8 - currentByteBits);
                uint mask = ((1U << bitsToWrite) - 1) << (bitCount - bitsToWrite);
                uint bits = (value & mask) >> (bitCount - bitsToWrite);

                currentByte |= (byte)(bits << (8 - currentByteBits - bitsToWrite));
                currentByteBits += bitsToWrite;
                bitCount -= bitsToWrite;

                if (currentByteBits == 8)
                {
                    encodeBuffer.Add(currentByte);
                    currentByte = 0;
                    currentByteBits = 0;
                }
            }
        }

        private uint ReadBits(int bitCount)
        {
            uint value = 0;
            while (bitCount > 0)
            {
                int byteIndex = decodeBitPosition >> 3;
                int bitOffset = decodeBitPosition & 7;

                byte currentByte = decodeBuffer[byteIndex];

                int bitsAvailable = 8 - bitOffset;
                int bitsToRead = Math.Min(bitCount, bitsAvailable);

                uint bits = (uint)(currentByte >> (bitsAvailable - bitsToRead)) & ((1U << bitsToRead) - 1);

                value = (value << bitsToRead) | bits;

                decodeBitPosition += bitsToRead;
                bitCount -= bitsToRead;
            }
            return value;
        }

        public byte[] ToArray()
        {
            if (currentByteBits > 0)
            {
                encodeBuffer.Add(currentByte);
            }
            return encodeBuffer.ToArray();
        }
    }
}
