namespace Gosuji.API.Helpers
{
    public class ByteUtils
    {
        public static byte[] NumToBytes(int num, int byteCount = 4, byte[]? arr = null)
        {
            byte[] bytes = new byte[byteCount];

            for (int i = byteCount - 1; i >= 0; i--)
            {
                bytes[i] = (byte)(num & 0xff);
                num >>= 8; // Shift the number by 8 bits (1 byte)
            }

            if (arr == null)
            {
                return bytes;
            }

            return arr.Concat(bytes).ToArray();
        }

        public static int BytesToNum(byte[] arr, int byteCount = 4, int offset = 0)
        {
            int num = 0;

            for (int i = 0; i < byteCount; i++)
            {
                num <<= 8; // Shift left by 8 bits (1 byte)
                num += arr[i + offset]; // Add the byte value
            }

            return num;
        }
    }
}
