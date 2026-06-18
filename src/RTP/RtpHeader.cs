using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace RTP
{
    /// <summary>
    /// Заголовок RTP пакета.
    /// Согласно RFC 3550 https://datatracker.ietf.org/doc/html/rfc3550#section-5.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RtpHeader
    {
        #region consts
        /// <summary>
        /// Битовая маска сброса битов версии.
        /// </summary>
        private const ushort VersionMaskReset = 0x3F;

        /// <summary>
        /// Битовая маска для получения битов заполнения.
        /// </summary>
        private const ushort PaddingMaskGet = 0b0010_0000_0000_0000;

        /// <summary>
        /// Битовая маска для установки значения заполния в true.
        /// </summary>
        private const ushort PaddingMaskSetTrue = 0b0010_0000_0000_0000;

        /// <summary>
        /// Битовая маска для установки значения заполнения в false.
        /// </summary>
        private const ushort PaddingMaskSetFalse = 0b1101_1111_1111_1111;

        /// <summary>
        /// Битовая маска для получения битов расширения.
        /// </summary>
        private const ushort ExtensionMaskGet = 0b0001_0000_0000_0000;

        /// <summary>
        /// Битовая маска для установки значения расширения в true.
        /// </summary>
        private const ushort ExtensionMaskSetTrue = 0b0001_0000_0000_0000;

        /// <summary>
        /// Битовая маска для установки значения расширения в false.
        /// </summary>
        private const ushort ExtensionMaskSetFalse = 0b1110_1111_1111_1111;

        /// <summary>
        /// Максимальное количество источников вклада.
        /// </summary>
        private const int CsrcMaxCount = 15;

        /// <summary>
        /// Битовая маска сброса битов количества источников вклада.
        /// </summary>
        private const ushort CsrcMaskReset = 0xF0FF;

        /// <summary>
        /// Битовая маска для получения битов количества источников вклада.
        /// </summary>
        private const ushort CsrcMaskGet = 0b0000_1111_0000_0000;

        /// <summary>
        /// Битовая маска для получения битов маркера.
        /// </summary>
        private const ushort MarkerMaskGet = 0b0000_0000_1000_0000;

        /// <summary>
        /// Битовая маска для установки значения маркера в true.
        /// </summary>
        private const ushort MarkerMaskSetTrue = 0b0000_0000_1000_0000;

        /// <summary>
        /// Битовая маска для установки значения маркера в false.
        /// </summary>
        private const ushort MarkerMaskSetFalse = 0b1111_1111_0111_1111;

        /// <summary>
        /// Битовая маска сброса битов типа полезной нагрузки.
        /// </summary>
        private const ushort PayloadTypeMaskReset = 0xFF80;

        /// <summary>
        /// Битовая маска для получения битов полезной нагрузки.
        /// </summary>
        private const ushort PayloadTypeMaskGet = 0b0000_0000_0111_1111;

        /// <summary>
        /// Размер фиксированного заголовка в байтах.
        /// Согласно RFC 3550 https://datatracker.ietf.org/doc/html/rfc3550#section-5.
        /// </summary>
        public const int FixedHeaderSize = 12;
        #endregion

        /// <summary>
        /// Первые 16 бит заголовка.
        /// В следующей последовательности в них кодируется:
        /// Version - 2 бита,
        /// Padding - 1 бит,
        /// Extension - 1 бит,
        /// CSRC count - 4 бита,
        /// Maker - 1 бит,
        /// Payload type - 7 бит
        /// </summary>
        private ushort _first16bit;

        /// <summary>
        /// Порядковый номер.
        /// </summary>
        private ushort _sequenceNumber;

        /// <summary>
        /// Временная метка.
        /// </summary>
        private uint _timestamp;

        /// <summary>
        /// Источник синхронизации.
        /// </summary>
        private uint _ssrc;

        /// <summary>
        /// Версия протокола.
        /// </summary>
        public int Version
        {
            get
            {
                return (ushort)(_first16bit >> 14);
            }
            set
            {
                _first16bit &= VersionMaskReset;
                _first16bit |= (ushort)(value << 14);
            }
        }

        /// <summary>
        /// Заполнение.
        /// </summary>
        public bool Padding
        {
            get
            {
                return (_first16bit & PaddingMaskGet) > 1;
            }
            set
            {

                if (value)
                {
                    _first16bit |= PaddingMaskSetTrue;
                }
                else
                {
                    _first16bit &= PaddingMaskSetFalse;
                }
            }
        }

        /// <summary>
        /// Расширение.
        /// </summary>
        public bool Extension
        {
            get
            {
                return (_first16bit & ExtensionMaskGet) > 1;
            }
            set
            {
                if (value)
                {
                    _first16bit |= ExtensionMaskSetTrue;
                }
                else
                {
                    _first16bit &= ExtensionMaskSetFalse;
                }
            }
        }

        /// <summary>
        /// Количество источников вклада (контрибуции).
        /// </summary>
        public int CsrcCount
        {
            get
            {
                return (_first16bit & CsrcMaskGet) >> 8;
            }
            set
            {
                _first16bit &= CsrcMaskReset;
                _first16bit |= (ushort)(value << 8);
            }
        }

        /// <summary>
        /// Маркер.
        /// </summary>
        public bool Marker
        {
            get
            {
                return (_first16bit & MarkerMaskGet) > 1;
            }
            set
            {
                if (value)
                {
                    _first16bit |= MarkerMaskSetTrue;
                }
                else
                {
                    _first16bit &= MarkerMaskSetFalse;
                }
            }
        }

        /// <summary>
        /// Тип полезной нагрузки.
        /// </summary>
        public RtpPayloadType PayloadType
        {
            get
            {
                return (RtpPayloadType)(_first16bit & PayloadTypeMaskGet);
            }
            set
            {
                _first16bit &= PayloadTypeMaskReset;
                _first16bit |= (ushort)value;
            }
        }

        /// <summary>
        /// Порядковый номер.
        /// </summary>
        public ushort SequenceNumber
        {
            get
            {
                return _sequenceNumber;
            }
            set
            {
                _sequenceNumber = value;
            }
        }

        /// <summary>
        /// Временная метка.
        /// </summary>
        public uint Timestamp
        {
            get
            {
                return _timestamp;
            }
            set
            {
                _timestamp = value;
            }
        }

        /// <summary>
        /// Источник синхронизации.
        /// </summary>
        public uint Ssrc
        {
            get
            {
                return _ssrc;
            }
            set
            {
                _ssrc = value;
            }
        }

        /// <summary>
        /// Получение байтов заголовка в сетевом порядке.
        /// </summary>
        /// <param name="buffer">Буфер, в который будут записаны байты</param>
        public void GetNetworkOrderBytes(in Memory<byte> buffer)
        {
            if (buffer.Length < 12)
            {
                return;
            }

            var bufferSpan = buffer.Span;

            BinaryPrimitives.WriteUInt16BigEndian(bufferSpan.Slice(0, 2), _first16bit);
            BinaryPrimitives.WriteUInt16BigEndian(bufferSpan.Slice(2, 2), _sequenceNumber);
            BinaryPrimitives.WriteUInt32BigEndian(bufferSpan.Slice(4, 4), _timestamp);
            BinaryPrimitives.WriteUInt32BigEndian(bufferSpan.Slice(8, 4), _ssrc);         
        }
    }
}
