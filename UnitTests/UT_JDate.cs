using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Principles;

namespace UnitTests
{
    [TestClass]
    public class UT_JDate: IDisposable
    {

        
        [TestMethod]
        public void Utc2Jd()
        {
            const double day = 19.75;
            const int month = 6;
            const int year = 2009;
            double jd;
            const double expected = 2455002.25;
            try
            {
                jd = JDate.Utc2Jd(day, month, year);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, jd, $"Utc2Jd Not Correct {expected}:{jd}");
        }

        [TestMethod]
        public void Ole2Jd()
        {
            var dt = new DateTime(2009, 6, 19, 18, 0, 0);
            double jd;
            const double expected = 2455002.25;
            try
            {
                jd = JDate.Ole2Jd(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, jd, $"Ole2Jd Not Correct {expected}:{jd}");
        }

        [TestMethod]
        public void Utc2Jd1()
        {
            const int year = 2009;
            const int month = 6;
            const int day = 19;
            const int hour = 18;
            const int minute = 0;
            const int second = 0;
            const int millisecond = 0;
            double jd;
            const double expected = 2455002.25;
            try
            {
                jd = JDate.Utc2Jd1(year, month, day, hour, minute, second, millisecond);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, jd, $"Utc2Jd1 Not Correct {expected}:{jd}");
        }

        [TestMethod]
        public void Utc2Jd2()
        {
            var dt = new DateTime(2009, 6, 19, 18, 0, 0);
            double jd;
            const double expected = 2455002.25;
            try
            {
                jd = JDate.Utc2Jd2(dt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, jd, $"Utc2Jd2 Not Correct {expected}:{jd}");
        }

        [TestMethod]
        public void IsJd()
        {
            const int day = 19;
            const int month = 6;
            const int year = 2009;
            bool b;
            const bool expected = true;
            try
            {
                b = JDate.IsJd(day, month, year);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, b, $"IsJd Not Correct {expected}:{b}");
        }

        [TestMethod]
        public void Jd2Utc()
        {
            const double juliandate = 2455002.25;
            double cd;
            const double expected = 2456306;
            try
            {
                cd = JDate.Jd2Utc(juliandate);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, cd, $"Jd2Utc Not Correct {expected}:{cd}");
        }

        [TestMethod]
        public void Jd2Cday()
        {
            double day;
            const double expected = 30.942361110821366;
            try
            {
                day = JDate.Jd2Cday(2456474.4423611108);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, day, $"Jd2Cday Not Correct {expected}:{day}");
        }

        [TestMethod]
        public void Jd2Cmonth()
        {
            int month;
            const int expected = 6;
            try
            {
                month = JDate.Jd2Cmonth(2456474.4423611108);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, month, $"Jd2Cmonth Not Correct {expected}:{month}");
        }

        [TestMethod]
        public void Jd2Cyear()
        {
            int year;
            const int expected = 2013;
            try
            {
                year = JDate.Jd2Cyear(2456474.4423611108);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, year, $"Jd2Cyear Not Correct {expected}:{year}");
        }

        [TestMethod]
        public void EpochDays()
        {
            var dt = new DateTime(2010, 6, 19, 18, 0, 0);
            var dt1 = new DateTime(1999, 6, 19, 18, 0, 0);
            double d;
            const int expected = -4018;
            try
            {
                d = JDate.EpochDays(dt,dt1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"EpochDays Not Correct {expected}:{d}");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            // free native resources if there are any.
        }
    }
}
