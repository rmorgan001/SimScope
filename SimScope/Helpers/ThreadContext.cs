using System;
using System.Threading;
using System.Windows;

namespace SimServer.Helpers
{
    public static class ThreadContext
    {
        /// <summary>
        /// Executes on the UI thread, but calling thread waits for completion before continuing.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void InvokeOnUiThread(Action action, CancellationToken token = default(CancellationToken))
        {
            if (Application.Current == null) return;
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                if (token.IsCancellationRequested) return;
                Application.Current.Dispatcher.Invoke(action);
            }
        }

        /// <summary>
        /// Executes on the UI thread, and calling thread doesn't wait for completion
        /// </summary>
        /// <param name="action"></param>
        public static void BeginInvokeOnUiThread(Action action)
        {
            if (Application.Current == null) return;
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(action);
            }
        }
    }
}
