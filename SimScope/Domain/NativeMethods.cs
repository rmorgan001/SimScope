using System;
using System.Runtime.InteropServices;

namespace SimServer.Domain
{
    internal static class NativeMethods
    {
        [DllImport("ole32.dll")]
        internal static extern int CoRegisterClassObject(
            [In] ref Guid rclsid,
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            uint dwClsContext,
            uint flags,
            out uint lpdwRegister);

        /// <summary>
        /// Called by a COM EXE Server that can register multiple class objects 
        /// to inform COM about all registered classes, and permits activation 
        /// requests for those class objects. 
        /// This function causes OLE to inform the SCM about all the registered 
        /// classes, and begins letting activation requests into the server process.
        /// </summary>
        /// <returns></returns>
        [DllImport("ole32.dll")]
        internal static extern int CoResumeClassObjects();

        /// <summary>
        /// Prevents any new activation requests from the SCM on all class objects
        /// registered within the process. Even though a process may call this API, 
        /// the process still must call CoRevokeClassObject for each CLSID it has 
        /// registered, in the apartment it registered in.
        /// </summary>
        /// <returns></returns>
        [DllImport("ole32.dll")]
        internal static extern int CoSuspendClassObjects();

        /// <summary>
        /// CoRevokeClassObject() is used to unregister a Class Factory
        /// from COM's internal table of Class Factories.
        /// </summary>
        /// <param name="dwRegister"></param>
        /// <returns></returns>
        [DllImport("ole32.dll")]
        internal static extern int CoRevokeClassObject(uint dwRegister);

        /// <summary>
        /// PostThreadMessage() allows us to post a Windows Message to
        /// a specific thread (identified by its thread id).
        /// We will need this API to post a WM_QUIT message to the main 
        /// thread in order to terminate this application.
        /// </summary>
        /// <param name="idThread"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        internal static extern bool PostThreadMessage(uint idThread, uint msg, UIntPtr wParam,
            IntPtr lParam);

        /// <summary>
        /// GetCurrentThreadId() allows us to obtain the thread id of the
        /// calling thread. This allows us to post the WM_QUIT message to
        /// the main thread.
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        internal static extern uint GetCurrentThreadId();

        /// <summary>
        /// SetProcessWorkingSetSize is typically used to increase the amount of RAM allocated
        /// for a process. Or to force a trim when the app knows that it is going to
        /// be idle for a long time. 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="minimumWorkingSetSize"></param>
        /// <param name="maximumWorkingSetSize"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);
    }
}
