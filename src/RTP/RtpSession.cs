using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace RTP
{
    /// <summary>
    /// Логическая ассоциация между группой участников,
    /// взаимодействующих по протоколу RTP.
    /// https://datatracker.ietf.org/doc/html/rfc3550#section-3
    /// </summary>
    public class RtpSession: IDisposable
    {
        private const int RtpVersionByDefault = 2;

        private const int ContributingSourceCountByDefault = 0;

        private const int MaxUdpDatagramSize = 65536;

        // Идентифицируется Источником синхронизации SSRC.

        private readonly uint _ssrc;

        private uint _timestamp;

        private ushort _sequenceNumber;

        private readonly Socket _socket;

        private IPEndPoint _destinationEndpoint;

        private readonly ArrayPool<byte> _pool;

        private bool _isDisposed = false;

        public uint Ssrc
        {
            get
            {
                return _ssrc;
            }
        }

        public RtpSession(IPEndPoint localEndpoint, IPEndPoint destinationEndpoint, uint ssrc = 0)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(localEndpoint);
            _destinationEndpoint = destinationEndpoint;

            _ssrc = ssrc == 0
                ? GenerateRandomUint32Number()
                : ssrc;

            _timestamp = GenerateRandomUint32Number();
            _sequenceNumber = (ushort)RandomNumberGenerator.GetInt32(0, ushort.MaxValue);
            

            _pool = ArrayPool<byte>.Create();
        }

        ~RtpSession()
        {
            Dispose();
        }

        public async Task StartReceive(Action<ArraySegment<byte>> onReceive, CancellationToken cancellationToken)
        {

            await Task.Run(async () =>
            {
                var buffer = _pool.Rent(MaxUdpDatagramSize);
                var remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);

                while (!cancellationToken.IsCancellationRequested)
                {

                    var receiveResult = await _socket.ReceiveFromAsync(new ArraySegment<byte>(buffer), 
                        remoteEndpoint, 
                        cancellationToken);

                    onReceive?.Invoke(new ArraySegment<byte>(buffer, 0, receiveResult.ReceivedBytes));
                }

                _pool.Return(buffer);
            }, cancellationToken);
        }

        public async Task Send(Memory<byte> samples, RtpPayloadType codec, int samplingRate, int durationMs)
        { 
            var header = new RtpHeader()
            {
                Version = RtpVersionByDefault,
                Padding = false,
                Extension = false,
                CsrcCount = ContributingSourceCountByDefault,
                Marker = false,
                PayloadType = codec,
                SequenceNumber = _sequenceNumber,
                Timestamp = _timestamp,
                Ssrc = _ssrc,
            };

            // Создаем буфер или берем из пула для отправки данных.
            /*
             * var buffer = Pool.GetBuffer();
             * buffer.AddHeader(header);
             * buffer.AddPayload(samples);
             * 
             * _socket.SendTo(buffer)
             * 
             * Pool.FreeBuffer(buffer);
            */

            var packetSize = RtpHeader.FixedHeaderSize + samples.Length;
            var buffer = _pool.Rent(packetSize);
            var headerBuffer = new ArraySegment<byte>(buffer, 0, RtpHeader.FixedHeaderSize);
            
            header.GetNetworkOrderBytes(headerBuffer);
            samples.CopyTo(new ArraySegment<byte>(buffer, RtpHeader.FixedHeaderSize, samples.Length));

            await _socket.SendToAsync(new ArraySegment<byte>(buffer, 0, packetSize), _destinationEndpoint);

            _pool.Return(buffer);

            unchecked
            {
                _sequenceNumber++;
                _timestamp += (uint)(durationMs * samplingRate) / 1000;
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                return;
            }

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            _isDisposed = true;

            GC.SuppressFinalize(this);
        }

        private uint GenerateRandomUint32Number()
        {
            var first16bit = RandomNumberGenerator.GetInt32(0, ushort.MaxValue);
            var second16bit = RandomNumberGenerator.GetInt32(0, short.MaxValue);

            return (uint)((first16bit << 16) | second16bit);
        }
    }
}
