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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using ASCOM.DeviceInterface;
using ASCOM.Utilities;
using SimServer.Domain;
using SimServer.Helpers;
using Mount;
using Principles;

namespace SimServer
{
    /// <summary>
    /// Main class for SimScope - ASCOM Telescope Control Simulator
    /// </summary>
    public static class Scope
    {
        #region Events 

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        #endregion

        #region Fields

        private static readonly object _timerLock = new object();
        private static readonly Util _util = new Util();
        private static MediaTimer _mediatimer;
        private static long _idQueueCount;

        private static Vector _rateAxes;
        private static Vector _guideRate;
        private static Vector _rateRaDec;
        private static Vector _mountAxes;
        private static Vector _targetRaDec;
        private static double SlewSpeedOne;
        private static double SlewSpeedTwo;
        private static double SlewSpeedThree;
        private static double SlewSpeedFour;
        private static double SlewSpeedFive;
        private static double SlewSpeedSix;
        private static double SlewSpeedSeven;
        public static double SlewSpeedEight;

        #endregion

        #region Properties

        // Alt Az
        public static double Altitude { get; private set; }
        public static double Azimuth { get; private set; }
        // Dec
        private static double Declination
        {
            get; set;
        }
        public static double DecConverted
        {
            get; private set;
        }
        public static double DecRate
        {
            get => _rateRaDec.Y;
            set
            {
                _rateRaDec.Y = value;
                var unused = new CmdRate(GetQueueId(), AxisId.Axis2, value);
            }
        }
        public static string DecPosition
        {
            get; private set;
        }
        public static double DecRateAxis
        {
            set
            {
                _rateAxes.Y = value;
                if (Math.Abs(value) > 0)
                {
                    IsSlewing = true;
                    SlewState = SlewType.SlewMoveAxis;
                }
                else
                {
                    IsSlewing = false;
                    SlewState = SlewType.SlewNone;
                }
                var unused = new CmdRateAxis(GetQueueId(), AxisId.Axis2, value);
            }
        }
        public static double DecGuideRate
        {
            get => _guideRate.Y; set => _guideRate.Y = value;
        }
        public static double DecTarget
        {
            get => _targetRaDec.Y; set => _targetRaDec.Y = value;
        }
        private static bool IsDecPulseGuiding
        {
            get; set;
        }
        public static int DecSteps
        {
            get; private set;
        }
        // Ra
        private static double RightAscension
        {
            get; set;
        }
        public static double RaConverted
        {
            get; private set;
        }
        public static double RaRate
        {
            get => _rateRaDec.X;
            set
            {
                _rateRaDec.X = value;
                var unused = new CmdRate(GetQueueId(), AxisId.Axis1, value);
            }
        }
        public static string RaPosition
        {
            get; private set;
        }
        public static double RaRateAxis
        {
            set
            {
                _rateAxes.X = value;
                if (Math.Abs(value) > 0)
                {
                    IsSlewing = true;
                    SlewState = SlewType.SlewMoveAxis;
                }
                else
                {
                    IsSlewing = false;
                    SlewState = SlewType.SlewNone;
                }
                var unused = new CmdRateAxis(GetQueueId(), AxisId.Axis1, value);
            }
        }
        public static double RaGuideRate
        {
            get => _guideRate.X; set => _guideRate.X = value;
        }
        public static double RaTarget
        {
            get => _targetRaDec.X; set => _targetRaDec.X = value;
        }
        private static bool IsRaPulseGuiding
        {
            get; set;
        }
        public static int RaSteps { get; private set; }
        // Mount
        private static AxisStatus Axis1Status { get; set; }
        private static AxisStatus Axis2Status { get; set; }
        private static MountInfo _capabilities;
        public static MountInfo Capabilities
        {
            get => _capabilities;
            private set
            {
                _capabilities = value;
                OnStaticPropertyChanged();
            }
        }
        private static string DriverName { get; set; }

