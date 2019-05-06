using System;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using Principles;

namespace Benchmarks
{
    public class BM_Timestamp: IDisposable
    {

        public BM_Timestamp()
        {
            NowDatetime();
            UTCDatetime();
            HiResDateTimes();
            StopWatch();
        }

        [Benchmark]
        public void NowDatetime()
        {
            var _ = DateTime.Now;
        }

        [Benchmark]
        public void UTCDatetime()
        {
            var _ = DateTime.UtcNow;
        }

        [Benchmark]
        public void HiResDateTimes()
        {
            var _ = HiResDateTime.UtcNow;
        }

        [Benchmark]
        public void StopWatch()
        {
            var _ = new DateTime(Stopwatch.GetTimestamp());
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
