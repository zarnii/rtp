using System;

namespace RTP
{
    /// <summary>
    /// RTP пакет.
    /// </summary>
    public struct RtpPacket
    {
        /// <summary>
        /// Заголовок.
        /// </summary>
        public RtpHeader Header { get; set; }

        /// <summary>
        /// Полезная нагрузка.
        /// </summary>
        public byte[] Payload { get; set; }

        /// <summary>
        /// Копирование пакета.
        /// </summary>
        /// <param name="packet">Пакет, в который скопируются данные.</param>
        public void CopyTo(ref RtpPacket packet)
        {
            packet.Header = Header;
            Payload.CopyTo(packet.Payload, 0);
        }
    }
}
