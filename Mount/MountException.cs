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
