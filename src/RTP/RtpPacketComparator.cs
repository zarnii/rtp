using System.Collections.Generic;

namespace RTP
{
    public class RtpPacketComparator : IComparer<RtpPacket>
    {
        public int Compare(RtpPacket x, RtpPacket y)
        {
            if (x.Header.SequenceNumber == y.Header.SequenceNumber) 
            {
                return 0;
            }

            var difference = unchecked((short)(x.Header.SequenceNumber - y.Header.SequenceNumber));

            if (difference == short.MinValue)
            {
                return x.Header.SequenceNumber < y.Header.SequenceNumber
                    ? -1 
                    : 1;
            }

            return difference;
        }
    }
}
