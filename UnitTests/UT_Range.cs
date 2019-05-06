using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Principles;

namespace UnitTests
{
    [TestClass]
    public class UT_Range
    {
        [TestMethod]
        public void Range12()
        {
            const double dou = 90.169444444444451;
            double d;
            const double expected = -5.8305555555555486;
            try
            {
                d = Range.Range12(dou);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Range12 Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Range24()
        {
            const double dou = 90.169444444444451;
            double d;
            const double expected = 18.169444444444451;
            try
            {
                d = Range.Range24(dou);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Range24 Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Range90()
        {
            const double dou = 90.169444444444451;
            double d;
            const double expected = 89.830555555555549;
            try
            {
                d = Range.Range90(dou);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Range90 Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Range270()
        {
            const double dou = 290.169444444444451;
            double d;
            const double expected = -69.830555555555577;
            try
            {
                d = Range.Range270(dou);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Range270 Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Range360()
        {
            const double dou = 590.169444444444451;
            double d;
            const double expected = 230.16944444444448;
            try
            {
                d = Range.Range360(dou);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Range360 Not Correct {expected}:{d}");
        }
    }
}
