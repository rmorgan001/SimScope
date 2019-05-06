using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Mount
{
    [Serializable]
    public class MountException : Exception
    {
        public ErrorCode ErrorCode { get; }

        public MountException()
        {
        }

        public MountException(ErrorCode err) : base($"Mount: {err}")
        {
            ErrorCode = err;
        }

        public MountException(ErrorCode err, string message) : base($"Mount: {err}, {message}")
        {
            ErrorCode = err;
        }

        public MountException(ErrorCode err, string message, Exception inner) : base($"Mount: {err}, {message}", inner)
        {
            ErrorCode = err;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        // Constructor should be protected for unsealed classes, private for sealed classes.
        // (The Serializer invokes this constructor through reflection, so it can be private)
        protected MountException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Enum.TryParse("err", out ErrorCode err);
            ErrorCode = err;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            info.AddValue("err", ErrorCode.ToString());
            // MUST call through to the base class to let it save its own state
            base.GetObjectData(info, context);
        }
    }
}
