using System.Buffers;

namespace Gosuji.API.Helpers
{
    // Bits | Signed Min     | Signed Max    | Unsigned Max  
    // -----|----------------|---------------|---------------
    // 1    | -1             | 0             | 1             
    // 2    | -2             | 1             | 3             
    // 3    | -4             | 3             | 7             
    // 4    | -8             | 7             | 15            
    // 5    | -16            | 15            | 31            
    // 6    | -32            | 31            | 63            
    // 7    | -64            | 63            | 127           
    // 8    | -128           | 127           | 255           
    // 9    | -256           | 255           | 511           
    // 10   | -512           | 511           | 1,023         
    // 11   | -1,024         | 1,023         | 2,047         
    // 12   | -2,048         | 2,047         | 4,095         
    // 13   | -4,096         | 4,095         | 8,191         
    // 14   | -8,192         | 8,191         | 16,383        
    // 15   | -16,384        | 16,383        | 32,767        
    // 16   | -32,768        | 32,767        | 65,535        
    // 17   | -65,536        | 65,535        | 131,071       
    // 18   | -131,072       | 131,071       | 262,143       
    // 19   | -262,144       | 262,143       | 524,287       
    // 20   | -524,288       | 524,287       | 1,048,575     
    // 21   | -1,048,576     | 1,048,575     | 2,097,151     
    // 22   | -2,097,152     | 2,097,151     | 4,194,303     
    // 23   | -4,194,304     | 4,194,303     | 8,388,607     
    // 24   | -8,388,608     | 8,388,607     | 16,777,215    
    // 25   | -16,777,216    | 16,777,215    | 33,554,431    
    // 26   | -33,554,432    | 33,554,431    | 67,108,863    
    // 27   | -67,108,864    | 67,108,863    | 134,217,727   
    // 28   | -134,217,728   | 134,217,727   | 268,435,455   
    // 29   | -268,435,456   | 268,435,455   | 536,870,911   
    // 30   | -536,870,912   | 536,870,911   | 1,073,741,823 
    // 31   | -1,073,741,824 | 1,073,741,823 | 2,147,483,647 
    // 32   | -2,147,483,648 | 2,147,483,647 | 4,294,967,295 
    public class BitUtils
    {
        private ArrayBufferWriter<byte> encodeBuffer;
        private int encodeBitPosition;

        private byte currentByte;
        private int currentByteBits;

        private ReadOnlyMemory<byte> decodeBuffer;
        private int decodeBitPosition;

        public void EncodeInit()
        {
            encodeBuffer = new ArrayBufferWriter<byte>(256);
            encodeBitPosition = 0;
            currentByte = 0;
            currentByteBits = 0;
        }

        public void DecodeInit(ReadOnlyMemory<byte> buffer)
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
            else
            {
                value &= (1 << bitCount) - 1;
            }

            WriteBits((uint)value, bitCount);
        }

        public void AddInt(int? value, int bitCount, bool isSigned = false)
        {
            AddInt(value.Value, bitCount, isSigned);
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

        public void AddDouble(double value, int bitCount, int decimalPoints, bool isSigned = false)
        {
            double scale = Math.Pow(10, decimalPoints);
            int scaledValue = (int)Math.Round(value * scale);
            AddInt(scaledValue, bitCount, isSigned);
        }

        public void AddDouble(double? value, int bitCount, int decimalPoints, bool isSigned = false)
        {
            AddDouble(value.Value, bitCount, decimalPoints, isSigned);
        }

        public double ExtractDouble(int bitCount, int decimalPoints, bool isSigned = false)
        {
            double scale = Math.Pow(10, decimalPoints);
            int scaledValue = ExtractInt(bitCount, isSigned);
            return scaledValue / scale;
        }

        public void AddEnum<TEnum>(TEnum value, int bitCount, bool isSigned = false) where TEnum : Enum
        {
            AddInt(Convert.ToInt32(value), bitCount, isSigned);
        }

        public void AddEnum<TEnum>(TEnum? value, int bitCount, bool isSigned = false) where TEnum : struct, Enum
        {
            AddEnum(value.Value, bitCount, isSigned);
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
                currentByte |= (byte)(((value >> (bitCount - bitsToWrite)) & ((1U << bitsToWrite) - 1)) << (8 - currentByteBits - bitsToWrite));

                currentByteBits += bitsToWrite;
                bitCount -= bitsToWrite;

                if (currentByteBits == 8)
                {
                    encodeBuffer.GetSpan(1)[0] = currentByte;
                    encodeBuffer.Advance(1);
                    currentByte = 0;
                    currentByteBits = 0;
                }
            }
        }

        private uint ReadBits(int bitCount)
        {
            uint value = 0;
            ReadOnlySpan<byte> span = decodeBuffer.Span;

            while (bitCount > 0)
            {
                int byteIndex = decodeBitPosition >> 3;
                int bitOffset = decodeBitPosition & 7;
                byte currentByte = span[byteIndex];

                int bitsAvailable = 8 - bitOffset;
                int bitsToRead = Math.Min(bitCount, bitsAvailable);

                value = (value << bitsToRead) | (uint)((currentByte >> (bitsAvailable - bitsToRead)) & ((1U << bitsToRead) - 1));

                decodeBitPosition += bitsToRead;
                bitCount -= bitsToRead;
            }
            return value;
        }

        public byte[] ToArray()
        {
            if (currentByteBits > 0)
            {
                encodeBuffer.GetSpan(1)[0] = currentByte;
                encodeBuffer.Advance(1);
            }
            return encodeBuffer.WrittenSpan.ToArray();
        }
    }
}
