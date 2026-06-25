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
        // Unwrapping long???
        private Dictionary<RtpHeader, byte[]> _buffer;
        private ArrayPool<byte> _pool;

        public JitterBuffer()
        {
            _pool = ArrayPool<byte>.Create();
            _buffer = new Dictionary<RtpHeader, byte[]>();
        }

        public void Push(RtpHeader header, Span<byte> payload)
        {
            // paylod будет копироваться в какй-нибудь массив.
        }

        public RtpHeader? Pop(/*out*/ Span<byte> payload) 
        {
            return null;
        }
    }
}
