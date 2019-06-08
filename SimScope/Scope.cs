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
// ReSharper disable RedundantAssignment

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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

        private static Vector _homeAxes;
        private static Vector _rateAxes;
        private static Vector _guideRate;
        private static Vector _rateRaDec;
        private static Vector _mountAxes;
        private static Vector _targetRaDec;
        private static Vector _altAzSync;
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
            get => Conversions.Deg2ArcSec(_rateRaDec.Y);
            set
            {
                _rateRaDec.Y = Conversions.ArcSec2Deg(value);
                var unused = new CmdRate(MountQueue.NewId, AxisId.Axis2, _rateRaDec.Y);
            }
        }
        public static string DecPosition
        {
            get; private set;
        }
        public static double DecRateAxis
        {
            private get => _rateAxes.Y;
            set
            {
                _rateAxes.Y = Conversions.ArcSec2Deg(value);
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
                var unused = new CmdRateAxis(0, AxisId.Axis2, _rateAxes.Y);
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
            get => Conversions.Deg2ArcSec(_rateRaDec.X);
            set
            {
                _rateRaDec.X = Conversions.ArcSec2Deg(value);
                var unused = new CmdRate(MountQueue.NewId, AxisId.Axis1, _rateRaDec.X);
            }
        }
        public static string RaPosition
        {
            get; private set;
        }
        public static double RaRateAxis
        {
            private get => _rateAxes.X;
            set
            {
                _rateAxes.X = Conversions.ArcSec2Deg(value);
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
                var unused = new CmdRateAxis(0, AxisId.Axis1, _rateAxes.X);
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

        // General Info
        public static string MountName { get; private set; }
        public static string MountVersion { get; private set; }
        public static long[] StepsPerRevolution { get; set; }

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

                SetTracking();
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
                MountStart();
            }
            catch (Exception e)
            {
                MountStop();
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
        /// Makes sure the axes are at full stop
        /// </summary>
        /// <returns></returns>
        private static bool AxesStopValidate()
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed.TotalSeconds <= 5000)
            {
                SimTasks(MountTaskName.StopAxes);
                Thread.Sleep(100);
                var statusx = new CmdAxisStatus(MountQueue.NewId, AxisId.Axis1);
                var axis1Status = (AxisStatus)MountQueue.GetCommandResult(statusx).Result;
                var axis1Stopped = axis1Status.Stopped;

                var statusy = new CmdAxisStatus(MountQueue.NewId, AxisId.Axis2);
                var axis2Status = (AxisStatus)MountQueue.GetCommandResult(statusy).Result;
                var axis2Stopped = axis2Status.Stopped;

                if (!axis1Stopped || !axis2Stopped) continue;
                stopwatch.Stop();
                return true;
            }
            stopwatch.Stop();
            return false;
        }

        /// <summary>
        /// For initial connection to mount
        /// </summary>
        private static void ConnectMount()
        {
            var positions = GetDefaultPositions();
            object _ = new CmdAxisToDegrees(0, AxisId.Axis1, positions[0]);
            _ = new CmdAxisToDegrees(0, AxisId.Axis2, positions[1]);

            SimTasks(MountTaskName.MountName);
            SimTasks(MountTaskName.MountVersion);
            SimTasks(MountTaskName.StepsPerRevolution);
        }

        /// <summary>
        /// Slew state based on axis status
        /// </summary>
        private static void CheckSlewState()
        {
            var slewing = false;
            switch (SlewState)
            {
                case SlewType.SlewNone:
                    slewing = false;
                    break;
                case SlewType.SlewSettle:
                    slewing = true;
                    break;
                case SlewType.SlewMoveAxis:
                    slewing = true;
                    break;
                case SlewType.SlewRaDec:
                    slewing = true;
                    break;
                case SlewType.SlewAltAz:
                    slewing = true;
                    break;
                case SlewType.SlewPark:
                    slewing = true;
                    Tracking = false;
                    //AtPark = true;
                    break;
                case SlewType.SlewHome:
                    slewing = true;
                    Tracking = false;
                    break;
                case SlewType.SlewHandpad:
                    slewing = true;
                    break;
                case SlewType.SlewComplete:
                    SlewState = SlewType.SlewNone;
                    break;
                default:
                    SlewState = SlewType.SlewNone;
                    break;
            }
            if (Math.Abs(RaRateAxis + DecRateAxis) > 0) slewing = true;
            IsSlewing = slewing;
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
        private static double CurrentTrackingRate()
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

            rate = rate / 3600;
            return rate;
        }

        /// <summary>
        /// Used when the mount is first turned on and the instance is created
        /// </summary>
        private static double[] GetDefaultPositions()
        {
            double[] positions;
            // set default home position
            switch (Settings.AlignmentMode)
            {
                case AlignmentModes.algGermanPolar:
                    _homeAxes.X = 90;
                    _homeAxes.Y = 90;
                    break;
                case AlignmentModes.algPolar:
                    _homeAxes.X = -90;
                    _homeAxes.Y = 0;
                    break;
                case AlignmentModes.algAltAz:
                    break;
                default:
                    _homeAxes.X = 90;
                    _homeAxes.Y = 90;
                    break;
            }

            // get home override from the settings
            if (Math.Abs(Settings.HomeX) > 0 || Math.Abs(Settings.HomeY) > 0)
            {
                _homeAxes.X = Settings.HomeX;
                _homeAxes.Y = Settings.HomeY;
            }

            // Set to the internal mount
            //_mountAxes = new Vector(_homeAxes.X, _homeAxes.Y);

            if (Settings.AtPark)
            {
                // _mountAxes = new Vector(SkySettings.ParkAxisX, SkySettings.ParkAxisY);
                //set mount positions to match parked axes
                //MountTasks(MountTaskName.SyncAxes);
                if (Settings.AutoTrack)
                {
                    Settings.AtPark = false;
                    Tracking = Settings.AutoTrack;
                }
                positions = new[] { Settings.ParkX, Settings.ParkY };
            }
            else
            {
                positions = new[] { _homeAxes.X, _homeAxes.Y };
            }
            return positions;
        }

        /// <summary>
        /// Runs the Goto in async so not to block the driver or UI threads.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="slewState"></param>
        private static async void GoToAsync(double[] target, SlewType slewState)
        {
            if (IsSlewing)
            {
                SlewState = SlewType.SlewNone;
                var stopped = AxesStopValidate();
                if (!stopped)
                {
                    AbortSlew();
                    throw new Exception("Timeout stopping axes");
                }
            }
            SlewState = slewState;
            var startingState = slewState;
            var trackingState = Tracking;
            Tracking = false;

            var returncode = 0;
            returncode = await Task.Run(() => SimGoTo(target, trackingState));
            if (returncode == 0)
            {
                if (SlewState == SlewType.SlewNone) return;
                switch (startingState)
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
                        Settings.AtPark = true;
                        break;
                    case SlewType.SlewHome:
                        break;
                    case SlewType.SlewHandpad:
                        break;
                    case SlewType.SlewComplete:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                SlewState = SlewType.SlewComplete;
                Tracking = trackingState;
                return;
            }
            AbortSlew();
            throw new Exception($"GoTo Async Error: {returncode}");
        }

        /// <summary>
        /// Sim GOTO slew
        /// </summary>
        /// <returns></returns>
        private static int SimGoTo(double[] target, bool trackingState)
        {
            var returncode = 1;
            //  stop slew after 60 seconds
            const int timer = 60;
            var stopwatch = Stopwatch.StartNew();

            SimTasks(MountTaskName.StopAxes);
            var simTarget = Axes.SimToMount(target);

            #region First Slew
            // time could be off a bit may need to deal with each axis seperate
            object _ = new CmdAxisGoToTarget(0, AxisId.Axis1, simTarget[0]);
            _ = new CmdAxisGoToTarget(0, AxisId.Axis2, simTarget[1]);

            while (stopwatch.Elapsed.TotalSeconds <= timer)
            {
                Thread.Sleep(100);
                if (SlewState == SlewType.SlewNone) break;
                var statusx = new CmdAxisStatus(MountQueue.NewId, AxisId.Axis1);
                var axis1Status = (AxisStatus)MountQueue.GetCommandResult(statusx).Result;
                var axis1Stopped = axis1Status.Stopped;

                var statusy = new CmdAxisStatus(MountQueue.NewId, AxisId.Axis2);
                var axis2Status = (AxisStatus)MountQueue.GetCommandResult(statusy).Result;
                var axis2Stopped = axis2Status.Stopped;

                if (!axis1Stopped || !axis2Stopped) continue;
                if (SlewSettleTime > 0)
                {
                    // post-slew settling time
                    var sw = Stopwatch.StartNew();
                    while (sw.Elapsed.TotalSeconds < SlewSettleTime) { }
                    sw.Stop();
                }
                break;
            }

            AxesStopValidate();

            #endregion

            #region Repeat Slews for Ra

            if (trackingState)
            {
                //attempt precision moves to target
                var rate = CurrentTrackingRate();
                var deltaTime = stopwatch.Elapsed.TotalSeconds;
                var maxtries = 0;

                while (true)
                {
                    if (maxtries > 6) break;
                    maxtries++;
                    stopwatch.Reset();
                    stopwatch.Start();

                    //calculate new target position
                    var deltaDegree = rate * deltaTime;
                    if (deltaDegree < .001)
                    {
                        break;
                    }
                    target[0] += deltaDegree;
                    var deltaTarget = Axes.SimToMount(target);

                    //check for a stop
                    if (SlewState == SlewType.SlewNone) break;

                    //move to new target
                    _ = new CmdAxisGoToTarget(0, AxisId.Axis1, deltaTarget[0]);

                    // track movment until axis is stopped
                    var axis1stopped = false;
                    while (!axis1stopped)
                    {
                        Thread.Sleep(100);
                        if (SlewState == SlewType.SlewNone) break;
                        var deltax = new CmdAxisStatus(MountQueue.NewId, AxisId.Axis1);
                        var axis1Status = (AxisStatus)MountQueue.GetCommandResult(deltax).Result;
                        axis1stopped = axis1Status.Stopped;
                    }

                    //take the time and move again
                    deltaTime = stopwatch.Elapsed.TotalSeconds;
                }
                //make sure all axes are stopped
                SimTasks(MountTaskName.StopAxes);
            }
            stopwatch.Stop();

            //evertyhing seems okay to return
            returncode = 0;
            #endregion

            return returncode;
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
        /// Start connection, queues, and events
        /// </summary>
        private static void MountStart()
        {
            var interval = 0;
            MountQueue.Start();
            if (!MountQueue.IsRunning) throw new Exception("Failed to start simulator queue");

            ConnectMount();

            // start event to update UI
            interval = Settings.UIInterval;
            
            // Event to get mount data and update UI
            _mediatimer = new MediaTimer { Period = interval, Resolution = 5 };
            _mediatimer.Tick += UpdateServerEvent;
            _mediatimer.Start();
        }

        /// <summary>
        /// Stop queues and events
        /// </summary>
        private static void MountStop()
        {
            if (MountQueue.IsRunning)
            {
                _mediatimer.Tick -= UpdateServerEvent;
                _mediatimer?.Stop();
                _mediatimer?.Dispose();
                MountQueue.Stop();
            }
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
            dynamic _;
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

                _ = new CmdAxisPulse(0, AxisId.Axis1, raGuideRate, duration);
                PulseWait(duration, (int)AxisId.Axis1);
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
                _ = new CmdAxisPulse(0, AxisId.Axis2, decGuideRate, duration);
                PulseWait(duration, (int)AxisId.Axis2);
            }

        }

        /// <summary>
        /// Waits out the pulse duration time so it can report the pulse is finished.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="axis"></param>
        private static async void PulseWait(int duration, int axis)
        {
            await Task.Run(() =>
            {
                var sw = Stopwatch.StartNew();
                while (sw.Elapsed.TotalMilliseconds < duration)
                {
                    //do something while waiting
                }
                sw.Stop();
                switch (axis)
                {
                    case 0:
                        IsRaPulseGuiding = false;
                        break;
                    case 1:
                        IsDecPulseGuiding = false;
                        break;
                }
            });

        }

        /// <summary>
        /// Setup guide rates
        /// </summary>
        public static void SetGuideRates()
        {
            var rate = CurrentTrackingRate();
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
        /// Creates tasks that are put in the MountQueue
        /// </summary>
        /// <param name="taskname"></param>
        private static void SimTasks(MountTaskName taskname)
        {
            object _;
            switch (taskname)
            {
                case MountTaskName.AlternatingPpec:
                    break;
                case MountTaskName.DecPulseToGoTo:
                    break;
                case MountTaskName.Encoders:
                    break;
                case MountTaskName.GetOneStepIndicators:
                    break;
                case MountTaskName.LoadDefaults:
                    break;
                case MountTaskName.StopAxes:
                    _ = new CmdAxisStop(0, AxisId.Axis1);
                    _ = new CmdAxisStop(0, AxisId.Axis2);
                    break;
                case MountTaskName.InstantStopAxes:
                    break;
                case MountTaskName.SetSouthernHemisphere:
                    break;
                case MountTaskName.SyncAxes:
                    var sync = Axes.SimToMount(new[] { _mountAxes.X, _mountAxes.Y });
                    _ = new CmdAxisToDegrees(0, AxisId.Axis1, sync[0]);
                    _ = new CmdAxisToDegrees(0, AxisId.Axis2, sync[1]);
                    break;
                case MountTaskName.SyncTarget:
                    var xy = Axes.RaDecToAxesXY(new[] { RaTarget, DecTarget }, true);
                    var targ = Axes.SimToMount(new[] { xy[0], xy[1] });
                    _ = new CmdAxisToDegrees(0, AxisId.Axis1, targ[0]);
                    _ = new CmdAxisToDegrees(0, AxisId.Axis2, targ[1]);
                    break;
                case MountTaskName.SyncAltAz:
                    var yx = Axes.AltAzToAxesYX(new[] { _altAzSync.Y, _altAzSync.X });
                    var altaz = Axes.SimToMount(new[] { yx[1], yx[0] });
                    _ = new CmdAxisToDegrees(0, AxisId.Axis1, altaz[0]);
                    _ = new CmdAxisToDegrees(0, AxisId.Axis2, altaz[1]);
                    break;
                case MountTaskName.MonitorPulse:
                    break;
                case MountTaskName.Pec:
                    break;
                case MountTaskName.PecTraining:
                    break;
                case MountTaskName.Capabilities:
                    break;
                case MountTaskName.SetSt4Guiderate:
                    break;
                case MountTaskName.SkySetSnapPort:
                    break;
                case MountTaskName.MountName:
                    var mountName = new CmdMountName(MountQueue.NewId);
                    MountName = (string)MountQueue.GetCommandResult(mountName).Result;
                    break;
                case MountTaskName.GetAxisVersions:
                    break;
                case MountTaskName.GetAxisStrVersions:
                    break;
                case MountTaskName.MountVersion:
                    var mountVersion = new CmdMountVersion(MountQueue.NewId);
                    MountVersion = (string)MountQueue.GetCommandResult(mountVersion).Result;
                    break;
                case MountTaskName.StepsPerRevolution:
                    var spr = new CmdSpr(MountQueue.NewId);
                    var sprnum = (long)MountQueue.GetCommandResult(spr).Result;
                    StepsPerRevolution = new[] { sprnum, sprnum };
                    break;
                case MountTaskName.SetHomePositions:
                    _ = new CmdAxisToDegrees(0, AxisId.Axis1, _homeAxes.X);
                    _ = new CmdAxisToDegrees(0, AxisId.Axis2, _homeAxes.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(taskname), taskname, null);
            }

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
        private static void SetTracking()
        {
            double rateChange = 0;
            switch (_trackingMode)
            {
                case TrackingMode.Off:
                    break;
                case TrackingMode.AltAz:
                    rateChange = CurrentTrackingRate();
                    break;
                case TrackingMode.EqN:
                    rateChange = CurrentTrackingRate();
                    break;
                case TrackingMode.EqS:
                    rateChange = -CurrentTrackingRate();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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
            Settings.AtPark = false;
            //todo checklimit
            GoToAsync(new[] { targetx, targety }, slewState);
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
            _altAzSync = new Vector(azimuth, altitude);
            SimTasks(MountTaskName.SyncAltAz);
        }

        /// <summary>
        /// Sync pointing to Ra and Dec
        /// </summary>
        public static void SyncToTargetRaDec()
        {
            SimTasks(MountTaskName.SyncTarget);
        }

        /// <summary>
        /// Event to get mount positions and update display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UpdateServerEvent(object sender, EventArgs e)
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

                // Event interval time set for UI performance
                _mediatimer.Period = Settings.UIInterval;

                // Get positions
                var simPositions = new CmdAxesDegrees(MountQueue.NewId);
                var actualDegrees = (double[])MountQueue.GetCommandResult(simPositions).Result;
                var axes = Axes.MountToSim(actualDegrees);

                //local to just track positions
                _mountAxes.X = axes[0];
                _mountAxes.Y = axes[1];

                CheckAxisLimits();
                PierSide = SideOfPier;

                //update UI for degrees
                RaPosition = $"{actualDegrees[0]:0.000000}";
                DecPosition = $"{actualDegrees[1]:0.000000}";

                // convert to Ra and Dec
                var raDec = Axes.AxesXYToRaDec(axes);
                var raDecConverted = Coords.Topo2Coords(raDec[0], raDec[1]);

                RightAscension = raDec[0];
                Declination = raDec[1];
                RaConverted = raDecConverted[0];
                DecConverted = raDecConverted[1];

                // convert to Az and Alt
                var azAlt = Axes.AxesXYToAzAlt(axes);
                Altitude = azAlt[1];
                Azimuth = azAlt[0];
                //Azimuth = Coords.Topo2CoordAz(raDec[0], raDec[1]);

                // get axis positions in steps
                var steps = new CmdAxisSteps(MountQueue.NewId);
                var i = (int[])MountQueue.GetCommandResult(steps).Result;
                RaSteps = i[0];
                DecSteps = i[1];

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
