using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Principles;

namespace UnitTests
{
    [TestClass]
    public class UT_Timer
    {

        [TestMethod]
        public void MediaTimer()
        {
            bool expected;
            try
            {
                var mediatimer = new MediaTimer { Period = 1000 };
                mediatimer.Tick += TestTimerEvent;
                mediatimer.Start();
                expected = mediatimer.IsRunning;
                mediatimer.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.IsTrue(expected,$"MediaTimer fail: {expected}");
        }

        [TestMethod]
        public void StopwatchTimer()
        {
            bool expected;
            try
            {
                var stopwatchtimer = new StopwatchTimer( 1000);
                stopwatchtimer.Elapsed += TestTimerEvent;
                stopwatchtimer.Start();
                expected = stopwatchtimer.IsRunning;
                stopwatchtimer.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.IsTrue(expected, $"MediaTimer fail: {expected}");
        }

        /// <summary>
        /// Event for timer tests
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TestTimerEvent(object sender, EventArgs e)
        {
            // do somthing....
        }
    }
}
