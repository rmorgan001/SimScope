using System;
using BenchmarkDotNet.Attributes;
using Principles;

namespace Benchmarks
{
    public class BM_Time: IDisposable
    {
        public BM_Time()
        {
            Dhs2HMS();
            Ts2Dhrs();
            Sx2Dhrs();
            HMS2Dhrs();
            Gmt2Lst();
            Lst();
            Lsta();
            Lst1();
            Lst2();
        }

        [Benchmark]
        public void Dhs2HMS()
        {
            Time.Dhs2HMS(18.524166667);
        }

        [Benchmark]
        public void Ts2Dhrs()
        {
            var ts = new TimeSpan(0, 18, 31, 27, 0);
            Time.Ts2Dhrs(ts);
        }

        [Benchmark]
        public void Sx2Dhrs()
        {
            const string ts = "18:31:27";
            Time.Sx2Dhrs(ts);
        }

        [Benchmark]
        public void HMS2Dhrs()
        {
            const int hours = 18;
            const int minutes = 31;
            const int seconds = 27;
            const int milliseconds = 0;
            Time.HMS2Dhrs(hours, minutes, seconds, milliseconds);
        }

        [Benchmark]
        public void Gmt2Lst()
        {
            var ts = new TimeSpan(0, 4, 40, 5, 230);
            const double longitude = -64.0;
            Time.Gmt2Lst(ts, longitude);
        }


        [Benchmark]
        public void Lst()
        {
            var epoch = JDate.Utc2Jd2(new DateTime(2000, 1, 1, 12, 0, 0));     // 2451545.0; //J2000
            var jd = JDate.Utc2Jd2(new DateTime(2009, 6, 19, 4, 40, 5, 230));
            const double longitude = 81;
            Time.Lst(epoch, jd, true, longitude);
        }

        [Benchmark]
        public void Lsta()
        {
            var epoch = JDate.Utc2Jd2(new DateTime(2000, 1, 1, 12, 0, 0));     // 2451545.0; //J2000
            var jd = JDate.Utc2Jd2(new DateTime(2009, 6, 19, 4, 40, 5, 230));
            const double longitude = 81;
            Time.Lst(epoch, jd, false, longitude);
        }

        [Benchmark]
        public void Lst1()
        {
            var epoch = JDate.Utc2Jd2(new DateTime(2000, 1, 1, 12, 0, 0));     // 2451545.0; //J2000
            var jd = JDate.Utc2Jd2(new DateTime(2009, 6, 19, 4, 40, 5, 230));
            const double longitude = 81;
            Time.Lst1(epoch, jd, longitude);
        }

        [Benchmark]
        public void Lst2()
        {
            var epoch = JDate.Utc2Jd2(new DateTime(2000, 1, 1, 12, 0, 0));     // 2451545.0; //J2000
            var dt = new DateTime(2009, 6, 19, 4, 40, 5, 230);
            const double longitude = 81;
            Time.Lst2(epoch, dt, longitude);
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
