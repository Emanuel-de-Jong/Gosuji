namespace Gosuji.API.Helpers
{
    public class ByteUtils
    {
        private byte[] buffer;
        private int index;

        public ByteUtils(int initialCapacity = 256)
        {
            buffer = new byte[initialCapacity];
            index = 0;
        }

        public ByteUtils(byte[] data)
        {
            buffer = data;
            index = 0;
        }

        private void EnsureCapacity(int additionalBytes)
        {
            if (index + additionalBytes > buffer.Length)
            {
                int newCapacity = Math.Max(buffer.Length * 2, index + additionalBytes);
                Array.Resize(ref buffer, newCapacity);
            }
        }

        public void AddInt(int value, int byteCount, bool isUnsigned = false)
        {
            EnsureCapacity(byteCount);
            for (int i = 0; i < byteCount; i++)
            {
                buffer[index++] = (byte)(value >> (8 * i));
            }
        }

        public void AddShort(short value, int byteCount, bool isUnsigned = false)
        {
            EnsureCapacity(byteCount);
            for (int i = 0; i < byteCount; i++)
            {
                buffer[index++] = (byte)(value >> (8 * i));
            }
        }

        public void AddLong(long value, int byteCount, bool isUnsigned = false)
        {
            EnsureCapacity(byteCount);
            for (int i = 0; i < byteCount; i++)
            {
                buffer[index++] = (byte)(value >> (8 * i));
            }
        }

        public void AddChar(char value)
        {
            EnsureCapacity(2);
            buffer[index++] = (byte)value;
            buffer[index++] = (byte)(value >> 8);
        }

        public void AddEnum<TEnum>(TEnum value, int byteCount, bool isUnsigned = false) where TEnum : Enum
        {
            AddInt(Convert.ToInt32(value), byteCount, isUnsigned);
        }

        public int ExtractInt(int byteCount, bool isUnsigned = false)
        {
            int result = 0;
            for (int i = 0; i < byteCount; i++)
            {
                result |= buffer[index++] << (8 * i);
            }
            if (!isUnsigned && byteCount < 4 && (result & (1 << (8 * byteCount - 1))) != 0)
            {
                result |= -1 << (8 * byteCount);
            }
            return result;
        }

        public short ExtractShort(int byteCount, bool isUnsigned = false)
        {
            int result = 0;
            for (int i = 0; i < byteCount; i++)
            {
                result |= buffer[index++] << (8 * i);
            }
            if (!isUnsigned && byteCount < 2 && (result & (1 << (8 * byteCount - 1))) != 0)
            {
                result |= -1 << (8 * byteCount);
            }
            return (short)result;
        }

        public long ExtractLong(int byteCount, bool isUnsigned = false)
        {
            long result = 0;
            for (int i = 0; i < byteCount; i++)
            {
                result |= (long)buffer[index++] << (8 * i);
            }
            if (!isUnsigned && byteCount < 8 && (result & (1L << (8 * byteCount - 1))) != 0)
            {
                result |= -1L << (8 * byteCount);
            }
            return result;
        }

        public char ExtractChar()
        {
            char result = (char)(buffer[index++] | (buffer[index++] << 8));
            return result;
        }

        public TEnum ExtractEnum<TEnum>(int byteCount, bool isUnsigned = false) where TEnum : Enum
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), ExtractInt(byteCount, isUnsigned));
        }

        public byte[] ToArray()
        {
            byte[] result = new byte[index];
            Array.Copy(buffer, result, index);
            return result;
        }
    }
}
