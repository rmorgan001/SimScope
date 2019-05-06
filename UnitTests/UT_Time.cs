using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Principles;

namespace UnitTests
{
    [TestClass]
    public class UT_Time: IDisposable
    {
        [TestMethod]
        public void Dhs2HMS()
        {
            const double dh = 18.524166667;
            TimeSpan hms;
            var expected = new TimeSpan(0, 18, 31, 27, 0);
            try
            {
                hms = Time.Dhs2HMS(dh);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, hms, $"Dhs2HMS Not Correct {expected}:{hms}");
        }

        [TestMethod]
        public void Ts2Dhrs()
        {
            var ts = new TimeSpan(0, 18, 31, 27, 0);
            double h;
            const double expected = 18.524166666666666;
            try
            {
                h = Time.Ts2Dhrs(ts);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, h, $"Ts2Dhrs Not Correct {expected}:{h}");
        }

        [TestMethod]
        public void Sx2Dhrs()
        {
            const string hms = "18:31:27";
            double d;
            const double expected = 18.524166666666666;
            try
            {
                d = Time.Sx2Dhrs(hms);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Sx2Dhrs Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void HMS2Dhrs()
        {
            const int hours = 18;
            const int minutes = 31;
            const int seconds = 27;
            const int milliseconds = 0;
            double d;
            const double expected = 18.524166666666666;
            try
            {
                d = Time.HMS2Dhrs(hours, minutes, seconds, milliseconds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"HMS2Dhrs Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Dhrs2Hrs()
        {
            const double dh = 18.524166667;
            int h;
            const int expected = 18;
            try
            {
                h = Time.Dhrs2Hrs(dh);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, h, $"Dhrs2Hrs Not Correct {expected}:{h}");
        }

        [TestMethod]
        public void Dhrs2Mins()
        {
            const double dh = 18.524166667;
            int m;
            const int expected = 31;
            try
            {
                m = Time.Dhrs2Mins(dh);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, m, $"Dhrs2Mins Not Correct {expected}:{m}");
        }

        [TestMethod]
        public void Dhrs2Secs()
        {
            const double dh = 18.52420139;
            double s;
            const double expected = 27.125004;
            try
            {
                s = Time.Dhrs2Secs(dh);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, s, $"Dhrs2Secs Not Correct {expected}:{s}");
        }

        [TestMethod]
        public void Gmt2Lst()
        {
            var ts = new TimeSpan(0, 4, 40, 5, 230);
            const double longitude = 81;
            double lst;
            const double expected = 10.068119444444445;
            try
            {
                lst = Time.Gmt2Lst(ts, longitude);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, lst, $"Gmt2Lst Not Correct {expected}:{lst}");
        }

        [TestMethod]
        public void Lst()
        {
            var epoch = JDate.Utc2Jd2(new DateTime(2000,1,1,12,0,0));     
            var jd = JDate.Utc2Jd2(new DateTime(2009, 6, 19, 4, 40, 5, 230));
            const double longitude = 81.0;
            double lstn; double lst;
            const double expectedn = 3.9042962940932857;     // {03:54:15.4700000}
            const double expected = 3.9042829930006215;     // {03:54:15.4200000}
            try
            {
                lstn = Time.Lst(epoch, jd,true, longitude);
                lst = Time.Lst(epoch, jd, false, longitude);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expectedn, lstn, $"Lst Not Correct {expectedn}:{lstn}");
            Assert.AreEqual(expected, lst, $"Lst Not Correct {expected}:{lst}");
        }

        [TestMethod]
        public void Lst1()
        {
            var epoch = JDate.Utc2Jd2(new DateTime(2000, 1, 1, 12, 0, 0)); 
            var jd = JDate.Utc2Jd2(new DateTime(2009, 6, 19, 4, 40, 5, 230)); 
            const double longitude = 81.0;
            double lst;
            const double expected = 3.9042832246341277; 
            try
            {
                lst = Time.Lst1(epoch, jd, longitude);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, lst, $"Lst1 Not Correct {expected}:{lst}");
        }

        [TestMethod]
        public void Lst2()
        {
            var epoch = JDate.Utc2Jd2(new DateTime(2000, 1, 1, 12, 0, 0));
            var utc = new DateTime(2009, 6, 19, 4, 40, 5, 230);
            const double longitude = 81.0;
            double lst;
            const double expected = 3.9042831390200021;
            try
            {
                lst = Time.Lst2(epoch, utc, longitude);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, lst, $"LST2 Not Correct {expected}:{lst}");
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
