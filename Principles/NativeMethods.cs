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