        // Scope
        public static bool AtHome
        {
            get
            {
                var h = new Vector(Settings.HomeX, Settings.HomeY);
                var m = new Vector(Math.Abs(_mountAxes.X), _mountAxes.Y); // Abs is for S Hemi
                var x = (m - h);
                var r = x.LengthSquared < 0.001;
                return r;
            }
        }
        private static bool _limitAlarm;
        public static bool LimitAlarm
        {
            get => _limitAlarm;
            private set
            {
                _limitAlarm = value;
                OnStaticPropertyChanged();
            }
        }
        private static bool _isHome;
        public static bool IsHome
        {
            get => _isHome;
            private set
            {
                _isHome = value;
                OnStaticPropertyChanged();
            }
        }
        public static bool IsPulseGuiding
        {
            get
            {
                if (IsDecPulseGuiding || IsRaPulseGuiding) return true;
                return false;
            }
        }
        public static bool IsSlewing
        {
            get; private set;
        }
        private static bool _openSetupDialog;
        public static bool OpenSetupDialog
        {
            get => _openSetupDialog;
            set
            {
                _openSetupDialog = value;
                OpenSetupDialogFinished = !value;
                OnStaticPropertyChanged();
            }
        }
        public static bool OpenSetupDialogFinished
        {
            get; set;
        }
        public static double SiderealTime
        {
            get; private set;
        }
        public static short SlewSettleTime
        {
            get; set;
        }
        public static bool SouthernHemisphere { get; set; }
        private static int TimerOverruns
        {
            get; set;
        }
        private static bool _tracking;
        public static bool Tracking
        {
            get => _trackingMode != TrackingMode.Off;
            set
            {
                if (value == _tracking) return;
                _tracking = value;

                if (value)
                {
                    if (Settings.AtPark)
                    {
                        throw new ASCOM.ParkedException("Cannot track when parked");
                    }

                    switch (Settings.AlignmentMode)
                    {
                        case AlignmentModes.algAltAz:
                            _trackingMode = TrackingMode.AltAz;
                            break;
                        case AlignmentModes.algGermanPolar:
                        case AlignmentModes.algPolar:
                            _trackingMode = Settings.Latitude >= 0 ? TrackingMode.EqN : TrackingMode.EqS;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                }
                else
                {
                    _trackingMode = TrackingMode.Off;
                }

                SetTracking(value);
                OnStaticPropertyChanged();
            }
        }
        private static PierSide _pierSide;
        public static PierSide PierSide
        {
            get => _pierSide;
            private set
            {
                if (_pierSide == value) return;
                _pierSide = value;
                OnStaticPropertyChanged();
            }
        }
        public static SlewType SlewState
        {
            get;
            private set;
        }
        public static PierSide SideOfPier
        {
            get
            {
                if (SouthernHemisphere)
                {
                    return _mountAxes.Y <= 90 && _mountAxes.Y >= -90 ? PierSide.pierWest : PierSide.pierEast;
                }

                return _mountAxes.Y <= 90 && _mountAxes.Y >= -90 ? PierSide.pierEast : PierSide.pierWest;
            }
            set
            {
                double pa;
                if (SouthernHemisphere)
                {
                    pa = _mountAxes.X;
                    if (pa <= 0 || pa >= Settings.HourAngleLimit)
                    {
                        throw new InvalidOperationException($"South Hemi SideOfPier to {value} is out of range limits: {Settings.HourAngleLimit}");
                    }
                    // change the pier side
                    SlewAxes(_mountAxes.Y,_mountAxes.X, SlewType.SlewRaDec);
                }
                else
                { 
                    // check the new side can be reached
                    pa = Range.Range360(_mountAxes.X - 180);
                    if (pa >= Settings.HourAngleLimit + 180 || pa <= -Settings.HourAngleLimit)
                    {
                        throw new InvalidOperationException($"North Hemi SideOfPier to {value} is out of range limits: {Settings.HourAngleLimit}");
                    }
                    // change the pier side
                    SlewAxes(180 - _mountAxes.Y, pa, SlewType.SlewRaDec);
                }
            }
        }
        private static TrackingMode _trackingMode;


        #endregion

        static Scope()
        {
            try
            {
                //defaults
                Settings.Load();
                SouthernHemisphere = Settings.Latitude < 0;
                SlewState = SlewType.SlewNone;
                SlewSettleTime = 0;
                RaRateAxis = 0;
                DecRateAxis = 0;
                RaRate = 0;
                DecRate = 0;
                RaTarget = double.NaN;
                DecTarget = double.NaN;
                SetHcSpeeds(Settings.MaximumSlewRate);
                SetGuideRates();
                ConnectMount();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        #region Scope Methods     

        /// <summary>
        /// Time to stop slewing
        /// </summary>
        public static void AbortSlew()
        {
            IsSlewing = false;
            Tracking = false;
            RaRateAxis = 0;
            DecRateAxis = 0;
            RaRate = 0;
            DecRate = 0;
            SlewState = SlewType.SlewNone;
            var unused = new CmdAxisStop(0, AxisId.Axis1);
            var dummy = new CmdAxisStop(0, AxisId.Axis2);
        }

        /// <summary>
        /// For initial connection to mount
        /// </summary>
        private static void ConnectMount()
        {
            // Once serial connection is established pass to queues
            //var conn = Connection.ConnectSerial;
            MountQueue.Serial = Connection.Serial;

            if (!Connection.IsConnected()) return;

            var a = new CmdDriverName(GetQueueId());
            DriverName = (string)MountQueue.GetCommandResult(a.Id).Result;
            if (!a.Successful) throw new Exception("Cannot find mount");
            if(DriverName != "SimScope") throw new Exception("Cannot find SimScope");

            var cap = new CmdCapabilities(GetQueueId());
            Capabilities = (MountInfo)MountQueue.GetCommandResult(cap.Id).Result;

            // Set default mount positions
            if (Settings.AtPark)
            {
                var unused = new CmdAxisToDegrees(0, AxisId.Axis1, Settings.ParkX);
                var dummy = new CmdAxisToDegrees(0, AxisId.Axis2, Settings.ParkY);
            }
            else
            {
                var unused = new CmdAxisToDegrees(0, AxisId.Axis1, Settings.HomeX);
                var dummy = new CmdAxisToDegrees(0, AxisId.Axis2, Settings.HomeY);
            }

            if (Settings.AutoTrack)
            {
                Settings.AtPark = false;
                Tracking = Settings.AutoTrack;
            }

            // Event to get mount data and update UI
            _mediatimer = new MediaTimer {Period = Settings.UIInterval};
            _mediatimer.Tick += UpdateUIEvent;
            _mediatimer.Start();
        }

        /// <summary>
        /// Slew state based on axis status
        /// </summary>
        private static void CheckSlewState()
        {
            var x = Axis1Status;
            var y = Axis2Status;
            if (x.Slewing || y.Slewing)
            {
                IsSlewing = true;
                return;
            }
            switch (SlewState)
            {
                case SlewType.SlewNone:
                    break;
                case SlewType.SlewSettle:
                    break;
                case SlewType.SlewMoveAxis:
                    break;
                case SlewType.SlewRaDec:
                    break;
                case SlewType.SlewAltAz:
                    break;
                case SlewType.SlewPark:
                    Tracking = false;
                    Settings.AtPark = true;
                    break;
                case SlewType.SlewHome:
                    break;
                case SlewType.SlewHandpad:
                    break;
            }
            IsSlewing = false;
            SlewState = SlewType.SlewNone;
        }

        /// <summary>
        /// Checks the axis limits. AltAz and Polar mounts allow continuous movement,
        /// GEM mounts check the hour angle limit.
        /// </summary>
        private static void CheckAxisLimits()
        {
            var limitHit = false;
            // check the ranges of the axes
            // primary axis must be in the range 0 to 360 for AltAz or Polar
            // and -hourAngleLimit to 180 + hourAngleLimit for german polar
            switch (Settings.AlignmentMode)
            {
                case AlignmentModes.algAltAz:
                    // the primary axis must be in the range 0 to 360
                    //_mountAxes.X = Range.Range360(_mountAxes.X);
                    break;
                case AlignmentModes.algGermanPolar:
                    // the primary axis needs to be in the range -180 to +180 to correspond with hour angles of -12 to 12.
                    // check if we have hit the hour angle limit
                    if (SouthernHemisphere)
                    {
                        if (_mountAxes.X >= Settings.HourAngleLimit || _mountAxes.X <= -Settings.HourAngleLimit - 180)
                        {
                            limitHit = true;
                        }
                    }
                    else
                    {
                        if (_mountAxes.X >= Settings.HourAngleLimit + 180  || _mountAxes.X <= -Settings.HourAngleLimit )
                        {
                            limitHit = true;
                        }
                    }
                    break;
                case AlignmentModes.algPolar:
                    // the axis needs to be in the range -180 to +180 to correspond with hour angles
                    //_mountAxes.X = Range.Range180(_mountAxes.X);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // secondary must be in the range -90 to 0 to +90 for normal 
            // and +90 to 180 to 270 for through the pole.
            // rotation is continuous
            //_mountAxes.X = Range.Range270(_mountAxes.X);

            if (limitHit) Tracking = false;
            LimitAlarm = limitHit;
        }

        /// <summary>
        /// Current tracking rate in arcseconds per degree
        /// </summary>
        /// <returns></returns>
        private static double GetCurrentTrackingRate()
        {
            double rate;
            switch (Settings.TrackingRate)
            {
                case DriveRates.driveSidereal:
                    rate = Settings.SiderealRate;
                    break;
                case DriveRates.driveSolar:
                    rate = Settings.SolarRate;
                    break;
                case DriveRates.driveLunar:
                    rate = Settings.LunarRate;
                    break;
                case DriveRates.driveKing:
                    rate = Settings.KingRate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return rate;
        }

        /// <summary>
        /// Used as tranactions ids to run mount commands and look up results
        /// </summary>
        /// <returns></returns>
        private static long GetQueueId()
        {
            return Interlocked.Increment(ref _idQueueCount);
        }

        /// <summary>
        /// Slew home
        /// </summary>
        public static void GoToHome()
        {
            Tracking = false;
            SlewAxes(Settings.HomeY, Settings.HomeX, SlewType.SlewHome);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void GoToPark()
        {
            Tracking = false;
            SlewAxes(Settings.ParkY, Settings.ParkX, SlewType.SlewPark);
        }

        /// <summary>
        /// return the change in axis values as a result of any HC button presses
        /// </summary>
        /// <returns></returns>
        public static void HcMoves(SlewSpeed speed, SlewDirection direction)
        {
            var change = new double[]{0, 0};
            double delta;
            switch (speed)
            {
                case SlewSpeed.One:
                    delta = SlewSpeedOne;
                    break;
                case SlewSpeed.Two:
                    delta = SlewSpeedTwo;
                    break;
                case SlewSpeed.Three:
                    delta = SlewSpeedThree;
                    break;
                case SlewSpeed.Four:
                    delta = SlewSpeedFour;
                    break;
                case SlewSpeed.Five:
                    delta = SlewSpeedFive;
                    break;
                case SlewSpeed.Six:
                    delta = SlewSpeedSix;
                    break;
                case SlewSpeed.Seven:
                    delta = SlewSpeedSeven;
                    break;
                case SlewSpeed.Eight:
                    delta = SlewSpeedEight;
                    break;
                default:
                    delta = 0;
                    break;
            }

            // check the button states
            switch (direction)
            {
                case SlewDirection.SlewNorth:
                case SlewDirection.SlewUp:
                    change[1] = delta;
                    break;
                case SlewDirection.SlewSouth:
                case SlewDirection.SlewDown:
                    change[1] = -delta;
                    break;
                case SlewDirection.SlewEast:
                case SlewDirection.SlewLeft:
                    change[0] = SouthernHemisphere ? -delta : delta;
                    break;
                case SlewDirection.SlewWest:
                case SlewDirection.SlewRight:
                    change[0] = SouthernHemisphere ? delta : -delta;
                    break;
                case SlewDirection.SlewNone:
                    break;
                default:
                    change[0] = 0;
                    change[1] = 0;
                    break;
            }

            var unused = new CmdHcSlew(0, AxisId.Axis1, change[0]);
            var dummy = new CmdHcSlew(0, AxisId.Axis2, change[1]);


        }

        /// <summary>
        /// Called from the setter property.  Used to update UI elements.  propertyname is not required
        /// </summary>
        /// <param name="propertyName"></param>
        private static void OnStaticPropertyChanged([CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Pulse commands
        /// </summary>
        /// <param name="direction">GuideDirections</param>
        /// <param name="duration">in milliseconds</param>
        public static void PulseGuide(GuideDirections direction, int duration)
        {
            if (duration == 0)
            {
                // stops the current guide command
                switch (direction)
                {
                    case GuideDirections.guideNorth:
                    case GuideDirections.guideSouth:
                        IsDecPulseGuiding = false;
                        break;
                    case GuideDirections.guideEast:
                    case GuideDirections.guideWest:
                        IsRaPulseGuiding = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }
                return;
            }
            switch (direction)
            {
                case GuideDirections.guideNorth:
                case GuideDirections.guideSouth:
                    IsDecPulseGuiding = true;
                    break;
                case GuideDirections.guideEast:
                case GuideDirections.guideWest:
                    IsRaPulseGuiding = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            if (IsRaPulseGuiding)
            {
                var raGuideRate = Math.Abs(RaGuideRate);
                if (SouthernHemisphere)
                {
                    if (direction == GuideDirections.guideWest) raGuideRate = -raGuideRate;
                }
                else
                {
                    if (direction == GuideDirections.guideEast) raGuideRate = -raGuideRate;
                }

                var ra = new CmdAxisPulse(GetQueueId(), AxisId.Axis1, raGuideRate, duration);
                IsRaPulseGuiding = (bool)MountQueue.GetCommandResult(ra.Id).Result;
            }

            if (IsDecPulseGuiding)
            {
                var decGuideRate = Math.Abs(DecGuideRate);
                if (SideOfPier == PierSide.pierEast)
                {
                    if (direction == GuideDirections.guideNorth) decGuideRate = -decGuideRate;
                }
                else
                {
                    if (direction == GuideDirections.guideSouth) decGuideRate = -decGuideRate;
                }
                var dec = new CmdAxisPulse(GetQueueId(), AxisId.Axis2, decGuideRate, duration);
                IsDecPulseGuiding = (bool)MountQueue.GetCommandResult(dec.Id).Result;
            }

        }

        /// <summary>
        /// Setup guide rates
        /// </summary>
        public static void SetGuideRates()
        {
            var rate = GetCurrentTrackingRate();
            RaGuideRate = rate * Settings.GuideRateXPer;
            DecGuideRate = rate * Settings.GuideRateYPer;
        }

        /// <summary>
        /// Setup speeds for hand controller
        /// </summary>
        private static void SetHcSpeeds(double maxrate)
        {
            SlewSpeedOne = Math.Round(maxrate * 0.0034, 3);
            SlewSpeedTwo = Math.Round(maxrate * 0.0068, 3);
            SlewSpeedThree = Math.Round(maxrate * 0.047, 3);
            SlewSpeedFour = Math.Round(maxrate * 0.068, 3);
            SlewSpeedFive = Math.Round(maxrate * 0.2, 3);
            SlewSpeedSix = Math.Round(maxrate * 0.4, 3);
            SlewSpeedSeven = Math.Round(maxrate * 0.8, 3);
            SlewSpeedEight = Math.Round(maxrate * 1, 3);
        }

        /// <summary>
        /// Set park location
        /// </summary>
        public static void SetParkAxes()
        {
            Settings.ParkY = _mountAxes.Y;
            Settings.ParkX = _mountAxes.X;
            Settings.AtPark = true;
        }

        /// <summary>
        /// Gets the side of pier using the right ascension, assuming it depends on the
        /// hour angle only.  Used for Destination side of Pier, NOT to determine the mount
        /// pointing state
        /// </summary>
        /// <param name="rightAscension">The right ascension.</param>
        /// <returns></returns>
        public static PierSide SideOfPierRaDec(double rightAscension)
        {
            if (Settings.AlignmentMode != AlignmentModes.algGermanPolar)
            {
                return PierSide.pierUnknown;
            }

            var ha = Coordinate.Ra2Ha(rightAscension, SiderealTime);
            PierSide sideOfPier;
            if (ha < 0.0 && ha >= -12.0) sideOfPier = PierSide.pierWest;
            else if (ha >= 0.0 && ha <= 12.0) sideOfPier = PierSide.pierEast;
            else sideOfPier = PierSide.pierUnknown;
            return sideOfPier;
        }

        /// <summary>
        /// Send tracking on or off to mount
        /// </summary>
        /// <param name="on"></param>
        private static void SetTracking(bool on)
        {
            double rateChange = 0;
            if (on)
            {
                rateChange = GetCurrentTrackingRate();
            }

            if (SouthernHemisphere) rateChange = -Math.Abs(rateChange);

            var unused = new CmdAxisTracking(0, AxisId.Axis1, rateChange);
        }

        /// <summary>
        /// Slew that a way
        /// </summary>
        /// <param name="targetAltAz"></param>
        private static void SlewAltAz(double[] targetAltAz)
        {
            var target = Axes.AltAzToAxesYX(targetAltAz);
            SlewMount(target[0], target[1], SlewType.SlewAltAz);
        }

        /// <summary>
        /// Slew exit stage left
        /// </summary>
        /// <param name="altitude"></param>
        /// <param name="azimuth"></param>
        public static void SlewAltAz(double altitude, double azimuth)
        {
            SlewAltAz(new[] {altitude, azimuth});
        }

        /// <summary>
        /// Slew this a way
        /// </summary>
        /// <param name="secondaryAxis"></param>
        /// <param name="primaryAxis"></param>
        /// <param name="slewState"></param>
        private static void SlewAxes(double secondaryAxis, double primaryAxis, SlewType slewState)
        {
            SlewMount(secondaryAxis, primaryAxis, slewState);
        }

        /// <summary>
        /// Starts a slew to the target position in degrees.
        /// </summary>
        /// <param name="targety"></param>
        /// <param name="targetx"></param>
        /// <param name="slewState"></param>
        private static void SlewMount(double targety, double targetx, SlewType slewState)
        {
            SlewState = slewState;
            IsSlewing = true;
            Settings.AtPark = false;
            //todo checklimit
            var axes = Axes.SimToMount(new[] {targetx, targety});
            var unused = new CmdAxisGoToTarget(0, AxisId.Axis1, axes[0]);
            var dummy = new CmdAxisGoToTarget(0, AxisId.Axis2, axes[1]);
        }

        /// <summary>
        /// Slew exit stage right
        /// </summary>
        /// <param name="rightAscension"></param>
        /// <param name="declination"></param>
        public static void SlewRaDec(double rightAscension, double declination)
        {
            var raDec = new[] {rightAscension, declination};
            var targetAxes = Axes.RaDecToAxesXY(raDec);
            SlewAxes(targetAxes[1], targetAxes[0], SlewType.SlewRaDec);
        }

        /// <summary>
        /// Sync pointing to Alt and Az
        /// </summary>
        /// <param name="altitude"></param>
        /// <param name="azimuth"></param>
        public static void SyncToAltAzm(double altitude, double azimuth)
        {
            var axes = Axes.AltAzToAxesYX(new[] {altitude, azimuth});
            axes = Axes.SimToMount(new[] { axes[1], axes[0]});
            var unused = new CmdAxisToDegrees(0, AxisId.Axis1, axes[0]);
            var dummy = new CmdAxisToDegrees(0, AxisId.Axis2, axes[1]);
        }

        /// <summary>
        /// Sync pointing to Ra and Dec
        /// </summary>
        public static void SyncToTargetRaDec()
        {
            var axes = Axes.RaDecToAxesXY(new[] {RightAscension, Declination});
            axes = Axes.SimToMount(axes);
            var unused = new CmdAxisToDegrees(0, AxisId.Axis1, axes[0]);
            var dummy = new CmdAxisToDegrees(0, AxisId.Axis2, axes[1]);
        }

        /// <summary>
        /// Event to get mount positions and update display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UpdateUIEvent(object sender, EventArgs e)
        {
            var hasLock = false;
            try
            {
                //stops the overrun of previous event not ended before next one starts
                Monitor.TryEnter(_timerLock, ref hasLock);
                if (!hasLock)
                {
                    TimerOverruns++;
                    return;
                }

                // calculate current lst
                SiderealTime = Time.Lst(JDate.Epoch2000Days(), _util.JulianDate, false, Settings.Longitude);

                // get mount axis positions in degrees
                var cmdD = new CmdAxesDegrees(GetQueueId());
                var axisDegrees = (double[])MountQueue.GetCommandResult(cmdD.Id).Result;
                var axes = Axes.MountToSim(axisDegrees);

                //local to just track positions
                _mountAxes.X = axes[0];
                _mountAxes.Y = axes[1];

                CheckAxisLimits();
                PierSide = SideOfPier;

                //update UI for degrees
                RaPosition = $"{axisDegrees[0]:0.000000}";
                DecPosition = $"{axisDegrees[1]:0.000000}";

                // convert to Ra and Dec
                var raDec = Axes.AxesXYToRaDec(axes);
                RightAscension = raDec[0];
                Declination = raDec[1];

                var c = Coords.Topo2Coords(raDec[0], raDec[1]);
                RaConverted = c[0];
                DecConverted = c[1];

                // convert to Az and Alt
                var azAlt = Axes.AxesXYToAzAlt(axes);
                Altitude = azAlt[1];
                //Azimuth = azAlt[0];
                Azimuth = Coords.Topo2CoordAz(raDec[0], raDec[1]);

                // get axis positions in steps
                var steps = new CmdAxisSteps(GetQueueId());
                var i = (int[]) MountQueue.GetCommandResult(steps.Id).Result;
                RaSteps = i[0];
                DecSteps = i[1];

                // get status
                var a = new CmdAxisStatus(GetQueueId(), AxisId.Axis1);
                Axis1Status = (AxisStatus) MountQueue.GetCommandResult(a.Id).Result;
                var b = new CmdAxisStatus(GetQueueId(), AxisId.Axis2);
                Axis2Status = (AxisStatus) MountQueue.GetCommandResult(b.Id).Result;

                CheckSlewState();

                //check if at home position
                IsHome = AtHome;
            }
            finally
            {
                if (hasLock) Monitor.Exit(_timerLock);
            }
        }

        #endregion
    }
}
