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

namespace Mount
{
    /// <summary>
    /// Actions or commands send to the mount  
    /// </summary>
    public class Actions
    {
        #region Fields

        private readonly IOSerial _ioserial;
        private readonly double[] _stepsPerSec = { 0.0, 0.0 };
        private readonly int[] _revSteps = { 0, 0 };

        #endregion

        #region Properties

        internal static bool IsConnected => true;
        internal MountInfo MountInfo { get; private set; }

        #endregion

        internal Actions()
        {
            _ioserial = new IOSerial();
        }

        #region Action Commands

        /// <summary>
        /// Angle to step
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        internal long AngleToStep(AxisId axis, double angle)
        {
            switch (axis)
            {
                case AxisId.Axis1:
                    return (long)(angle * 36000);
                case AxisId.Axis2:
                    return (long)(angle * 36000);
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        /// <summary>
        /// Gat axes in degrees
        /// </summary>
        /// <returns></returns>
        internal double[] AxesDegrees()
        {
            var x = Convert.ToDouble(_ioserial.Send($"degrees,{AxisId.Axis1}"));
            var y = Convert.ToDouble(_ioserial.Send($"degrees,{AxisId.Axis2}"));
            var d = new[] { x, y };
            return d;
        }

        /// <summary>
        /// Slew to target
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="target"></param>
        internal void AxisGoToTarget(AxisId axis, double target)
        {
            _ioserial.Send($"gototarget,{axis},{target}");
        }

        /// <summary>
        /// Set an axis in degrees
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="degrees"></param>
        internal void AxisToDegrees(AxisId axis, double degrees)
        {
            _ioserial.Send($"setdegrees,{axis},{degrees}");
        }

        /// <summary>
        /// Send pulse
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="guiderate"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        internal bool AxisPulse(AxisId axis, double guiderate, int duration)
        {
            var arcsecs = duration / 1000.0 * Principles.Conversions.ArcSec2Deg (Math.Abs(guiderate));

            switch (axis)
            {
                case AxisId.Axis1:
                    //Check for minimum pulse or a pulse less than 1 step
                    if (arcsecs < .0002) return false;
                    break;
                case AxisId.Axis2:
                    if (arcsecs < .0002) return false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }

            _ioserial.Send($"slew,{axis},{guiderate}");
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalMilliseconds < duration)
            {
                //do something while waiting
            }
            sw.Reset();
            _ioserial.Send($"slew,{axis},{0}");

            return false;
        }

        /// <summary>
        /// Start axis tracking
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="degrees"></param>
        internal void AxisTracking(AxisId axis, double degrees)
        {
            _ioserial.Send($"tracking,{axis},{degrees}");
        }

        /// <summary>
        /// Slew an axis
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="degrees"></param>
        internal void AxisSlew(AxisId axis, double degrees)
        {
            _ioserial.Send($"slew,{axis},{degrees}");
        }

        /// <summary>
        /// Get axis status
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public AxisStatus AxisStatus(AxisId axis)
        {
            var a = _ioserial.Send($"axisstatus,{axis}");
            var strings = a.Split(',');
            var b = new AxisStatus()
            {
                Axis = axis,
                Slewing = Convert.ToBoolean(strings[0]),
                Stopped = Convert.ToBoolean(strings[1]),
                Tracking = Convert.ToBoolean(strings[2]),
                Rate = Convert.ToBoolean(strings[3])
            };
            return b;
        }

        /// <summary>
        /// Stop an axis
        /// </summary>
        /// <param name="axis"></param>
        internal void AxisStop(AxisId axis)
        {
            _ioserial.Send($"stop,{axis}");
        }

        /// <summary>
        /// Get axis in steps
        /// </summary>
        /// <returns></returns>
        internal int[] AxisSteps()
        {
            var x = Convert.ToInt32(_ioserial.Send($"steps,{AxisId.Axis1}"));
            var y = Convert.ToInt32(_ioserial.Send($"steps,{AxisId.Axis2}"));
            var i = new[] { x, y };
            return i;
        }

        /// <summary>
        /// Hand control slew
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="degrees"></param>
        internal void HcSlew(AxisId axis, double degrees)
        {
            _ioserial.Send($"hc,{axis},{degrees}");
        }

        /// <summary>
        /// Initialize
        /// </summary>
        internal void InitializeAxes()
        {
            _ioserial.Send("initialize");
            LoadDefaults();
        }

        /// <summary>
        /// Loads mount defaults, both axes are the same size
        /// </summary>
        private void LoadDefaults()
        {
            //steps per revolution
            var a = Convert.ToInt32(_ioserial.Send("getrevsteps"));
            _revSteps[0] = a;
            _revSteps[1] = a;
            //steps per second
            var b = Principles.Conversions.StepPerArcsec(a);
            _stepsPerSec[0] = b;
            _stepsPerSec[1] = b;

            var c = _ioserial.Send("capabilities");
            var d = c.Split(',');
            var mountInfo = new MountInfo
            {
                CanAxisSlewsIndependent = Convert.ToBoolean(d[0]),
                CanAzEq = Convert.ToBoolean(d[1]),
                CanDualEncoders = Convert.ToBoolean(d[2]),
                CanHalfTrack = Convert.ToBoolean(d[3]),
                CanHomeSensors = Convert.ToBoolean(d[4]),
                CanPolarLed = Convert.ToBoolean(d[5]),
                CanPpec = Convert.ToBoolean(d[6]),
                CanWifi = Convert.ToBoolean(d[7]),
                MountName = d[8],
                MountVersion = d[9]
            };
            MountInfo = mountInfo;
        }

        /// <summary>
        /// Get name
        /// </summary>
        /// <returns></returns>
        internal string MountName()
        {
            return _ioserial.Send("mountname");
        }

        /// <summary>
        /// Gets version
        /// </summary>
        /// <returns></returns>
        internal string MountVersion()
        {
            return _ioserial.Send("mountversion");
        }

        /// <summary>
        /// Gets Steps Per Revolution
        /// </summary>
        /// <returns></returns>
        internal long Spr()
        {
            return Convert.ToInt32(_ioserial.Send("spr"));
        }

        /// <summary>
        /// Rates from driver
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="degrees"></param>
        internal void Rate(AxisId axis, double degrees)
        {
            _ioserial.Send($"rate,{axis},{degrees}");
        }

        /// <summary>
        /// Axis rates from driver
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="degrees"></param>
        internal void RateAxis(AxisId axis, double degrees)
        {
            _ioserial.Send($"rateaxis,{axis},{degrees}");
        }

        /// <summary>
        /// Shutdown
        /// </summary>
        internal void Shutdown()
        {
            _ioserial.Send("shutdown");
        }
        #endregion
    }
}
