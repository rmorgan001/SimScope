using System;
using System.Windows.Input;

namespace SimServer.Helpers
{
    public sealed class WaitCursor : IDisposable
    {
        private Cursor _previousCursor;

        /// <summary>
        /// makes a wait cursor
        /// </summary>
        public WaitCursor()
        {
            if (Mouse.OverrideCursor == Cursors.Wait) return;
            _previousCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        /// <inheritdoc />
        /// <summary>
        /// change cursor back
        /// </summary>
        public void Dispose()
        {
            Mouse.OverrideCursor = _previousCursor;
            Dispose(true);
        }

        /// <summary>
        /// clean up managed and native
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (_previousCursor != null)
                {
                    _previousCursor.Dispose();
                    _previousCursor = null;
                }
            }
            // free native resources if there are any.
            //if (nativeResource != IntPtr.Zero)
            //{
            //    Marshal.FreeHGlobal(nativeResource);
            //    nativeResource = IntPtr.Zero;
            //}
        }
    }
}
