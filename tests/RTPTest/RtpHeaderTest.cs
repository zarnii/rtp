using RTP;

namespace RTPTest
{
    [TestClass]
    public sealed class RtpHeaderTest
    {
        [TestMethod]
        [DataRow(2, 2)]
        [DataRow(1, 1)]
        [DataRow(0, 0)]
        public void Version_Set_Success(int actualVersion, int expectedVersion)
        {
            var header = new RtpHeader();

            header.Version = actualVersion;

            Assert.AreEqual(expectedVersion, header.Version);
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(1, 2, 2)]
        [DataRow(0, 1, 1)]
        [DataRow(0, 0, 0)]
        [DataRow(2, 1, 1)]
        [DataRow(0, 2, 2)]
        public void Version_Change_Success(int initVersion, int changeToVersion, int expectedVersion)
        {
            var header = new RtpHeader()
            {
                Version = initVersion
            };

            header.Version = changeToVersion;

            Assert.AreEqual(expectedVersion, header.Version);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, true)]
        public void Padding_Set_Success(bool actialPadding, bool expectedPadding)
        {
            var header = new RtpHeader();

            header.Padding = actialPadding;

            Assert.AreEqual(expectedPadding, header.Padding);
        }

        [TestMethod]
        [DataRow(true, true, true)]
        [DataRow(true, false, false)]
        [DataRow(false, false, false)]
        [DataRow(false, true, true)]
        public void Padding_Change_Success(bool initPadding, bool changeToPadding, bool expectedPadding)
        {
            var header = new RtpHeader()
            {
                Padding = initPadding
            };

            header.Padding = changeToPadding;

            Assert.AreEqual(expectedPadding, header.Padding);
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(false, false)]
        public void Extension_Set_Success(bool actualExtension, bool expectedExtension)
        {
            var header = new RtpHeader();

            header.Extension = actualExtension;

            Assert.AreEqual(expectedExtension, header.Extension);
        }


