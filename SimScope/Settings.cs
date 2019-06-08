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
using System.Reflection;
using System.Runtime.CompilerServices;
using ASCOM.DeviceInterface;
using ASCOM.Utilities;
using SimServer.Domain;
using SimServer.Helpers;

namespace SimServer
{
    /// <summary>
    /// Persistant settings SimScope - ASCOM Telescope Control Simulator
    /// </summary>
    public static class Settings
    {
        #region Events

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        #endregion
        
        #region Capabilities Properties

        private static bool _canAlignMode;
        public static bool CanAlignMode
        {
            get => _canAlignMode;
            private set
            {
                if (_canAlignMode == value) return;
                _canAlignMode = value;
                Properties.Server.Default.CanAlignMode = value;
            }
        }
        private static bool _canAltAz;
        public static bool CanAltAz
        {
            get => _canAltAz;
            private set
            {
                if (_canAltAz == value) return;
                _canAltAz = value;
                Properties.Server.Default.CanAltAz = value;
            }
        }
        private static bool _canDoesRefraction;
        public static bool CanDoesRefraction
        {
            get => _canDoesRefraction;
            set
            {
                if (_canDoesRefraction == value) return;
                _canDoesRefraction = value;
                Properties.Server.Default.CanDoesRefraction = value;
                Coords.Update();
            }
        }
        private static bool _canDualAxisPulseGuide;
        public static bool CanDualAxisPulseGuide
        {
            get => _canDualAxisPulseGuide;
            private set
            {
                if (_canDualAxisPulseGuide == value) return;
                _canDualAxisPulseGuide = value;
                Properties.Server.Default.CanDualAxisPulseGuide = value;
            }
        }
        private static bool _canEquatorial;
        public static bool CanEquatorial
        {
            get => _canEquatorial;
            private set
            {
                if (_canEquatorial == value) return;
                _canEquatorial = value;
                Properties.Server.Default.CanEquatorial = value;
            }
        }
        private static bool _canFindHome;
        public static bool CanFindHome
        {
            get => _canFindHome;
            private set
            {
                if (_canFindHome == value) return;
                _canFindHome = value;
                Properties.Server.Default.CanFindHome = value;
            }
        }
        private static bool _canLatLongElev;
        public static bool CanLatLongElev
        {
            get => _canLatLongElev;
            private set
            {
                if (_canLatLongElev == value) return;
                _canLatLongElev = value;
                Properties.Server.Default.CanLatLongElev = value;
            }
        }
        private static bool _canOptics;
        public static bool CanOptics
        {
            get => _canOptics;
            private set
            {
                if (_canOptics == value) return;
                _canOptics = value;
                Properties.Server.Default.CanOptics = value;
            }
        }
        private static bool _canPark;
        public static bool CanPark
        {
            get => _canPark;
            private set
            {
                if (_canPark == value) return;
                _canPark = value;
                Properties.Server.Default.CanPark = value;
            }
        }
        private static bool _canPulseGuide;
        public static bool CanPulseGuide
        {
            get => _canPulseGuide;
            private set
            {
                if (_canPulseGuide == value) return;
                _canPulseGuide = value;
                Properties.Server.Default.CanPulseGuide = value;
            }
        }
        private static bool _canSetEquRates;
        public static bool CanSetEquRates
        {
            get => _canSetEquRates;
            private set
            {
                if (_canSetEquRates == value) return;
                _canSetEquRates = value;
                Properties.Server.Default.CanSetEquRates = value;
            }
        }
        private static bool _canSetDeclinationRate;
        public static bool CanSetDeclinationRate
        {
            get => _canSetDeclinationRate;
            private set
            {
                if (_canSetDeclinationRate == value) return;
                _canSetDeclinationRate = value;
                Properties.Server.Default.CanSetDeclinationRate = value;
            }
        }
        private static bool _canSetGuideRates;
        public static bool CanSetGuideRates
        {
            get => _canSetGuideRates;
            private set
            {
                if (_canSetGuideRates == value) return;
                _canSetGuideRates = value;
                Properties.Server.Default.CanSetGuideRates = value;
            }
        }
        private static bool _canSetPark;
        public static bool CanSetPark
        {
            get => _canSetPark;
            private set
            {
                if (_canSetPark == value) return;
                _canSetPark = value;
                Properties.Server.Default.CanSetPark = value;
            }
        }
        private static bool _canSetPierSide;
        public static bool CanSetPierSide
        {
            get => _canSetPierSide;
            private set
            {
                if (_canSetPierSide == value) return;
                _canSetPierSide = value;
                Properties.Server.Default.CanSetPierSide = value;
            }
        }
        private static bool _canSetRightAscensionRate;
        public static bool CanSetRightAscensionRate
        {
            get => _canSetRightAscensionRate;
            private set
            {
                if (_canSetRightAscensionRate == value) return;
                _canSetRightAscensionRate = value;
                Properties.Server.Default.CanSetRightAscensionRate = value;
            }
        }
        private static bool _canSetTracking;
        public static bool CanSetTracking
        {
            get => _canSetTracking;
            private set
            {
                if (_canSetTracking == value) return;
                _canSetTracking = value;
                Properties.Server.Default.CanSetTracking = value;
            }
        }
        private static bool _canSiderealTime;
        public static bool CanSiderealTime
        {
            get => _canSiderealTime;
            private set
            {
                if (_canSiderealTime == value) return;
                _canSiderealTime = value;
                Properties.Server.Default.CanSiderealTime = value;
            }
        }
        private static bool _canSlew;
        public static bool CanSlew
        {
            get => _canSlew;
            private set
            {
                if (_canSlew == value) return;
                _canSlew = value;
                Properties.Server.Default.CanSlew = value;
            }
        }
        private static bool _canSlewAltAz;
        public static bool CanSlewAltAz
        {
            get => _canSlewAltAz;
            private set
            {
                if (_canSlewAltAz == value) return;
                _canSlewAltAz = value;
                Properties.Server.Default.CanSlewAltAz = value;
            }
        }
        private static bool _canSlewAltAzAsync;
        public static bool CanSlewAltAzAsync
        {
            get => _canSlewAltAzAsync;
            private set
            {
                if (_canSlewAltAzAsync == value) return;
                _canSlewAltAzAsync = value;
                Properties.Server.Default.CanSlewAltAzAsync = value;
            }
        }
        private static bool _canSlewAsync;
        public static bool CanSlewAsync
        {
            get => _canSlewAsync;
            private set
            {
                if (_canSlewAsync == value) return;
                _canSlewAsync = value;
                Properties.Server.Default.CanSlewAsync = value;
            }
        }
        private static bool _canSync;
        public static bool CanSync
        {
            get => _canSync;
            private set
            {
                if (_canSync == value) return;
                _canSync = value;
                Properties.Server.Default.CanSync = value;
            }
        }
        private static bool _canSyncAltAz;
        public static bool CanSyncAltAz
        {
            get => _canSyncAltAz;
            private set
            {
                if (_canSyncAltAz == value) return;
                _canSyncAltAz = value;
                Properties.Server.Default.CanSyncAltAz = value;
            }
        }
        private static bool _canTrackingRates;
        public static bool CanTrackingRates
        {
            get => _canTrackingRates;
            private set
            {
                if (_canTrackingRates == value) return;
                _canTrackingRates = value;
                Properties.Server.Default.CanTrackingRates = value;
            }
        }
        private static bool _canUnpark;
        public static bool CanUnpark
        {
            get => _canUnpark;
            private set
            {
                if (_canUnpark == value) return;
                _canUnpark = value;
                Properties.Server.Default.CanUnpark = value;
            }
        }
        private static bool _noSyncPastMeridian;
        public static bool NoSyncPastMeridian
        {
            get => _noSyncPastMeridian;
            private set
            {
                if (_noSyncPastMeridian == value) return;
                _noSyncPastMeridian = value;
                Properties.Server.Default.NoSyncPastMeridian = value;
            }
        }
        private static int _telescopeAxes;
        public static int TelescopeAxes
        {
            get => _telescopeAxes;
            private set
            {
                if (_telescopeAxes == value) return;
                _telescopeAxes = value;
                Properties.Server.Default.TelescopeAxes = value;
            }
        }
        #endregion

