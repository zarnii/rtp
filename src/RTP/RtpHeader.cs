using BinaryDebugLibrary;
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
        private readonly ushort VersionMaskReset = 0x3F;

        private readonly ushort PaddingMaskGet = BinaryPrimitives.ReverseEndianness((ushort)0b0010_0000_0000_0000);

        private readonly ushort PaddingMaskSetTrue = BinaryPrimitives.ReverseEndianness((ushort)0b0010_0000_0000_0000);

        private readonly ushort PaddingMaskSetFalse = BinaryPrimitives.ReverseEndianness((ushort)0b1101_1111_1111_1111);

        private readonly ushort ExtensionMaskGet = BinaryPrimitives.ReverseEndianness((ushort)0b0001_0000_0000_0000);

        private readonly ushort ExtensionMaskSetTrue = BinaryPrimitives.ReverseEndianness((ushort)0b0001_0000_0000_0000);

        private readonly ushort ExtensionMaskSetFalse = BinaryPrimitives.ReverseEndianness((ushort)0b1110_1111_1111_1111);

        private const int CsrcMaxCount = 15;

        private const ushort CsrcMaskReset = 0xF0FF;

        private const ushort CsrcMaskGet = 0b0000_1111_0000_0000;

        private readonly ushort MarkerMaskGet = BinaryPrimitives.ReverseEndianness((ushort)0b0000_0000_1000_0000);

        private readonly ushort MarkerMaskSetTrue = BinaryPrimitives.ReverseEndianness((ushort)0b0000_0000_1000_0000);

        private readonly ushort MarkerMaskSetFalse = BinaryPrimitives.ReverseEndianness((ushort)0b1111_1111_0111_1111);

        private const ushort PayloadTypeMaskReset = 0xFF80;

        private const ushort PayloadTypeMaskGet = (ushort)0b0000_0000_0111_1111;

        public const int HeaderSize = 12;
        #endregion

        /// <summary>
        /// Первые 16 бит заголовка. Хранится в сетевом порядке.
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
        /// Порядковый номер. Хранится в сетевом порядке.
        /// </summary>
        private ushort _sequenceNumber;

        /// <summary>
        /// Временная метка. Хранится в сетевом порядке.
        /// </summary>
        private uint _timestamp;

        /// <summary>
        /// Источник синхронизации. Хранится в сетевом порядке.
        /// </summary>
        private uint _ssrc;

        public int Version
        {
            get
            {
                var hostEndian = BinaryPrimitives.ReverseEndianness(_first16bit);

                return (ushort)(hostEndian >> 14);
            }
            set
            {
                var hostEndian = BinaryPrimitives.ReverseEndianness(_first16bit);

                hostEndian &= VersionMaskReset;
                hostEndian |= (ushort)(value << 14);

                _first16bit = BinaryPrimitives.ReverseEndianness(hostEndian);
            }
        }

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

        public int CsrcCount
        {
            get
            {
                var hostEndian = BinaryPrimitives.ReverseEndianness(_first16bit);
                hostEndian &= CsrcMaskGet;

                return hostEndian >> 8;
            }
            set
            {
                var hostEndian = BinaryPrimitives.ReverseEndianness(_first16bit);

                hostEndian &= CsrcMaskReset;
                hostEndian |= (ushort)(value << 8);

                _first16bit = BinaryPrimitives.ReverseEndianness(hostEndian);
            }
        }

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

        public RtpPayloadType PayloadType
        {
            get
            {
                var hostEndian = BinaryPrimitives.ReverseEndianness(_first16bit);

                return (RtpPayloadType)(hostEndian & PayloadTypeMaskGet);
            }
            set
            {
                Console.WriteLine(BinaryWriter.ConvertNumberToBinaryString(_first16bit));

                var hostEndian = BinaryPrimitives.ReverseEndianness(_first16bit);

                hostEndian &= PayloadTypeMaskReset;
                hostEndian |= (ushort)value;

                _first16bit = BinaryPrimitives.ReverseEndianness(hostEndian);

                Console.WriteLine(BinaryWriter.ConvertNumberToBinaryString(_first16bit));
            }
        }

        public ushort SequenceNumber
        {
            get
            {
                return BinaryPrimitives.ReverseEndianness(_sequenceNumber);
            }
            set
            {
                _sequenceNumber = BinaryPrimitives.ReverseEndianness(value);
            }
        }

        public uint Timestamp
        {
            get
            {
                return BinaryPrimitives.ReverseEndianness(_timestamp);
            }
            set
            {
                _timestamp = BinaryPrimitives.ReverseEndianness(value);
            }
        }

        public uint Ssrc
        {
            get
            {
                return BinaryPrimitives.ReverseEndianness(_ssrc);
            }
            set
            {
                _ssrc = BinaryPrimitives.ReverseEndianness(value);
            }
        }

        public RtpHeader()
        {

        }

        public void GetBytes(in Memory<byte> buffer)
        {
            if (buffer.Length < 12)
            {
                return;
            }

            var memorySpan = buffer.Span;

            BinaryPrimitives.WriteUInt16LittleEndian(memorySpan.Slice(0, 2), _first16bit);
            BinaryPrimitives.WriteUInt16LittleEndian(memorySpan.Slice(2, 2), _sequenceNumber);
            BinaryPrimitives.WriteUInt32LittleEndian(memorySpan.Slice(4, 4), _timestamp);
            BinaryPrimitives.WriteUInt32LittleEndian(memorySpan.Slice(8, 4), _ssrc);
        }
    }
}
