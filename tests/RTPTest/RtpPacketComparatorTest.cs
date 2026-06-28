using RTP;

namespace RTPTest
{
    [TestClass]
    public class RtpPacketComparatorTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DynamicData(nameof(RtpPacket_LessThen_Generator))]
        public void RptPacket_LessThen_Success(RtpPacket x, RtpPacket y)
        {
            var comparator = new RtpPacketComparator();

            var difference = comparator.Compare(x, y);

            TestContext.WriteLine($"Comparison of {x.Header.SequenceNumber} " +
                $"and {y.Header.SequenceNumber}. " +
                $"Result: {difference}");

            Assert.IsLessThan(0, difference);
        }

        [TestMethod]
        [DynamicData(nameof(RtpPacket_GreaterThen_Generator))]
        public void RtpPacket_GreaterThen_Success(RtpPacket x, RtpPacket y)
        {
            var comparator = new RtpPacketComparator();

            var difference = comparator.Compare(x, y);

            TestContext.WriteLine($"Comparison of {x.Header.SequenceNumber} " +
                $"and {y.Header.SequenceNumber}. " +
                $"Result: {difference}");

            Assert.IsGreaterThan(0, difference);
        }

        private static IEnumerable<(RtpPacket x, RtpPacket y)> RtpPacket_LessThen_Generator()
        {
            yield return (new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = 1,
                }
            }, new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = 2
                }
            });

            yield return (new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = 2
                }
            }, new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = 3
                }

            });

            yield return (new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = ushort.MaxValue
                }
            }, new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = ushort.MinValue
                }
            });

            yield return (new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = ushort.MaxValue / 2
                }
            }, new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = (ushort.MaxValue / 2) + 1
                }
            });

            yield return (new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = 0
                }
            }, new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = 1
                }
            });

            yield return (new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = ushort.MaxValue - 1
                }
            }, new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = ushort.MaxValue
                }
            });

            yield return (new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = 54320
                }
            }, new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = 21211
                }
            });
        }

        private static IEnumerable<(RtpPacket x, RtpPacket y)> RtpPacket_GreaterThen_Generator()
        {
            yield return (new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = 2
                }
            }, new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = 1
                }
            });

            yield return (new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = ushort.MinValue
                }
            }, new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = ushort.MaxValue
                }

            });

            yield return (new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = ushort.MaxValue / 2
                }
            }, new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = ushort.MinValue
                }
            });

            yield return (new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = ushort.MaxValue
                }
            }, new RtpPacket()
            {
                Header = new RtpHeader()
                {
                    SequenceNumber = ushort.MaxValue - 1
                }
            });

            
        }
    }
}