        #region Scope Properties

        private static double _apertureArea;
        public static double ApertureArea
        {
            get => _apertureArea;
            set
            {
                if (Math.Abs(_apertureArea - value) < 0.00000001) return;
                _apertureArea = value;
                Properties.Server.Default.ApertureArea = value;
                OnStaticPropertyChanged();
            }
        }
        private static double _apertureDiameter;
        public static double ApertureDiameter
        {
            get => _apertureDiameter;
            set
            {
                if (Math.Abs(_apertureDiameter - value) < 0.00000001) return;
                _apertureDiameter = value;
                Properties.Server.Default.ApertureDiameter = value;
            }
        }
        private static AlignmentModes _alignmentMode;
        public static AlignmentModes AlignmentMode
        {
            get => _alignmentMode;
            set
            {
                if (_alignmentMode == value) return;
                _alignmentMode = value;
                Properties.Server.Default.AlignmentMode = value.ToString();
            }
        }
        private static bool _ascomOn;
        public static bool AscomOn
        {
            get => _ascomOn;
            set
            {
                if (_ascomOn == value) return;
                _ascomOn = value;
                Properties.Server.Default.AscomOn = value;
            }
        }
        private static bool _atPark;
        public static bool AtPark
        {
            get => _atPark;
            set
            {
                if (_atPark == value) return;
                _atPark = value;
                Properties.Server.Default.AtPark = value;
                OnStaticPropertyChanged();
            }
        }
        private static bool _autoTrack;
        public static bool AutoTrack
        {
            get => _autoTrack;
            set
            {
                if (_autoTrack == value) return;
                _autoTrack = value;
                Properties.Server.Default.AutoTrack = value;
            }
        }
        private static DriveRates _trackingRate;
        public static DriveRates TrackingRate
        {
            get => _trackingRate;
            set
            {
                if (TrackingRate == value) return;
                _trackingRate = value;
                Properties.Server.Default.TrackingRate = value.ToString();
                OnStaticPropertyChanged();
            }
        }
        private static EquatorialCoordinateType _equatorialCoordinateType;
        public static EquatorialCoordinateType EquatorialCoordinateType
        {
            get => _equatorialCoordinateType;
            set
            {
                if (_equatorialCoordinateType == value) return;
                _equatorialCoordinateType = value;
                Properties.Server.Default.EquatorialCoordinateType = value.ToString();
                OnStaticPropertyChanged();
            }
        }
        private static double _focalLength;
        public static double FocalLength
        {
            get => _focalLength;
            set
            {
                if (Math.Abs(_focalLength - value) < 0.00000001) return;
                _focalLength = value;
                Properties.Server.Default.FocalLength = value;
                OnStaticPropertyChanged();
            }
        }
        private static double _elevation;
        public static double Elevation
        {
            get => _elevation;
            set
            {
                if (Math.Abs(_elevation - value) < 0.00000001) return;
                _elevation = value;
                Properties.Server.Default.Elevation = value;
                Coords.Update();
                OnStaticPropertyChanged();
            }
        }
        private static double _guideRateXPer;
        public static double GuideRateXPer
        {
            get => _guideRateXPer;
            set
            {
                if (Math.Abs(_guideRateXPer - value) < 0.00000001) return;
                _guideRateXPer = value;
                Properties.Server.Default.GuideRateXPer = value;
                Scope.SetGuideRates();
                OnStaticPropertyChanged();
            }
        }
        private static double _guideRateYPer;
        public static double GuideRateYPer
        {
            get => _guideRateYPer;
            set
            {
                if (Math.Abs(_guideRateYPer - value) < 0.00000001) return;
                _guideRateYPer = value;
                Properties.Server.Default.GuideRateYPer = value;
                Scope.SetGuideRates();
                OnStaticPropertyChanged();
            }
        }
        private static string _instrumentName;
        public static string InstrumentName
        {
            get => _instrumentName;
            set
            {
                if (_instrumentName == value) return;
                _instrumentName = value;
                Properties.Server.Default.InstrumentName = value;
            }
        }
        private static string _instrumentDescription;
        public static string InstrumentDescription
        {
            get => _instrumentDescription;
            set
            {
                if (_instrumentDescription == value) return;
                _instrumentDescription = value;
                Properties.Server.Default.InstrumentDescription = value;
            }
        }
        private static double _latitude;
        public static double Latitude
        {
            get => _latitude;
            set
            {
                if (Math.Abs(_latitude - value) < 0.00000001) return;
                _latitude = value;
                Scope.SouthernHemisphere = value < 0;
                Properties.Server.Default.Latitude = value;
                Coords.Update();
                OnStaticPropertyChanged();
            }
        }
        private static double _longitude;
        public static double Longitude
        {
            get => _longitude;
            set
            {
                if (Math.Abs(_longitude - value) < 0.00000001) return;
                _longitude = value;
                Properties.Server.Default.Longitude = value;
                Coords.Update();
                OnStaticPropertyChanged();
            }
        }
        private static double _maximumSlewRate;
        public static double MaximumSlewRate
        {
            get => _maximumSlewRate;
            set
            {
                if (Math.Abs(_maximumSlewRate - value) < 0.00000001) return;
                _maximumSlewRate = value;
                Properties.Server.Default.MaximumSlewRate = value;
                OnStaticPropertyChanged();
            }
        }
        private static int _utcDateOffset;
        public static int UTCDateOffset
        {
            get => _utcDateOffset;
            set
            {
                if (_utcDateOffset == value) return;
                _utcDateOffset = value;
                Properties.Server.Default.UTCDateOffset = value;
            }
        }
        private static bool _versionOne;
        public static bool VersionOne
        {
            get => _versionOne;
            set
            {
                if (_versionOne == value) return;
                _versionOne = value;
                Properties.Server.Default.VersionOne = value;
            }
        }
        private static double _hourAngleLimit;
        public static double HourAngleLimit
        {
            get => _hourAngleLimit;
            set
            {
                if (Math.Abs(_hourAngleLimit - value) < 0.00000001) return;
                _hourAngleLimit = value;
                Properties.Server.Default.HourAngleLimit = value;
                OnStaticPropertyChanged();
            }
        }
        private static double _homeX;
        public static double HomeX
        {
            get => _homeX;
            private set
            {
                if (Math.Abs(_homeX - value) < 0.00000001) return;
                _homeX = value;
                Properties.Server.Default.HomeX = value;
            }
        }
        private static double _homeY;
        public static double HomeY
        {
            get => _homeY;
            private set
            {
                if (Math.Abs(_homeY - value) < 0.00000001) return;
                _homeY = value;
                Properties.Server.Default.HomeY = value;
            }
        }
        private static double _parkX;
        public static double ParkX
        {
            get => _parkX;
            set
            {
                if (Math.Abs(_parkX - value) < 0.00000001) return;
                _parkX = value;
                Properties.Server.Default.ParkX = value;
            }
        }
        private static double _parkY;
        public static double ParkY
        {
            get => _parkY;
            set
            {
                if (Math.Abs(_parkY - value) < 0.00000001) return;
                _parkY = value;
                Properties.Server.Default.ParkY = value;
            }
        }
        private static double _lunarRate;
        public static double LunarRate
        {
            get => _lunarRate;
            set
            {
                if (Math.Abs(_lunarRate - value) < 0.00000001) return;
                _lunarRate = value;
                Properties.Server.Default.LunarRate = value;
                OnStaticPropertyChanged();
            }
        }
        private static double _kingRate;
        public static double KingRate
        {
            get => _kingRate;
            set
            {
                if (Math.Abs(_kingRate - value) < 0.00000001) return;
                _kingRate = value;
                Properties.Server.Default.KingRate = value;
                OnStaticPropertyChanged();
            }
        }
        private static double _siderealRate;
        public static double SiderealRate
        {
            get => _siderealRate;
            set
            {
                if (Math.Abs(_siderealRate - value) < 0.00000001) return;
                _siderealRate = value;
                Properties.Server.Default.SiderealRate = value;
                OnStaticPropertyChanged();
            }
        }
        private static double _solarRate;
        public static double SolarRate
        {
            get => _solarRate;
            set
            {
                if (Math.Abs(_solarRate - value) < 0.00000001) return;
                _solarRate = value;
                Properties.Server.Default.SolarRate = value;
                OnStaticPropertyChanged();
            }
        }
        private static double _temperature;
        public static double Temperature
        {
            get => _temperature;
            set
            {
                if (Math.Abs(_temperature - value) < 0.00000001) return;
                _temperature = value;
                Properties.Server.Default.Temperature = value;
                Coords.Update();
                OnStaticPropertyChanged();
            }
        }
        private static int _uIInterval;
        public static int UIInterval
        {
            get => _uIInterval;
            private set
            {
                if (_uIInterval == value) return;
                _uIInterval = value;
                Properties.Server.Default.UIInterval = value;
                OnStaticPropertyChanged();
            }
        }
        private static SlewSpeed _hcSpeed;
        public static SlewSpeed HcSpeed
        {
            get => _hcSpeed;
            set
            {
                if (_hcSpeed == value) return;
                _hcSpeed = value;
                Properties.Server.Default.HcSpeed = Convert.ToInt32(value);
            }
        }

