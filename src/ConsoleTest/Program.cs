using RTP;
using System.Buffers;
using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;

namespace ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            var header = new RtpHeader()
            {
                Version = 2,
                Padding = true,
                Extension = true,
                CsrcCount = 15,
                Marker = true,
                PayloadType = RtpPayloadType.Dynamic127,
                SequenceNumber = 65535,
                Timestamp = uint.MaxValue,
                Ssrc = uint.MaxValue,
            };*/
            var header = new RtpHeader()
            {
                Version = 2,
                CsrcCount = 5,
                Marker = true,
                Ssrc = uint.MaxValue
            };
            var bufferPool = ArrayPool<byte>.Create();
            var buffer = bufferPool.Rent(12);

            header.GetBytes(buffer.AsMemory());
            Console.WriteLine(BinaryDebugLibrary.BinaryWriter.ConvertBytesToBinaryString(buffer));
        }
    }
}