        [TestMethod]
        [DataRow(true, true, true)]
        [DataRow(true, false, false)]
        [DataRow(false, false, false)]
        [DataRow(false, true, true)]
        public void Extension_Change_Success(bool initExtension, bool changeToExtension, bool expectedExtension)
        {
            var header = new RtpHeader()
            {
                Extension = initExtension
            };

            header.Extension = changeToExtension;

            Assert.AreEqual(expectedExtension, header.Extension);
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(2, 2)]
        [DataRow(3, 3)]
        [DataRow(4, 4)]
        [DataRow(5, 5)]
        [DataRow(6, 6)]
        [DataRow(7, 7)]
        [DataRow(8, 8)]
        [DataRow(9, 9)]
        [DataRow(10, 10)]
        [DataRow(11, 11)]
        [DataRow(12, 12)]
        [DataRow(13, 13)]
        [DataRow(14, 14)]
        [DataRow(15, 15)]
        public void CsrcCount_Set_Success(int actualCsrcCount, int expectedCsrcCount)
        {
            var header = new RtpHeader();

            header.CsrcCount = actualCsrcCount;

            Assert.AreEqual(expectedCsrcCount, header.CsrcCount);
        }


        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(1, 1, 1)]
        [DataRow(2, 2, 2)]
        [DataRow(0, 15, 15)]
        [DataRow(1, 4, 4)]
        [DataRow(15, 0, 0)]
        [DataRow(6, 13, 13)]
        [DataRow(8, 3, 3)]
        [DataRow(9, 4, 4)]
        public void CsrcCount_Change_Success(int initCsrcCount, int changeToCsrcCount, int expextedCsrcCount)
        {
            var header = new RtpHeader()
            {
                CsrcCount = initCsrcCount,
            };

            header.CsrcCount = changeToCsrcCount;

            Assert.AreEqual(header.CsrcCount, expextedCsrcCount);
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(false, false)]
        public void Marker_Set_Success(bool actualMarker, bool expectedMarker)
        {
            var header = new RtpHeader();

            header.Marker = actualMarker;

            Assert.AreEqual(expectedMarker, header.Marker);
        }

        [TestMethod]
        [DataRow(true, true, true)]
        [DataRow(false, false, false)]
        [DataRow(true, false, false)]
        [DataRow(false, true, true)]
        public void Marker_Change_Success(bool initMarker, bool changeToMarker, bool expextedMarker)
        {
            var header = new RtpHeader()
            {
                Marker = initMarker
            };

            header.Marker = changeToMarker;

            Assert.AreEqual(expextedMarker, header.Marker);
        }

        [TestMethod]
        [DataRow(RtpPayloadType.Pcmu, RtpPayloadType.Pcmu)]
        [DataRow(RtpPayloadType.Gsm, RtpPayloadType.Gsm)]
        [DataRow(RtpPayloadType.G729, RtpPayloadType.G729)]
        [DataRow(RtpPayloadType.G722, RtpPayloadType.G722)]
        [DataRow(RtpPayloadType.Dynamic96, RtpPayloadType.Dynamic96)]
        [DataRow(RtpPayloadType.Dynamic97, RtpPayloadType.Dynamic97)]
        [DataRow(RtpPayloadType.Dynamic98, RtpPayloadType.Dynamic98)]
        [DataRow(RtpPayloadType.Dynamic99, RtpPayloadType.Dynamic99)]
        [DataRow(RtpPayloadType.Dynamic100, RtpPayloadType.Dynamic100)]
        [DataRow(RtpPayloadType.Dynamic101, RtpPayloadType.Dynamic101)]
        [DataRow(RtpPayloadType.Dynamic102, RtpPayloadType.Dynamic102)]
        [DataRow(RtpPayloadType.Dvi4_8hz, RtpPayloadType.Dvi4_8hz)]
        [DataRow(RtpPayloadType.Dvi4_11hz, RtpPayloadType.Dvi4_11hz)]
        [DataRow(RtpPayloadType.Dvi4_16hz, RtpPayloadType.Dvi4_16hz)]
        [DataRow(RtpPayloadType.Dvi4_22hz, RtpPayloadType.Dvi4_22hz)]
        public void PayloadType_Set_Success(RtpPayloadType actualPayloadType,  RtpPayloadType expectedPayloadType)
        {
            var header = new RtpHeader();
            header.PayloadType = actualPayloadType;

            Assert.AreEqual(expectedPayloadType, header.PayloadType);
        }

        [TestMethod]
        [DataRow(RtpPayloadType.Pcmu, RtpPayloadType.Pcmu, RtpPayloadType.Pcmu)]
        [DataRow(RtpPayloadType.Gsm, RtpPayloadType.Gsm, RtpPayloadType.Gsm)]
        [DataRow(RtpPayloadType.G729, RtpPayloadType.Dynamic100, RtpPayloadType.Dynamic100)]
        [DataRow(RtpPayloadType.G722, RtpPayloadType.Celb, RtpPayloadType.Celb)]
        [DataRow(RtpPayloadType.Dynamic96, RtpPayloadType.Dynamic96, RtpPayloadType.Dynamic96)]
        [DataRow(RtpPayloadType.Dynamic97, RtpPayloadType.L16, RtpPayloadType.L16)]
        [DataRow(RtpPayloadType.Dynamic98, RtpPayloadType.L16_2channels, RtpPayloadType.L16_2channels)]
        [DataRow(RtpPayloadType.Dynamic99, RtpPayloadType.Dynamic99, RtpPayloadType.Dynamic99)]
        [DataRow(RtpPayloadType.Dynamic100, RtpPayloadType.Pcma, RtpPayloadType.Pcma)]
        [DataRow(RtpPayloadType.Dynamic101, RtpPayloadType.Mp2t, RtpPayloadType.Mp2t)]
        [DataRow(RtpPayloadType.Dynamic102, RtpPayloadType.Cn, RtpPayloadType.Cn)]
        [DataRow(RtpPayloadType.Dvi4_8hz, RtpPayloadType.H261, RtpPayloadType.H261)]
        [DataRow(RtpPayloadType.Dvi4_11hz, RtpPayloadType.Dvi4_11hz, RtpPayloadType.Dvi4_11hz)]
        [DataRow(RtpPayloadType.Dvi4_16hz, RtpPayloadType.Jpeg, RtpPayloadType.Jpeg)]
        [DataRow(RtpPayloadType.Dvi4_22hz, RtpPayloadType.Mpv, RtpPayloadType.Mpv)]
        public void PayloadType_Change_Success(RtpPayloadType initPayloadType, RtpPayloadType changeToPayloadType, 
            RtpPayloadType expectedPayloadType)
        {
            var header = new RtpHeader()
            {
                PayloadType = initPayloadType,
            };

            header.PayloadType = changeToPayloadType;

            Assert.AreEqual(expectedPayloadType, header.PayloadType);
        }

        [TestMethod]
        public void RtpHeader_GetNetworkOrderBytes_Success(RtpHeader header, byte[] expectedBytes)
        {
            /*
             var header = new RtpHeader()
            {
                Version = 2,
                Padding = true,
                Extension = true,
                CsrcCount = 2,
                Marker = true,
                PayloadType = RtpPayloadType.Dynamic100,
                SequenceNumber = 54,
                Timestamp = uint.MaxValue,
                Ssrc = 1
            };
             */
            // 0xB2, 0xE4, 0x00, 0x36, 0x00, 0x00, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF
            // DynamicData

            var actualBytes = new byte[RtpHeader.FixedHeaderSize];
            header.GetNetworkOrderBytes(actualBytes);

            CollectionAssert.AreEqual(actualBytes, expectedBytes);
        }

        [TestMethod]
        public void RtpHeader_CreateFromNetworkOrderBytes_Success(Span<byte> bytes, RtpHeader expectedHeader)
        {
            var actualHeader = RtpHeader.CreateFromNetworkOrderBytes(bytes);

            Assert.IsTrue(expectedHeader.Equals(actualHeader));
        }
    }
}
