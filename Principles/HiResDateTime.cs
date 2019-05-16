/* SimScope - ASCOM Telescope Control Simulator Copyright (c) 2019 Robert Morgan
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

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
