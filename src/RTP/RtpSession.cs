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
        /// <summary>
        /// Версия RTP по умолчанию.
        /// </summary>
        private const int RtpVersionByDefault = 2;

        /// <summary>
        /// Количество источников контрибуции по умолчанию.
        /// </summary>
        private const int ContributingSourceCountByDefault = 0;

        /// <summary>
        /// Максимальный размер датаграммы UDP.
        /// </summary>
        private const int MaxUdpDatagramSize = 65536;

        /// <summary>
        /// Источник синхронизации.
        /// </summary>
        private readonly uint _ssrc;

        /// <summary>
        /// Временная метка.
        /// </summary>
        private uint _timestamp;

        /// <summary>
        /// Порядковый номер.
        /// </summary>
        private ushort _sequenceNumber;

        /// <summary>
        /// Локальный сокет.
        /// </summary>
        private readonly Socket _socket;

        /// <summary>
        /// Эндпоинт клиента.
        /// </summary>
        private IPEndPoint _destinationEndpoint;

        /// <summary>
        /// Пул массивов. Используется для аренды массивов для 
        /// буферов получения и отправки данных.
        /// </summary>
        private readonly ArrayPool<byte> _pool;

        /// <summary>
        /// Флаг, показывающий освобождены ли неуправляемые ресурсы.
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// Источник синхронизации.
        /// </summary>
        public uint Ssrc
        {
            get
            {
                return _ssrc;
            }
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="localEndpoint">Локальный эндпоинт, к которому привяжется сокет.</param>
        /// <param name="destinationEndpoint">Эндпоинт клиента.</param>
        /// <param name="ssrc">Источник синхронизации.</param>
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

        /// <summary>
        /// Деструктор.
        /// </summary>
        ~RtpSession()
        {
            Dispose();
        }

        /// <summary>
        /// Запуск прослущивания входных пакетов.
        /// </summary>
        /// <param name="onReceive">Делегат, вызываемый при получении пакета.
        ///                         В делегат передаются сырые данные.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Отправка данных.
        /// </summary>
        /// <param name="samples">Сэмлы, представляющие аудио или видео фрагмент.</param>
        /// <param name="codec">Кодек, которым обработаны сэмлы.</param>
        /// <param name="samplingRate">Частота дискретизации.</param>
        /// <param name="durationMs">Длительность сэмплов в миллисекундах.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Освобождение неуправляемых ресурсов.
        /// </summary>
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

        /// <summary>
        /// Создание криптографического случайного беззнакового 32х битного числа.
        /// </summary>
        /// <returns>Случайное беззнаковое 32х битное число.</returns>
        private uint GenerateRandomUint32Number()
        {
            var first16bit = RandomNumberGenerator.GetInt32(0, ushort.MaxValue);
            var second16bit = RandomNumberGenerator.GetInt32(0, short.MaxValue);

            return (uint)((first16bit << 16) | second16bit);
        }
    }
}
