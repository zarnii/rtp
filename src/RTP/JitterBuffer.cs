using System;
using System.Buffers;
using System.Collections.Generic;

namespace RTP
{
    /// <summary>
    /// Джиттер-буфер. Принмает пакеты из RTP сессии и сглаживает их.
    /// </summary>
    public class JitterBuffer
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

        /// <summary>
        /// Буфер.
        /// </summary>
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
            _isBuffering = true;
        }

        /// <summary>
        /// Передача полезной нагрузки в буфер.
        /// </summary>
        /// <param name="header">Заголовок.</param>
        /// <param name="payload">Полезная нагрузка.</param>
        /// <returns>True, если удалось передать данные в буфер. Иначе - false.</returns>
        public bool Push(RtpHeader header, Span<byte> payload)
        {
            if (!_isBuffering)
            {
                return false;
            }

            var packet = new RtpPacket
            {
                Header = header,
                Payload = _pool.Rent(payload.Length)
            };

            payload.CopyTo(packet.Payload);
            _buffer.Add(packet);
            _isBuffering = _buffer.Count * _frameDurationMs < _bufferDelayMs;

            return true;
        }

        /// <summary>
        /// Получение накопленных пакетов.
        /// </summary>
        /// <param name="packets">Список, в который будут скопированы пакеты.</param>
        /// <returns>True, если удалось скопировать пакеты. Иначе - false.</returns>
        public bool Pop(RtpPacket[] packets) 
        {
            if (_isBuffering)
            {
                return false;
            }

            if (packets.Length < _buffer.Count)
            {
                return false;
            }

            _isBuffering = false;
            var packetIndex = 0;

            foreach (var packet in _buffer)
            {
                packet.CopyTo(ref packets[packetIndex]);
                _pool.Return(packet.Payload);
                packetIndex++;
            }

            _buffer.Clear();
            _isBuffering = true;

            return true;
        }
    }
}
