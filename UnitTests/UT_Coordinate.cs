using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Principles;

namespace UnitTests
{
    [TestClass]
    public class UT_Coordinate
    {

        [TestMethod]
        public void Ra2Ha()
        {
            const double ra = 14.310325754355503;
            const double lst = 18.90577273941123;
            double d;
            const double expected = 4.5954469850557267;
            try
            {
                d = Coordinate.Ra2Ha(ra, lst);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Ra2Ha Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void RaDec2AltAz()
        {
            const double ra = 12.90577273941123;
            const double dec = 90;
            const double lst = 18.90577273941123;
            const double lat = 28.356111526489258;
            double[] d;
            var expected = new[] { 28.356111526489258,0 };

            try
            {
                d = Coordinate.RaDec2AltAz(ra, dec, lst, lat);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            CollectionAssert.AreEqual(expected, d, $"RaDec2AzmAlt Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void HaDec2AltAz()
        {
            const double ha = 21.904296294093285;
            const double dec = 90;
            const double lat = 28.356111526489258;
            double[] d;
            var expected = new[]{ 28.356111526489261, 2.0791565994500092E-15 };
            try
            {
                d = Coordinate.HaDec2AltAz(ha, dec, lat);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            CollectionAssert.AreEqual(expected, d, $"HaDec2AzmAlt Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void HaDec2Azm()
        {
            const double ha = 6;
            const double dec = 90;
            const double lat = 28.356111526489258;
            double d;
            const double expected = 0;
            try
            {
                d = Coordinate.HaDec2Azm(ha, dec, lat);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"HaDec2Azm Not Correct {expected}:{d}");
        }


        [TestMethod]
        public void HaDec2Alt()
        {
            const double ha = 6;
            const double dec = 90;
            const double lat = 28.356111526489258;
            double d;
            const double expected = 28.356111526489258;
            try
            {
                d = Coordinate.HaDec2Alt(ha, dec, lat);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"HaDec2Alt Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void AltAz2RaDec()
        {
            const double azm = 0;
            const double alt = 28.356111526489258;
            const double lat = 28.356111526489258;
            const double lst = 1.4184750948815061;
            double[] d;
            var expected = new[] { 1.4184750948815061, 90 };
            try
            {
                d = Coordinate.AltAz2RaDec(alt, azm, lat, lst);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            CollectionAssert.AreEqual(expected, d, $"AzmAlt2RaDec Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void AltAz2Ra()
        {
            const double azm = 0;
            const double alt = 28.356111526489258;
            const double lat = 28.356111526489258;
            const double lst = 1.4184750948815061;
            double d;
            const double expected = 1.4184750948815061;
            try
            {
                d = Coordinate.AltAz2Ra(alt, azm, lat, lst);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"AzmAlt2Ra Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void AltAz2Dec()
        {
            const double azm = 0;
            const double alt = 28.356111526489258;
            const double lat = 28.356111526489258;
            double d;
            const double expected = 90;
            try
            {
                d = Coordinate.AltAz2Dec(alt, azm, lat);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"AzmAlt2Dec Not Correct {expected}:{d}");
        }
    }
}
