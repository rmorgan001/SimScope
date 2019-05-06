using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Principles;

namespace UnitTests
{
    [TestClass]
    public class UT_Mount
    {

        [TestMethod]
        public void Ms2Arcsec()
        {
            const int milliseconds = 15;
            const double rate = 15.041;
            const double prate = .5;
            double a;
            const double expected = 0.1128075;
            try
            {
                a = Conversions.Ms2Arcsec(milliseconds, rate, prate);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, a, $"Ms2Arc Not Correct {expected}:{a}");
        }

        [TestMethod]
        public void Rate2Steps()
        {
            const int milliseconds = 15;
            const double rate = 15.041;
            const double prate = .5;
            const int totalsteps = 11136000;
            double a;
            const double expected = 0.96930888888888878;
            try
            {
                a = Conversions.Rate2Steps(milliseconds, rate, prate, totalsteps);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, a, $"Rate2Steps Not Correct {expected}:{a}");
        }

        [TestMethod]
        public void StepPerArcsec()
        {
            const int totalsteps = 11136000;
            double a;
            const double expected = 8.5925925925925917;
            try
            {
                a = Conversions.StepPerArcsec(totalsteps);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, a, $"StepPerArcsec Not Correct {expected}:{a}");
        }

        [TestMethod]
        public void GuideRate()
        {
            const double rate = 15.041;
            const double prate = .5;
            double a;
            const double expected = 0.0020890277777777778;
            try
            {
                a = Conversions.GuideRate(rate, prate);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, a, $"GuideRate Not Correct {expected}:{a}");
        }
    }
}
