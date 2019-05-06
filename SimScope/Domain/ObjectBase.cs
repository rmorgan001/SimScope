using System.Runtime.InteropServices;

namespace SimServer.Domain
{
    [ComVisible(false)]
    public class ObjectBase
    {
        protected ObjectBase()
        {
            // We increment the global count of objects.
            Server.CountObject();
        }

        ~ObjectBase()
        {
            // We decrement the global count of objects.
            Server.UncountObject();
            // We then immediately test to see if we the conditions are right to attempt to terminate this server application.
            Server.ExitIf();
        }
    }
}
