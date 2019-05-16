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
using System.Threading;

namespace Mount
{
    /// <summary>
    /// Simulates a mount controller card that drives dual stepper motors
    /// </summary>
    internal class Controllers
    {
        #region Fields

        private static CancellationTokenSource _ctsMount = new CancellationTokenSource();
        private const int RevolutionSteps = 12960000;
        private const bool CanAxisSlewsIndependent = false;
        private const bool CanAzEq = false;
        private const bool CanDualEncoders = false;
        private const bool CanHalfTrack = false;
        private const bool CanHomeSensors = false;
        private const bool CanPolarLed = false;
        private const bool CanPpec = false;
        private const bool CanWifi = false;
        private const string MountName = "SimScope";
        private const string MountVersion = "1.0";
        private bool _running;
        private DateTime _lastUpdateTime;
        private double _gotoX;
        private double _gotoY;
        private double _pulseX;
        private double _pulseY;
        private double _rateX;
        private double _rateY;
        private double _rateAxisX;
        private double _rateAxisY;
        private double _slewX;
        private double _slewY;
        private double _trackingX;
        private double _trackingY;
        private bool _isStoppedX;
        private bool _isStoppedY;

        private bool _isSlewingX;
        private bool _isSlewingY;
        private bool _isTrackingX;
        private bool _isTrackingY;
        private bool _isRateTrackingX;
        private bool _isRateTrackingY;
        private bool _isRateAxisSlewingX;
        private bool _isRateAxisSlewingY;
        private bool _isSlewSlewingX;
        private bool _isSlewSlewingY;
        private bool _isGotoSlewingX;
        private bool _isGotoSlewingY;

        #endregion

        #region Properties

        private double DegreesX { get; set; }
        private double DegreesY { get; set; }
        private int StepsX => (int) (DegreesX * 36000);
        private int StepsY => (int) (DegreesY * 36000);
        private double HcX { get; set; }
        private double HcY { get; set; }

        #endregion

