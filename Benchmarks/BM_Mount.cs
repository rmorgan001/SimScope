using System;
using BenchmarkDotNet.Attributes;
using Principles;

namespace Benchmarks
{
    public class BM_Mount: IDisposable
    {

        [Benchmark]
        public void Ms2Arcsec()
        {
            Conversions.Ms2Arcsec(15, 15.041, 0.5);
        }

        [Benchmark]
        public void Rate2Steps()
        {
            Conversions.Rate2Steps(15, 15.041, 0.5, 111136000);
        }

        [Benchmark]
        public void StepPerArcsec()
        {
            Conversions.StepPerArcsec(111136000);
        }

        [Benchmark]
        public void GuideRate()
        {
            Conversions.GuideRate(15.041, 0.5);
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
