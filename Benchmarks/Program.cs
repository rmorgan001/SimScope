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
using BenchmarkDotNet.Running;
using Principles;

namespace Benchmarks
{
    public static class Program
    {
        private static DateTime timerTestDate;
        private static int timerTestCount;
        private static MediaTimer mediatimer;
        private static StopwatchTimer stopwatchtimer;
        private static System.Timers.Timer systemtimer;

        private static void Main(string[] args) 
        {
           // It's recommended that benchmark tests run from a release build and not debug. Uncomment to run in debug
           // BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());

            ConsoleKeyInfo console;
                do
                {
                    DisplayMenu();
                    console = Console.ReadKey(false); 
                    switch (console.KeyChar.ToString())
                    {
                        case "1":
                            #if DEBUG
                                Console.WriteLine(" Please run in release mode");
                            #else
                                Console.WriteLine(" Starting...");
                                BenchmarkRunner.Run<BM_Date>();
                            #endif
                        break;
                        case "2":
                            #if DEBUG
                                Console.WriteLine(" Please run in release mode");
                            #else
                                Console.WriteLine(" Starting...");
                                BenchmarkRunner.Run<BM_Time>();
                            #endif
                        break;
                        case "3":
                            #if DEBUG
                                Console.WriteLine(" Please run in release mode");
                            #else
                                Console.WriteLine(" Starting...");
                                BenchmarkRunner.Run<BM_Timestamp>();
                            #endif
                        break;
                        case "4":
                            #if DEBUG
                            Console.WriteLine(" Running in release mode is recommended, Starting...");
                            #else
                            Console.WriteLine(" Starting...");
                            #endif
                            NowResolutionTest();
                            UtcResolutionTest();
                            HighResDateTimeTest();
                            HighResStopwatchTest();
                            break;
                        case "5":
                            Console.WriteLine(" Starting 50 iterations... (Esc = Stop)");
                            MediaTimerTest(500);
                            break;
                        case "6":
                            Console.WriteLine(" Starting 50 iterations... (Esc = Stop)");
                            StopwatchTimerTest(500);
                            break;
                        case "7":
                            Console.WriteLine(" Starting 50 iterations... (Esc = Stop)");
                            SystemTimerTest(500);
                            break;
                        case "8":
#if DEBUG
                            Console.WriteLine(" Please run in release mode");
#else
                                Console.WriteLine(" Starting...");
                                BenchmarkRunner.Run<BM_Mount>();
#endif
                            break;
                        case "9":
#if DEBUG
                            Console.WriteLine(" Please run in release mode");
#else
                            Console.WriteLine(" Starting...");
                            BenchmarkRunner.Run<BM_Coordinate>();
#endif
                            break;
                        // etc..
                }
                } while (console.Key != ConsoleKey.Escape);
        }

        /// <summary>
        /// Menu
        /// </summary>
        private static void DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("     Basics Benchmark Tests - Select a number to start test");
            Console.WriteLine();
            Console.WriteLine("     1. Dates");
            Console.WriteLine("     2. Times");
            Console.WriteLine("     3. Timestamps");
            Console.WriteLine("     4. Timestamps Accuracy");
            Console.WriteLine("     5. MediaTimer");
            Console.WriteLine("     6. StopwatchTimer");
            Console.WriteLine("     7. System Timer");
            Console.WriteLine("     8. Mount");
            Console.WriteLine("     9. Coordinates");
            Console.WriteLine("     Esc = Exit");
            Console.WriteLine();
        }

        /// <summary>
       /// Accuracy test using DateTime.Now
       /// </summary>
        private static void NowResolutionTest()
       {
           Console.WriteLine();
           Console.WriteLine("     5 seconds of creating dates using DateTime.Now....");
           var start = DateTime.Now;
           var distinct = 0;
           var total = 0;
           var dups = 0;
            var sw = Stopwatch.StartNew();
           while (sw.Elapsed.TotalSeconds < 5)
           {
               var now = DateTime.Now;
               if (now != start)
               {
                   start = now;
                   distinct++;
               }
               else
               {
                   dups++;
               }
               total++;
           }
           sw.Stop();
           Console.WriteLine("     Precision: {0:0.000000}ms Total={1} Distinct={2} Duplicates={3} Distincts={4:0.000%}",
           sw.Elapsed.TotalMilliseconds / distinct, total, distinct, dups, (double)distinct / total);
        }

        /// <summary>
        /// Accuracy test using DateTime.UtcNow
        /// </summary>
        private static void UtcResolutionTest()
       {
           Console.WriteLine();
           Console.WriteLine("     5 seconds of creating dates using DateTime.UtcNow....");
           var start = DateTime.UtcNow;
           var distinct = 0;
           var total = 0;
           var dups = 0;
            var sw = Stopwatch.StartNew();
           while (sw.Elapsed.TotalSeconds < 5)
           {
               var now = DateTime.UtcNow;
               if (now != start)
               {
                   start = now;
                   distinct++;
               }
               else {dups++;}
               total++;
           }
           sw.Stop();
           Console.WriteLine("     Precision: {0:0.000000}ms Total={1} Distinct={2} Duplicates={3} Distincts={4:0.000%}",
               sw.Elapsed.TotalMilliseconds / distinct, total, distinct, dups, (double)distinct / total);
        }

