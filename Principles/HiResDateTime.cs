using System;
using System.Diagnostics;
using System.Threading;

namespace Principles
{
    /// <summary>
    /// Windows 8 or Server 2012 and higher. All others return using System.Diagnostics.Stopwatch />.
    /// </summary>
    public static class HiResDateTime
    {
        private static readonly long maxidle = TimeSpan.FromSeconds(10).Ticks;
        private const long TicksMultiplier = 1000 * TimeSpan.TicksPerMillisecond;
        private static readonly ThreadLocal<DateTime> starttime = new ThreadLocal<DateTime>(() => DateTime.UtcNow, false);
        private static readonly ThreadLocal<double> starttimestamp = new ThreadLocal<double>(() => Stopwatch.GetTimestamp(), false);

        /// <summary>
        /// High resolution supported
        /// Returns True on Windows 8 and Server 2012 and higher.
        /// </summary>
        private static bool IsPrecise { get; }

        /// <summary>
        /// Gets the datetime in UTC.
        /// </summary>
        public static DateTime UtcNow
        {
            get
            {
                if (IsPrecise)
                {
                    NativeMethods.GetSystemTimePreciseAsFileTime(out var preciseTime);
                    return DateTime.FromFileTimeUtc(preciseTime);
                }
                double endTimestamp = Stopwatch.GetTimestamp();
                var durationInTicks = (endTimestamp - starttimestamp.Value) / Stopwatch.Frequency * TicksMultiplier;
                if (!(durationInTicks >= maxidle)) return starttime.Value.AddTicks((long) durationInTicks);
                starttimestamp.Value = Stopwatch.GetTimestamp();
                starttime.Value = DateTime.UtcNow;
                return starttime.Value;
            }
        }

        /// <summary>
        /// Creates an instance
        /// </summary>
        static HiResDateTime()
        {
            try
            {
                NativeMethods.GetSystemTimePreciseAsFileTime(out _);
                IsPrecise = true;
            }
            catch (EntryPointNotFoundException)
            {
                IsPrecise = false;
            }
        }
    }
}
