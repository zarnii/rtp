namespace BinaryDebugLibrary
{
    public static class BinaryWriter
    {
        public static string ConvertNumberToBinaryString(ushort number)
        {
            var bytes = BitConverter.GetBytes(number);

            return String.Join(" ", bytes.Select((b) => Convert.ToString(b, 2).PadLeft(8, '0')));
        }

        public static string ConvertNumberToBinaryString(int number)
        {
            var bytes = BitConverter.GetBytes(number);

            return String.Join(" ", bytes.Select((b) => Convert.ToString(b, 2).PadLeft(8, '0')));
        }

        public static string ConvertBytesToBinaryString(Memory<byte> bytes)
        {
            var bytesArray = bytes.ToArray();

            return String.Join(" ", bytesArray.Select((b) => Convert.ToString(b, 2).PadLeft(8, '0')));
        }
    }
}