        /// <summary>
       /// Accuracy test using HiResTime
       /// </summary>
        private static void HighResDateTimeTest()
       {
           Console.WriteLine();
           Console.WriteLine("     5 seconds of creating dates using HiResDateTime ....");
           var start = HiResDateTime.UtcNow;
           var distinct = 0;
           var total = 0;
           var dups = 0;
            var sw = Stopwatch.StartNew();
           while (sw.Elapsed.TotalSeconds < 5)
           {
               var now = HiResDateTime.UtcNow;
               if (now != start)
               {
                   start = now;
                   distinct++;
               }
               else {dups++;}
               total++;
           }
           sw.Stop();
           Console.WriteLine("     Precision: {0:0.000000}ms Total={1} Distinct={2} Duplicates={3} Distincts={4:0.000%}",
               sw.Elapsed.TotalMilliseconds / distinct, total, distinct, dups, (double)distinct / total);
        }

        /// <summary>
        /// Accuracy test using HiResTime
        /// </summary>
        private static void HighResStopwatchTest()
        {
            Console.WriteLine();
            Console.WriteLine("     5 sec read using StopWatch ....");
            var tickCountnow = Stopwatch.GetTimestamp();
            var start = new DateTime(tickCountnow);
            var distinct = 0;
            var total = 0;
            var dups = 0;
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalSeconds < 5)
            {
                var tickCount = Stopwatch.GetTimestamp();
                var now = new DateTime(tickCount);
                if (now != start)
                {
                    start = now;
                    distinct++;
                }
                else { dups++; }
                total++;
            }
            sw.Stop();
            Console.WriteLine("     Precision: {0:0.000000}ms Total={1} Distinct={2} Duplicates={3} Distincts={4:0.000%}",
                sw.Elapsed.TotalMilliseconds / distinct, total, distinct, dups, (double)distinct / total);
        }

        /// <summary>
        /// Event timing test using MediaTimer
        /// </summary>
        private static void MediaTimerTest(int interval)
        {
            timerTestCount = 0;
            timerTestDate = DateTime.MinValue;
            mediatimer = new MediaTimer { Period = interval };
            mediatimer.Tick += TestTimerEvent;
            mediatimer.Start();
            timerTestDate = HiResDateTime.UtcNow;
            ConsoleKeyInfo console;
            do
            {
                console = Console.ReadKey(false);
            } while (console.Key != ConsoleKey.Escape);
            mediatimer.Stop();
            mediatimer.Dispose();
            timerTestDate = DateTime.MinValue;
        }

        /// <summary>
        /// Event timing test using Stopwatch
        /// </summary>
        private static void StopwatchTimerTest(int interval)
        {
            timerTestCount = 0;
            timerTestDate = DateTime.MinValue;
            stopwatchtimer = new StopwatchTimer(interval);
            stopwatchtimer.Elapsed += TestTimerEvent;
            stopwatchtimer.Start();
            timerTestDate = HiResDateTime.UtcNow;
            ConsoleKeyInfo console;
            do
            {
                console = Console.ReadKey(false);
            } while (console.Key != ConsoleKey.Escape);
            stopwatchtimer.Stop();
        }

        /// <summary>
        /// Event timing test using System Timer
        /// </summary>
        private static void SystemTimerTest(int interval)
        {
            timerTestCount = 0;
            timerTestDate = DateTime.MinValue;
            systemtimer = new System.Timers.Timer();
            systemtimer.Elapsed += TestTimerEvent;
            systemtimer.Interval = interval;
            systemtimer.Start();
            timerTestDate = HiResDateTime.UtcNow;
            ConsoleKeyInfo console;
            do
            {
                console = Console.ReadKey(false);
            } while (console.Key != ConsoleKey.Escape);
            systemtimer.Stop();
        }

        /// <summary>
        /// Event for timer tests
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TestTimerEvent(object sender, EventArgs e)
        {
            timerTestCount++;
            if (timerTestCount > 50)
            {
                var a = HiResDateTime.UtcNow;
                var b = a - timerTestDate;
                var c = b.TotalMilliseconds;
                var d = c / timerTestCount;
                Console.WriteLine($"        Average Event Timespan= {d} ms");
                Console.WriteLine("        Hit Esc key for menu");
                systemtimer?.Stop();
                mediatimer?.Stop();
                stopwatchtimer?.Stop();
                return;
            }
            Console.WriteLine($"        Interation= {timerTestCount}");
        }
    }
}
