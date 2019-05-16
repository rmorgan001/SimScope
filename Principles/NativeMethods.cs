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

using System.Runtime.InteropServices;

namespace Principles
{
    public static class NativeMethods
    {
        #region Multimedia Timer

        // Gets timer capabilities.
        [DllImport("winmm.dll", EntryPoint = "timeGetDevCaps")]
        internal static extern int TimeGetDevCaps(ref TimerCaps caps, int sizeOfTimerCaps);

        // Creates and starts the timer.
        [DllImport("winmm.dll", EntryPoint = "timeSetEvent")]
        internal static extern int TimeSetEvent(int delay, int resolution, MediaTimer.TimeProc proc,ref int user, int mode);

        // Stops and destroys the timer.
        [DllImport("winmm.dll", EntryPoint = "timeKillEvent")]
        internal static extern int TimeKillEvent(int id);

        #endregion

        #region Time

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        internal static extern void GetSystemTimePreciseAsFileTime(out long filetime);

        [DllImport("Kernel32.dll")]
        internal static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        internal static extern bool QueryPerformanceFrequency(out long lpFrequency);

        #endregion
    }
}
