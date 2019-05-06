using System;
using BenchmarkDotNet.Attributes;
using Principles;

namespace Benchmarks
{
    public class BM_Coordinate : IDisposable
    {
        public BM_Coordinate()
        {
            Ra2Ha();
            RaDec2AltAz();
            HaDec2AltAz();
            HaDec2Azm();
            HaDec2Alt();
            AltAz2Ra();
            AzmAlt2Dec();
        }

        [Benchmark]
        public void Ra2Ha()
        {
            var _ = Coordinate.Ra2Ha(14.310325754355503, 18.90577273941123);
        }

        [Benchmark]
        public void RaDec2AltAz()
        {
            Coordinate.RaDec2AltAz(12.90577273941123, 90, 18.90577273941123, 28.356111526489258);
        }

        [Benchmark]
        public void HaDec2AltAz()
        {
            Coordinate.HaDec2AltAz(21.904296294093285, 90, 28.356111526489258);
        }

        [Benchmark]
        public void HaDec2Azm()
        {
            Coordinate.HaDec2Azm(6, 90, 28.356111526489258);
        }

        [Benchmark]
        public void AltAz2Ra()
        {
            Coordinate.AltAz2Ra(28.356111526489258, 0, 28.356111526489258, 1.4184750948815061);
        }

        [Benchmark]
        public void HaDec2Alt()
        {
            Coordinate.HaDec2Alt(6, 90, 28.356111526489258);
        }

        [Benchmark]
        public void AzmAlt2Dec()
        {
            Coordinate.AltAz2Dec(28.356111526489258, 0, 28.356111526489258);
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
