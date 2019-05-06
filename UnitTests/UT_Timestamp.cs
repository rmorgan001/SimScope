using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UT_Timestamp: IDisposable
    {
        [TestMethod]
        public void HiResDateTime()
        {
            TimeSpan ts;
            var passed = false;
            var expected = new TimeSpan(0,0,0,0,1);
            try
            {
                var h = Principles.HiResDateTime.UtcNow;
                var h1 = Principles.HiResDateTime.UtcNow;
                ts = h1 - h;
                if (ts < expected) passed = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Assert.IsTrue(passed, $"HiResDateTime out of {expected} range :{ts}");
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