        internal Controllers()
        {
            DegreesX = 0;
            DegreesY = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckSlewing()
        {
            if (_isGotoSlewingX || _isRateAxisSlewingX || _isSlewSlewingX)
            {
                _isSlewingX = true;
            }
            else
            {
                _isSlewingX = false;
            }

            if (_isGotoSlewingY || _isRateAxisSlewingY || _isSlewSlewingY)
            {
                _isSlewingY = true;
            }
            else
            {
                _isSlewingY = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeX"></param>
        /// <param name="changeY"></param>
        /// <param name="seconds"></param>
        private void CheckStopped(double changeX, double changeY, double seconds)
        {
            if (Math.Abs(changeX) > 0)
            {
                _isStoppedX = false;
                DegreesX += changeX * seconds;
            }
            else
            {
                _isStoppedX = true;
            }

            if (Math.Abs(changeY) > 0)
            {
                _isStoppedY = false;
                DegreesY += changeY * seconds;
            }
            else
            {
                _isStoppedY = true;
            }
        }

        /// <summary>
        /// Process incomming commands
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal string Command(string command)
        {
            var cmd = command.Split(',');
            double a;
            switch (cmd[0].ToLower())
            {
                case "capabilities":
                    return
                        $"{CanAxisSlewsIndependent},{CanAzEq},{CanDualEncoders},{CanHalfTrack},{CanHomeSensors},{CanPolarLed},{CanPpec},{CanWifi},{MountName},{MountVersion}";
                case "initialize":
                    return Start().ToString();
                case "shutdown":
                    return Stop().ToString();
                case "rate":
                    a = Convert.ToDouble(cmd[2]);
                    switch (ParseAxis(cmd[1]))
                    {
                        case AxisId.Axis1:
                            _isRateTrackingX = true;
                            _rateX = a;
                            break;
                        case AxisId.Axis2:
                            _isRateTrackingY = true;
                            _rateY = a;
                            break;
                    }
                    break;
                case "rateaxis":
                    a = Convert.ToDouble(cmd[2]);
                    switch (ParseAxis(cmd[1]))
                    {
                        case AxisId.Axis1:
                            _isRateAxisSlewingX = true;
                            _rateAxisX = a;
                            break;
                        case AxisId.Axis2:
                            _isRateAxisSlewingY = true;
                            _rateAxisY = a;
                            break;
                    }
                    break;
                case "slew":
                    a = Convert.ToDouble(cmd[2]);
                    switch (ParseAxis(cmd[1]))
                    {
                        case AxisId.Axis1:
                            _isSlewSlewingX = true;
                            _slewX = a;
                            break;
                        case AxisId.Axis2:
                            _isSlewSlewingY = true;
                            _slewY = a;
                            break;
                    }

                    break;
                case "pulse":
                    a = Convert.ToDouble(cmd[2]);
                    switch (ParseAxis(cmd[1]))
                    {
                        case AxisId.Axis1:
                            _isSlewSlewingX = true;
                            _slewX = a;
                            break;
                        case AxisId.Axis2:
                            _isSlewSlewingY = true;
                            _slewY = a;
                            break;
                    }

                    break;
                case "degrees":
                    switch (ParseAxis(cmd[1]))
                    {
                        case AxisId.Axis1:
                            return $"{DegreesX}";
                        case AxisId.Axis2:
                            return $"{DegreesY}";
                    }

                    break;
                case "steps":
                    switch (ParseAxis(cmd[1]))
                    {
                        case AxisId.Axis1:
                            return $"{StepsX}";
                        case AxisId.Axis2:
                            return $"{StepsY}";
                    }

                    break;
                case "stop":
                    StopAxis(ParseAxis(cmd[1]));
                    break;
                case "hc":
                    a = Convert.ToDouble(cmd[2]);
                    switch (ParseAxis(cmd[1]))
                    {
                        case AxisId.Axis1:
                            HcX = a;
                            break;
                        case AxisId.Axis2:
                            HcY = a;
                            break;
                    }

                    break;
                case "tracking":
                    a = Convert.ToDouble(cmd[2]);
                    switch (ParseAxis(cmd[1]))
                    {
                        case AxisId.Axis1:
                            _trackingX = a;
                            break;
                        case AxisId.Axis2:
                            _trackingY = a;
                            break;
                    }

                    break;
                case "getrevsteps":
                    return $"{RevolutionSteps}";
                case "gototarget":
                    a = Convert.ToDouble(cmd[2]);
                    switch (ParseAxis(cmd[1]))
                    {
                        case AxisId.Axis1:
                            _isGotoSlewingX = true;
                            _gotoX = a;
                            break;
                        case AxisId.Axis2:
                            _isGotoSlewingY = true;
                            _gotoY = a;
                            break;
                    }

                    break;
                case "setdegrees":
                    a = Convert.ToDouble(cmd[2]);
                    switch (ParseAxis(cmd[1]))
                    {
                        case AxisId.Axis1:
                            DegreesX = a;
                            break;
                        case AxisId.Axis2:
                            DegreesY = a;
                            break;
                    }

                    break;
                case "axisstatus":
                    switch (ParseAxis(cmd[1]))
                    {
                        case AxisId.Axis1:
                            return $"{_isSlewingX},{_isStoppedX},{_isTrackingX},{_isRateTrackingY}";
                        case AxisId.Axis2:
                            return $"{_isSlewingY},{_isStoppedY},{_isTrackingY},{_isRateTrackingY}";
                    }

                    break;
                default:
                    return "!";

            }

            return "ok";
        }

        /// <summary>
        ///  GoTo Movement
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private double GoTo(AxisId axis)
        {
            var change = 0.0;
            var delta = 0.0;
            var sign = 1;
            switch (axis)
            {
                case AxisId.Axis1:
                    if (!_isGotoSlewingX || double.IsNaN(_gotoX))
                    {
                        _isGotoSlewingX = false;
                        return change;
                    }
                    delta = _gotoX - DegreesX;
                    sign = delta < 0 ? -1 : 1;
                    delta = Math.Abs(delta);
                    if (delta <= .0001)
                    {
                        _isGotoSlewingX = false;
                        delta = 0;
                    }
                    break;
                case AxisId.Axis2:
                    if (!_isGotoSlewingY || double.IsNaN(_gotoY))
                    {
                        _isGotoSlewingY = false;
                        return change;
                    }
                    delta = _gotoY - DegreesY;
                    sign = delta < 0 ? -1 : 1;
                    delta = Math.Abs(delta);
                    if (delta <= .0001)
                    {
                        _isGotoSlewingY = false;
                        delta = 0;
                    }
                    break;
            }

            if (delta <= 0) return change;
            switch (delta)
            {
                case var _ when delta <= .0001:
                    change = .00002 * sign;
                    break;
                case var _ when delta <= .01 && delta > .0001:
                    change = .01 * sign;
                    break;
                case var _ when delta <= .1 && delta > .01:
                    change = .1 * sign;
                    break;
                case var _ when delta <= 2 && delta > .1:
                    change = 2 * sign;
                    break;
                case var _ when delta > 2:
                    change = 10 * sign;
                    break;
            }

            return change;
        }

        /// <summary>
        /// Rate tracking Movement
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private double Rate(AxisId axis)
        {
            switch (axis)
            {
                case AxisId.Axis1:
                    _isRateTrackingX = Math.Abs(_rateX) > 0;
                    return _isRateTrackingX ? _rateX : 0;
                case AxisId.Axis2:
                    _isRateTrackingY = Math.Abs(_rateY) > 0;
                    return _isRateTrackingY ? _rateY : 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        /// <summary>
        /// Pulse Movement
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private double Pulse(AxisId axis)
        {
            switch (axis)
            {
                case AxisId.Axis1:
                    return Principles.Conversions.ArcSec2Deg(_pulseX);
                case AxisId.Axis2:
                    return Principles.Conversions.ArcSec2Deg(_pulseY);
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        /// <summary>
        /// RateAxis Movement
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private double RateAxis(AxisId axis)
        {
            switch (axis)
            {
                case AxisId.Axis1:
                    _isRateAxisSlewingX = Math.Abs(_rateAxisX) > 0;
                    return _isRateAxisSlewingX ? Principles.Conversions.ArcSec2Deg(_rateAxisX) : 0;
                case AxisId.Axis2:
                    _isRateAxisSlewingY = Math.Abs(_rateAxisY) > 0;
                    return _isRateAxisSlewingY ? Principles.Conversions.ArcSec2Deg(_rateAxisY) : 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        /// <summary>
        /// Slew Movement
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private double Slew(AxisId axis)
        {
            switch (axis)
            {
                case AxisId.Axis1:
                    _isSlewSlewingX = Math.Abs(_slewX) > 0;
                    return _isSlewSlewingX ? Principles.Conversions.ArcSec2Deg(_slewX) : 0;
                case AxisId.Axis2:
                    _isSlewSlewingY = Math.Abs(_slewY) > 0;
                    return _isSlewSlewingY ? Principles.Conversions.ArcSec2Deg(_slewY) : 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        /// <summary>
        /// Tracking Movement
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private double Tracking(AxisId axis)
        {
            switch (axis)
            {
                case AxisId.Axis1:
                    _isTrackingX = Math.Abs(_trackingX) > 0;
                    return _isTrackingX ? Principles.Conversions.ArcSec2Deg(_trackingX): 0;
                case AxisId.Axis2:
                    _isTrackingY = Math.Abs(_trackingY) > 0;
                    return _isTrackingY ? Principles.Conversions.ArcSec2Deg(_trackingY): 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        /// <summary>
        /// Mount Thread
        /// </summary>
        private async void MountLoopAsync()
        {
            try
            {
                if (_ctsMount == null) _ctsMount = new CancellationTokenSource();
                var ct = _ctsMount.Token;
                _running = true;
                _lastUpdateTime = Principles.HiResDateTime.UtcNow;
                var task = System.Threading.Tasks.Task.Run(() =>
                {
                    while (!ct.IsCancellationRequested)
                    {
                        MoveAxes();
                    }
                }, ct);
                await task;
                task.Wait(ct);
                _running = false;
            }
            catch (OperationCanceledException)
            {
                _running = false;
            }
            catch (Exception)
            {
                _running = false;
            }
        }

        /// <summary>
        /// Main loop
        /// </summary>
        private void MoveAxes()
        {
            Thread.Sleep(10);
            var now = Principles.HiResDateTime.UtcNow;
            var seconds = (now - _lastUpdateTime).TotalSeconds;
            _lastUpdateTime = now;
            var changeX = 0.0;
            var changeY = 0.0;

            // Pulse
            changeX += Pulse(AxisId.Axis1);
            changeY += Pulse(AxisId.Axis2);

            // RateAxis
            changeX += RateAxis(AxisId.Axis1);
            changeY += RateAxis(AxisId.Axis2);

            // Rate
            changeX += Rate(AxisId.Axis1);
            changeY += Rate(AxisId.Axis2);

            // Slew
            changeX += Slew(AxisId.Axis1);
            changeY += Slew(AxisId.Axis2);

            // Tracking
            changeX += Tracking(AxisId.Axis1);
            changeY += Tracking(AxisId.Axis2);

            // Hand controls
            changeX += HcX;
            changeY += HcY;

            // Slewing
            changeX += GoTo(AxisId.Axis1);
            changeY += GoTo(AxisId.Axis2);

            CheckStopped(changeX, changeY, seconds);
            CheckSlewing();
        }

        /// <summary>
        /// Convert axis string to axis id
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private static AxisId ParseAxis(string axis)
        {
            switch (axis)
            {
                case "axis1":
                    return AxisId.Axis1;
                case "axis2":
                    return AxisId.Axis2;
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Initialize Mount
        /// </summary>
        private bool Start()
        {
            if (!_running) MountLoopAsync();
            return _running;
        }

        /// <summary>
        /// Shutdown Mount
        /// </summary>
        private static bool Stop()
        {
            _ctsMount?.Cancel();
            return true;
        }

        /// <summary>
        /// Complete Stop
        /// </summary>
        /// <param name="axis"></param>
        private void StopAxis(AxisId axis)
        {
            switch (axis)
            {
                case AxisId.Axis1:
                    _rateX = 0;
                    _gotoX = double.NaN;
                    _rateAxisX = 0;
                    _slewX = 0;
                    _trackingX = 0;
                    _pulseX = 0;
                    HcX = 0;
                    break;
                case AxisId.Axis2:
                    _rateY = 0;
                    _gotoY = double.NaN;
                    _rateAxisY = 0;
                    _slewY = 0;
                    _trackingY = 0;
                    _pulseY = 0;
                    HcY = 0;
                    break;
            }
        }
    } 
}
