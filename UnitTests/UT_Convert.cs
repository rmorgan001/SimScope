using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Principles;

namespace UnitTests
{
    [TestClass]
    public class UT_Convert
    {
        [TestMethod]
        public void Deg2Dou()
        {
            const double degrees = 90;
            const double minutes = 10;
            const double seconds = 10;
            double d;
            const double expected = 90.169444444444451;
            try
            {
                d = Units.Deg2Dou(degrees, minutes, seconds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Deg2Dou Not Correct {expected}:{d}");
        }
        
        [TestMethod]
        public void Ra2Dou()
        {
            const double hour = 90;
            const double minutes = 10;
            const double seconds = 10;
            double d;
            const double expected = 90.169444444444451;
            try
            {
                d = Units.Ra2Dou(hour, minutes, seconds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Ra2Dou Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Dou2Deg()
        {
            const double dou = 90.169444444444451; 
            string s;
            const string expected = "90:10:10";
            try
            {
                s = Units.Dou2Deg(dou);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, s, $"Dou2Deg Not Correct {expected}:{s}");
        }

        [TestMethod]
        public void Rad2Deg()
        {
            const double radians = 90.169444444444451;
            double d;
            const double expected = 5166.3286077060147;
            try
            {
                d = Units.Rad2Deg(radians);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Rad2Deg Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Rad2Deg1()
        {
            const double radians = 90.169444444444451;
            double d;
            const double expected = 5166.3286077060147;
            try
            {
                d = Units.Rad2Deg1(radians);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Rad2Deg1 Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Rad2Deg2()
        {
            const double radians = 90.169444444444451;
            double d;
            const double expected = 5166.3286077060147;
            try
            {
                d = Units.Rad2Deg2(radians);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Rad2Deg2 Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Deg2Rad()
        {
            const double degrees = 90.169444444444451;
            double d;
            const double expected = 1.5737536902496649;
            try
            {
                d = Units.Deg2Rad(degrees);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Deg2Rad Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Deg2Rad1()
        {
            const double degrees = 90.169444444444451;
            double d;
            const double expected = 1.5737536902496649;
            try
            {
                d = Units.Deg2Rad1(degrees);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Deg2Rad1 Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Deg2Rad2()
        {
            const double degrees = 90.169444444444451;
            double d;
            const double expected = 1.5737536902496649;
            try
            {
                d = Units.Deg2Rad2(degrees);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Deg2Rad2 Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Date2Deg()
        {
            DateTime dateTime = new DateTime(2000,1,1);
            double d;
            const double expected = 0;
            try
            {
                d = Units.Date2Deg(dateTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Date2Deg Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Hrs2Deg()
        {
            const double hours = 23;
            double d;
            const double expected = 345;
            try
            {
                d = Units.Hrs2Deg(hours);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Hrs2Deg Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Deg2Hrs()
        {
            const double hours = 345;
            double d;
            const double expected = 23;
            try
            {
                d = Units.Deg2Hrs(hours);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Deg2Hrs Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Hrs2Rad()
        {
            const double hours = 23;
            double d;
            const double expected = 6.0213859193804362;
            try
            {
                d = Units.Hrs2Rad(hours);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Hrs2Rad Not Correct {expected}:{d}");
        }

        [TestMethod]
        public void Rad2Hrs()
        {
            const double hours = 6.021385919380436;
            double d;
            const double expected = 23;
            try
            {
                d = Units.Rad2Hrs(hours);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.AreEqual(expected, d, $"Rad2Hrs Not Correct {expected}:{d}");
        }
    }
}
