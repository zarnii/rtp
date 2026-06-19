using System;

namespace RTP
{
    /// <summary>
    /// Джиттер-буфер. Принмает пакеты из RTP сессии и сглаживает их.
    /// </summary>
    internal class JitterBuffer
    {
        private readonly byte[] _buffer;

        /// <summary>
        /// Событие, сигнализирующее о заполнении буфера.
        /// </summary>
        public event Action<byte[]>? OnBufferFillng;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="buffer">Буфер, в который будут кладываться сэмлы.</param>
        public JitterBuffer(byte[] buffer)
        {
            ArgumentNullException.ThrowIfNull(buffer, nameof(buffer));

            _buffer = buffer;
        }

        public void ParseRawPacket(Span<byte> rawPacket)
        {
            if (rawPacket.Length < RtpHeader.FixedHeaderSize)
            {
                return; // ???
            }


        }
    }
}
