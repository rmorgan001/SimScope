using System;
using BenchmarkDotNet.Attributes;
using Principles;

namespace Benchmarks
{
    public class BM_Date: IDisposable
    {
        private readonly DateTime dt;

        public BM_Date()
        {
            dt = new DateTime(2000, 1, 1);
            Utc2Jd();
            Ole2Jd();
            Utc2Jd1();
            Utc2Jd2();
            Jd2Utc();
            Jd2Cday();
            Jd2Cmonth();
            Jd2Cyear();
        }

        [Benchmark]
        public void Utc2Jd()
        {
            JDate.Utc2Jd(19, 6, 2009);
        }

        [Benchmark]
        public void Ole2Jd()
        {
            JDate.Ole2Jd(dt);
        }

        [Benchmark]
        public void Utc2Jd1()
        {
            JDate.Utc2Jd1(2009, 6, 19, 18, 0, 0, 0);
        }

        [Benchmark]
        public void Utc2Jd2()
        {
            JDate.Utc2Jd2(new DateTime(2009, 6, 19, 18, 0, 0));
        }

        [Benchmark]
        public void Jd2Utc()
        {
            JDate.Jd2Utc(2455002.25);
        }

        [Benchmark]
        public void Jd2Cday()
        {
            JDate.Jd2Cday(2456474.4423611108);
        }

        [Benchmark]
        public void Jd2Cmonth()
        {
            JDate.Jd2Cmonth(2456474.4423611108);
        }

        [Benchmark]
        public void Jd2Cyear()
        {
            JDate.Jd2Cyear(2456474.4423611108);
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
