using System;
using System.Buffers;
using System.Collections.Generic;

namespace RTP
{
    /// <summary>
    /// Джиттер-буфер. Принмает пакеты из RTP сессии и сглаживает их.
    /// </summary>
    internal class JitterBuffer
    {
        /// <summary>
        /// Буфферная задержка в миллисекундах.
        /// </summary>
        private readonly int _bufferDelayMs;

        /// <summary>
        /// Длительность одного кадра в миллисекундах.
        /// </summary>
        private readonly int _frameDurationMs;

        /// <summary>
        /// Флаг, показывающий находится ли буфер
        /// в определенный момент времени в режиме
        /// накопления пакетов.
        /// </summary>
        private bool _isBuffering;

        // Unwrapping long???
        private SortedSet<RtpPacket> _buffer;

        /// <summary>
        /// Пул массивов.
        /// </summary>
        private ArrayPool<byte> _pool;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="bufferDelayMs">Буфферная задержка в миллисекундах.</param>
        /// <param name="frameDurationMs">Длительность одного кадра в миллисекундах.</param>
        public JitterBuffer(int bufferDelayMs, int frameDurationMs)
        {
            // ArgumentOutOfRangeException.

            _bufferDelayMs = bufferDelayMs;
            _frameDurationMs = frameDurationMs;

            _pool = ArrayPool<byte>.Create();
            _buffer = new SortedSet<RtpPacket>(new RtpPacketComparator());
        }

        public void Push(RtpHeader header, Span<byte> payload)
        {
            var packet = new RtpPacket
            {
                Header = header,
                Payload = _pool.Rent(payload.Length)
            };

            payload.CopyTo(packet.Payload);
            _buffer.Add(packet);

            _isBuffering = _buffer.Count * _frameDurationMs < _bufferDelayMs;
        }

        public void Pop(RtpPacket[] packets) 
        {
            if (!_isBuffering)
            {
                return;
            }

            if (packets.Length < _buffer.Count)
            {
                return;
            }

            foreach (var packet in _buffer)
            {

            }
        }
    }
}