        #endregion

        #region Serial Properties
       
        private static bool _dtrEnable;
        public static bool DTREnable
        {
            get => _dtrEnable;
            set
            {
                if (_dtrEnable == value) return;
                _dtrEnable = value;
                Properties.Server.Default.DtrEnable = value;
                OnStaticPropertyChanged();
            }
        }
        private static bool _rTSEnable;
        public static bool RTSEnable
        {
            get => _rTSEnable;
            set
            {
                if (_rTSEnable == value) return;
                _rTSEnable = value;
                Properties.Server.Default.RTSEnable = value;
                OnStaticPropertyChanged();
            }
        }
        private static int _dataBit;
        public static int DataBit
        {
            get => _dataBit;
            set
            {
                if (_dataBit == value) return;
                _dataBit = value;
                Properties.Server.Default.DataBit = value;
                OnStaticPropertyChanged();
            }
        }
        private static string _comPort;
        public static string ComPort
        {
            get => _comPort;
            set
            {
                if (_comPort == value) return;
                _comPort = value;
                Properties.Server.Default.ComPort = value;
                OnStaticPropertyChanged();
            }
        }
        private static SerialSpeed _baudRate;
        public static SerialSpeed BaudRate
        {
            get => _baudRate;
            set
            {
                if (_baudRate == value) return;
                _baudRate = value;
                Properties.Server.Default.BaudRate = $"{value}";
                OnStaticPropertyChanged();
            }
        }
        private static SerialHandshake _handShake;
        public static SerialHandshake HandShake
        {
            get => _handShake;
            set
            {
                if (_handShake == value) return;
                _handShake = value;
                Properties.Server.Default.HandShake = $"{value}";
                OnStaticPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public static void Load()
        {
            Upgrade();
            
            CanAlignMode = Properties.Server.Default.CanAlignMode;
            CanAltAz = Properties.Server.Default.CanAltAz;
            CanDoesRefraction = Properties.Server.Default.CanDoesRefraction;
            CanDualAxisPulseGuide = Properties.Server.Default.CanDualAxisPulseGuide;
            CanEquatorial = Properties.Server.Default.CanEquatorial;
            CanFindHome = Properties.Server.Default.CanFindHome;
            CanLatLongElev = Properties.Server.Default.CanLatLongElev;
            CanOptics = Properties.Server.Default.CanOptics;
            CanPark = Properties.Server.Default.CanPark;
            CanPulseGuide = Properties.Server.Default.CanPulseGuide;
            CanSetEquRates = Properties.Server.Default.CanSetEquRates;
            CanSetDeclinationRate = Properties.Server.Default.CanSetDeclinationRate;
            CanSetGuideRates = Properties.Server.Default.CanSetGuideRates;
            CanSetPark = Properties.Server.Default.CanSetPark;
            CanSetPierSide = Properties.Server.Default.CanSetPierSide;
            CanSetRightAscensionRate = Properties.Server.Default.CanSetRightAscensionRate;
            CanSetTracking = Properties.Server.Default.CanSetTracking;
            CanSiderealTime = Properties.Server.Default.CanSiderealTime;
            CanSlew = Properties.Server.Default.CanSlew;
            CanSlewAltAzAsync = Properties.Server.Default.CanSlewAltAzAsync;
            CanSlewAltAz = Properties.Server.Default.CanSlewAltAz;
            CanSlewAsync = Properties.Server.Default.CanSlewAsync;
            CanSync = Properties.Server.Default.CanSync;
            CanSyncAltAz = Properties.Server.Default.CanSyncAltAz;
            CanTrackingRates = Properties.Server.Default.CanTrackingRates;
            CanUnpark = Properties.Server.Default.CanUnpark;
            NoSyncPastMeridian = Properties.Server.Default.NoSyncPastMeridian;
            TelescopeAxes = Properties.Server.Default.TelescopeAxes;

            Enum.TryParse<EquatorialCoordinateType>(Properties.Server.Default.EquatorialCoordinateType, true, out var eparse);
            EquatorialCoordinateType = eparse;
            Enum.TryParse<AlignmentModes>(Properties.Server.Default.AlignmentMode, true, out var aparse);
            AlignmentMode = aparse;
            Enum.TryParse<DriveRates>(Properties.Server.Default.TrackingRate, true, out var dparse);
            TrackingRate = dparse;

            AscomOn = Properties.Server.Default.AscomOn;
            AtPark = Properties.Server.Default.AtPark;
            ApertureArea = Properties.Server.Default.ApertureArea;
            ApertureDiameter = Properties.Server.Default.ApertureDiameter;
            FocalLength = Properties.Server.Default.FocalLength;
            Elevation = Properties.Server.Default.Elevation;
            InstrumentName = Properties.Server.Default.InstrumentName;
            InstrumentDescription = Properties.Server.Default.InstrumentDescription;
            GuideRateXPer = Properties.Server.Default.GuideRateXPer;
            GuideRateYPer = Properties.Server.Default.GuideRateYPer;
            Latitude = Properties.Server.Default.Latitude;
            Longitude = Properties.Server.Default.Longitude;
            MaximumSlewRate = Properties.Server.Default.MaximumSlewRate;
            HomeX = Properties.Server.Default.HomeX;
            HomeY = Properties.Server.Default.HomeY;
            ParkX = Properties.Server.Default.ParkX;
            ParkY = Properties.Server.Default.ParkY;
            UTCDateOffset = Properties.Server.Default.UTCDateOffset;
            VersionOne = Properties.Server.Default.VersionOne;
            HourAngleLimit = Properties.Server.Default.HourAngleLimit;
            LunarRate = Properties.Server.Default.LunarRate;
            KingRate = Properties.Server.Default.KingRate;
            SiderealRate = Properties.Server.Default.SiderealRate;
            SolarRate = Properties.Server.Default.SolarRate;
            Temperature = Properties.Server.Default.Temperature;

            //serial
            Enum.TryParse<SerialSpeed>(Properties.Server.Default.BaudRate, true, out var bparse);
            BaudRate = bparse;
            Enum.TryParse<SerialHandshake>(Properties.Server.Default.HandShake, true, out var hparse);
            HandShake = hparse;

            DTREnable = Properties.Server.Default.DtrEnable;
            RTSEnable = Properties.Server.Default.RTSEnable;
            DataBit = Properties.Server.Default.DataBit;
            ComPort = Properties.Server.Default.ComPort;

            UIInterval = Properties.Server.Default.UIInterval;
            Enum.TryParse<SlewSpeed>(Properties.Server.Default.HcSpeed.ToString(), true, out var sparse);
            HcSpeed = sparse;
        }

        private static void Upgrade()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName().Version;
            var version = Properties.Server.Default.Version;
            if (version == assembly.ToString()) return;
            Properties.Server.Default.Upgrade();
            Properties.Server.Default.Version = assembly.ToString();
            Save();
        }

        public static void Save()
        {
            Properties.Server.Default.Save();
            Properties.Server.Default.Reload();
        }

        private static void OnStaticPropertyChanged([CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion
    }
}
