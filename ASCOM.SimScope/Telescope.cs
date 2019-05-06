using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using ASCOM.DeviceInterface;
using ASCOM.Utilities;
using SimServer;
using SimServer.Domain;
using SimServer.Helpers;


namespace ASCOM.SimScope.Telescope
{
    [Guid("D0835F55-D7FD-4BB8-84F5-A33335A37003")]
    [ServedClassName("ASCOM SimScope Telescope")]
    [ProgId("ASCOM.SimScope.Telescope")]
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class Telescope : ObjectBase, ITelescopeV3, IDisposable
    {
        private AxisRates[] _mAxisRates;
        private TrackingRates _mTrackingRates;
        private TrackingRatesSimple _mTrackingRatesSimple;
        private Util _util;
        private readonly long _objectId;

        public Telescope()
        {
            _mAxisRates = new AxisRates[3];
            _mAxisRates[0] = new AxisRates(TelescopeAxes.axisPrimary);
            _mAxisRates[1] = new AxisRates(TelescopeAxes.axisSecondary);
            _mAxisRates[2] = new AxisRates(TelescopeAxes.axisTertiary);
            _mTrackingRates = new TrackingRates();
            _mTrackingRatesSimple = new TrackingRatesSimple();
            _util = new Util();
            // get a unique instance id
            _objectId = Connection.GetConnId();
        }

        #region Public com Interface ITelescope Implementaion

        public string Action(string ActionName, string ActionParameters)
        {
            throw new MethodNotImplementedException("Action");
        }

        public ArrayList SupportedActions
        {
            // no supported actions, return empty array
            get
            {
                var sa = new ArrayList();
                return sa;
            }
        }

        public void AbortSlew()
        {
            CheckParked("AbortSlew");
            Scope.AbortSlew();
        }

        public AlignmentModes AlignmentMode
        {
            get
            {
                CheckCapability(Settings.CanAlignMode, "AlignmentMode");
                var r = Settings.AlignmentMode;

                switch (r)
                {
                    case AlignmentModes.algAltAz:
                        return AlignmentModes.algAltAz;
                    case AlignmentModes.algGermanPolar:
                        return AlignmentModes.algGermanPolar;
                    case AlignmentModes.algPolar:
                        return AlignmentModes.algPolar;
                    default:
                        return AlignmentModes.algGermanPolar;
                }
            }
        }

        public double Altitude
        {
            get
            {
                CheckCapability(Settings.CanAltAz, "Altitude", false);
                var r = Scope.Altitude;
                return r;
            }
        }

        public double ApertureArea
        {
            get
            {
                CheckCapability(Settings.CanOptics, "ApertureArea", false);
                var r = Settings.ApertureArea;
                return r;
            }
        }

        public double ApertureDiameter
        {
            get
            {
                CheckCapability(Settings.CanOptics, "ApertureDiameter", false);
                var r = Settings.ApertureDiameter;
                return r;
            }
        }

        public bool AtHome
        {
            get
            {
                CheckVersionOne("AtHome", false);
                var r = Scope.AtHome;
                return r;
            }
        }

        public bool AtPark
        {
            get
            {
                CheckVersionOne("AtPark", false);
                var r = Settings.AtPark;
                return r;
            }
        }

        public IAxisRates AxisRates(TelescopeAxes Axis)
        {
            switch (Axis)
            {
                case TelescopeAxes.axisPrimary:
                    return new AxisRates(TelescopeAxes.axisPrimary);
                case TelescopeAxes.axisSecondary:
                    return new AxisRates(TelescopeAxes.axisSecondary);
                case TelescopeAxes.axisTertiary:
                    return new AxisRates(TelescopeAxes.axisTertiary);
                default:
                    return null;
            }
        }

        public double Azimuth
        {
            get
            {
                CheckCapability(Settings.CanAltAz, "Azimuth", false);
                var r = Scope.Azimuth;
                return r;
            }
        }

        public bool CanFindHome
        {
            get
            {
                var r = Settings.CanFindHome;
                return r;
            }
        }

        public bool CanMoveAxis(TelescopeAxes Axis)
        {
            CheckVersionOne("CanMoveAxis");
            var a = Settings.TelescopeAxes;
            switch (Axis)
            {
                case TelescopeAxes.axisPrimary:
                    return a >= 1;
                case TelescopeAxes.axisSecondary:
                    return a >= 2;
                case TelescopeAxes.axisTertiary:
                    return a >= 3;
                default:
                    return false;
            }
        }

        public bool CanPark
        {
            get
            {
                var r = Settings.CanPark;
                return r;
            }
        }

        public bool CanPulseGuide
        {
            get
            {
                var r = Settings.CanPulseGuide;
                return r;
            }
        }

        public bool CanSetDeclinationRate
        {
            get
            {
                CheckVersionOne("CanSetDeclinationRate", false);
                var r = Settings.CanSetDeclinationRate;
                return r;
            }
        }

        public bool CanSetGuideRates
        {
            get
            {
                var r = Settings.CanSetGuideRates;
                return r;
            }
        }

        public bool CanSetPark
        {
            get
            {
                var r = Settings.CanSetPark;
                return r;
            }
        }

        public bool CanSetPierSide
        {
            get
            {
                CheckVersionOne("CanSetPierSide", false);
                var r = Settings.CanSetPierSide;
                return r;
            }
        }

        public bool CanSetRightAscensionRate
        {
            get
            {
                CheckVersionOne("CanSetRightAscensionRate", false);
                var r = Settings.CanSetRightAscensionRate;
                return r;
            }
        }

        public bool CanSetTracking
        {
            get
            {
                var r = Settings.CanSetTracking;
                return r;
            }
        }

        public bool CanSlew
        {
            get
            {
                var r = Settings.CanSlew;
                return r;
            }
        }

        public bool CanSlewAltAz
        {
            get
            {
                CheckVersionOne("CanSlewAltAz", false);
                var r = Settings.CanSlewAltAz;
                return r;
            }
        }

        public bool CanSlewAltAzAsync
        {
            get
            {
                CheckVersionOne("CanSlewAltAzAsync", false);
                var r = Settings.CanSlewAltAzAsync;
                return r;
            }
        }

        public bool CanSlewAsync
        {
            get
            {
                var r = Settings.CanSlewAsync;
                return r;
            }
        }

        public bool CanSync
        {
            get
            {
                var r = Settings.CanSync;
                return r;
            }
        }

        public bool CanSyncAltAz
        {
            get
            {
                var r = Settings.CanSyncAltAz;
                return r;
            }
        }

        public bool CanUnpark
        {
            get
            {
                var r = Settings.CanUnpark;
                return r;
            }
        }

        public void CommandBlind(string Command, bool Raw)
        {
            throw new MethodNotImplementedException("CommandBlind");
        }

        public bool CommandBool(string Command, bool Raw)
        {
            throw new MethodNotImplementedException("CommandBool");
        }

        public string CommandString(string Command, bool Raw)
        {
            throw new MethodNotImplementedException("CommandString");
        }

        public bool Connected
        {
            get
            {
                var r = Connection.Connected;
                return r;
            }
            set => Connection.SetConnected(_objectId, value);
        }

        public double Declination
        {
            get
            {
                CheckCapability(Settings.CanEquatorial, "Declination", false);
                var dec = Scope.DecConverted;
                return dec;
            }
        }

        public double DeclinationRate
        {
            get
            {
                var r = Scope.DecRate;
                return r;
            }
            set => Scope.DecRate = value;
        }

        public string Description
        {
            get
            {
                var r = Settings.InstrumentDescription;
                return r;
            }
        }

        public PierSide DestinationSideOfPier(double RightAscension, double Declination)
        {
            CheckVersionOne("DestinationSideOfPier");
            var radec = Coords.RaDec2Topo(RightAscension, Declination);
            var r = Scope.SideOfPierRaDec(radec[0]);
            return r;
        }

        public bool DoesRefraction
        {
            get
            {
                var r = Settings.CanDoesRefraction;
                CheckVersionOne("DoesRefraction", false);
                return r;
            }
            set
            {
                CheckVersionOne("DoesRefraction", true);
                Settings.CanDoesRefraction = value;
            }
        }

        public string DriverInfo
        {
            get
            {
                var asm = Assembly.GetExecutingAssembly();
                var r = asm.FullName;
                return r;
            }
        }

        public string DriverVersion
        {
            get
            {
                CheckVersionOne("DriverVersion", false);
                var asm = Assembly.GetExecutingAssembly();
                var r = asm.GetName().Version.ToString();
                return r;
            }
        }

        public EquatorialCoordinateType EquatorialSystem
        {
            get
            {
                CheckVersionOne("EquatorialSystem", false);
                return Settings.EquatorialCoordinateType;
            }
        }

        public void FindHome()
        {
            if (!Settings.AscomOn) return;
            CheckCapability(Settings.CanFindHome, "FindHome");
            CheckParked("FindHome");
            Scope.GoToHome();
            while (Scope.SlewState == SlewType.SlewHome || Scope.SlewState == SlewType.SlewSettle)
            {
                // Application.DoEvents();
                DoEvents();
            }
        }

        public double FocalLength
        {
            get
            {
                CheckVersionOne("FocalLength", false);
                CheckCapability(Settings.CanOptics, "FocalLength", false);
                var r = Settings.FocalLength;
                return r;
            }
        }

        public double GuideRateDeclination
        {
            get
            {
                CheckVersionOne("GuideRateDeclination", false);
                var r = Scope.DecGuideRate;
                return r;
            }
            set
            {
                CheckVersionOne("GuideRateDeclination", true);
                Scope.DecGuideRate = value;
            }
        }

        public double GuideRateRightAscension
        {
            get
            {
                CheckVersionOne("GuideRateRightAscension", false);
                var r = Scope.RaGuideRate;
                return r;
            }
            set
            {
                CheckVersionOne("GuideRateRightAscension", true);
                Scope.RaGuideRate = value;
            }
        }

        public short InterfaceVersion
        {
            get
            {
                CheckVersionOne("InterfaceVersion", false);
                return 3;
            }
        }

        public bool IsPulseGuiding
        {
            get
            {
                CheckCapability(Settings.CanPulseGuide, "IsPulseGuiding", false);
                var r = Scope.IsPulseGuiding;
                return r;
            }
        }

        public void MoveAxis(TelescopeAxes Axis, double Rate)
        {
            if (!Settings.AscomOn) return;

            CheckVersionOne("MoveAxis");
            CheckRate(Axis, Rate);
            if (!CanMoveAxis(Axis))
                throw new MethodNotImplementedException("CanMoveAxis " + Enum.GetName(typeof(TelescopeAxes), Axis));
            CheckParked("MoveAxis");

            switch (Axis)
            {
                case TelescopeAxes.axisPrimary:
                    Scope.RaRateAxis = Rate;
                    break;
                case TelescopeAxes.axisSecondary:
                    Scope.DecRateAxis = Rate;
                    break;
                case TelescopeAxes.axisTertiary:
                    // not implemented
                    break;
            }
        }

        public string Name
        {
            get
            {
                var r = Settings.InstrumentName;
                return r;
            }
        }

        public void Park()
        {
            if (!Settings.AscomOn) return;
            CheckCapability(Settings.CanPark, "Park");
            if (Settings.AtPark)
            {
                return;
            }
            Scope.GoToPark();
            while (Scope.SlewState == SlewType.SlewPark)
            {
                //Application.DoEvents();
                DoEvents();
            }

        }

        public void PulseGuide(GuideDirections Direction, int Duration)
        {
            if (!Settings.AscomOn) return;
            if (Settings.AtPark) throw new ParkedException();
            CheckCapability(Settings.CanPulseGuide, "PulseGuide");
            CheckRange(Duration, 0, 30000, "PulseGuide", "Duration");
            Scope.PulseGuide(Direction, Duration);
            if (!Settings.CanDualAxisPulseGuide)Thread.Sleep(Duration); // Must be synchronous so wait out the pulseguide duration here
        }

        /// <inheritdoc />
        /// <summary>
        /// The right ascension (hours) of the telescope's current equatorial coordinates,
        /// in the coordinate system given by the EquatorialSystem property 
        /// </summary>
        public double RightAscension
        {
            get
            {
                CheckCapability(Settings.CanEquatorial, "RightAscension", false);
                var ra = Scope.RaConverted;
                return ra;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// This property, together with DeclinationRate, provides support for "offset tracking".
        /// Offset tracking is used primarily for tracking objects that move relatively slowly against
        /// the equatorial coordinate system. It also may be used by a software guiding system that
        /// controls rates instead of using the PulseGuide method. 
        /// </summary>
        public double RightAscensionRate
        {
            get
            {
                var r = Scope.RaRate;
                return r;
            }
            set
            {
                CheckCapability(Settings.CanSetEquRates, "RightAscensionRate", true);
                Scope.RaRate = value;
            }
        }

        public void SetPark()
        {
            CheckCapability(Settings.CanSetPark, "SetPark");
            Scope.SetParkAxes();
        }

        public void SetupDialog()
        {
            var alreadyopen = Scope.OpenSetupDialog;
            if (alreadyopen) return;
            Scope.OpenSetupDialog = true;
            while (true)
            {
                Thread.Sleep(100);
                if (Scope.OpenSetupDialogFinished) break;
            }

        }

        public PierSide SideOfPier
        {
            get
            {
                var r = Scope.SideOfPier;
                return r;
            }
            set
            {
                CheckCapability(Settings.CanSetPierSide, "SideOfPier", true);
                if (value == Scope.SideOfPier)
                {
                    return;
                }
                Scope.SideOfPier = value;
            }
        }

        public double SiderealTime
        {
            get
            {
                CheckCapability(Settings.CanSiderealTime, "SiderealTime", false);
                var r = Scope.SiderealTime;
                return r;
            }
        }

        public double SiteElevation
        {
            get
            {
                CheckCapability(Settings.CanLatLongElev, "SiteElevation", false);
                var r = Settings.Elevation;
                return r;
            }
            set
            {
                CheckCapability(Settings.CanLatLongElev, "SiteElevation", true);
                CheckRange(value, -300, 10000, "SiteElevation");
                Settings.Elevation = value;
            }
        }

        public double SiteLatitude
        {
            get
            {
                CheckCapability(Settings.CanLatLongElev, "SiteLatitude", false);
                var r = Settings.Latitude;
                return r;
            }
            set
            {
                CheckCapability(Settings.CanLatLongElev, "SiteLatitude", true);
                CheckRange(value, -90, 90, "SiteLatitude");
                Settings.Latitude = value;
            }
        }

        public double SiteLongitude
        {
            get
            {
                CheckCapability(Settings.CanLatLongElev, "SiteLongitude", false);
                var r = Settings.Longitude;
                return r;
            }
            set
            {
                CheckCapability(Settings.CanLatLongElev, "SiteLongitude", true);
                CheckRange(value, -180, 180, "SiteLongitude");
                Settings.Longitude = value;
            }
        }

        public short SlewSettleTime
        {
            get
            {
                var r = Scope.SlewSettleTime;
                return r;
            }
            set
            {
                CheckRange(value, 0, 100, "SlewSettleTime");
                var r = value;
                Scope.SlewSettleTime = r;
            }
        }

        public void SlewToAltAz(double Azimuth, double Altitude)
        {
            if (!Settings.AscomOn) return;
            CheckCapability(Settings.CanSlewAltAz, "SlewToAltAz");
            CheckParked("SlewToAltAz");
            CheckTracking(false, "SlewToAltAz");
            CheckRange(Azimuth, 0, 360, "SlewToltAz", "azimuth");
            CheckRange(Altitude, -90, 90, "SlewToAltAz", "Altitude");
            Scope.SlewAltAz(Altitude, Azimuth);
            while (Scope.SlewState == SlewType.SlewAltAz || Scope.SlewState == SlewType.SlewSettle)
            {
                //Application.DoEvents();
                DoEvents();
            }
        }

        public void SlewToAltAzAsync(double Azimuth, double Altitude)
        {
            if (!Settings.AscomOn) return;
            CheckCapability(Settings.CanSlewAltAzAsync, "SlewToAltAzAsync");
            CheckParked("SlewToAltAz");
            CheckTracking(false, "SlewToAltAzAsync");
            CheckRange(Azimuth, 0, 360, "SlewToAltAzAsync", "Azimuth");
            CheckRange(Altitude, -90, 90, "SlewToAltAzAsync", "Altitude");
            Scope.SlewAltAz(Altitude, Azimuth);
        }

        public void SlewToCoordinates(double RightAscension, double Declination)
        {
            if (!Settings.AscomOn) return;

            CheckCapability(Settings.CanSlew, "SlewToCoordinates");
            CheckRange(RightAscension, 0, 24, "SlewToCoordinates", "RightAscension");
            CheckRange(Declination, -90, 90, "SlewToCoordinates", "Declination");
            CheckParked("SlewToCoordinates");
            CheckTracking(true, "SlewToCoordinates");
            var radec = Coords.RaDec2Topo(RightAscension, Declination);
            Scope.DecTarget = radec[1];
            Scope.RaTarget = radec[0];
            Scope.SlewRaDec(radec[0], radec[1]);
            while (Scope.IsSlewing)
            {
                DoEvents();
            }
        }

        public void SlewToCoordinatesAsync(double RightAscension, double Declination)
        {
            if (!Settings.AscomOn) return;

            CheckCapability(Settings.CanSlewAsync, "SlewToCoordinatesAsync");
            CheckRange(RightAscension, 0, 24, "SlewToCoordinatesAsync", "RightAscension");
            CheckRange(Declination, -90, 90, "SlewToCoordinatesAsync", "Declination");
            CheckParked("SlewToCoordinatesAsync");
            CheckTracking(true, "SlewToCoordinatesAsync");
            var radec = Coords.RaDec2Topo(RightAscension, Declination);
            Scope.DecTarget = radec[1];
            Scope.RaTarget = radec[0];
            Scope.SlewRaDec(radec[0], radec[1]);
        }

        public void SlewToTarget()
        {
            if (!Settings.AscomOn) return;

            CheckCapability(Settings.CanSlew, "SlewToTarget");
            CheckRange(Scope.RaTarget, 0, 24, "SlewToTarget", "TargetRightAscension");
            CheckRange(Scope.DecTarget, -90, 90, "SlewToTarget", "TargetDeclination");
            CheckParked("SlewToTarget");
            CheckTracking(true, "SlewToTarget");
            var ra = Scope.RaTarget;
            var dec = Scope.DecTarget;
            Scope.SlewRaDec(ra, dec);
            while (Scope.SlewState == SlewType.SlewRaDec || Scope.SlewState == SlewType.SlewSettle)
            {
                //Application.DoEvents();
                DoEvents();
            }
        }

        public void SlewToTargetAsync()
        {
            if (!Settings.AscomOn) return;
            CheckCapability(Settings.CanSlewAsync, "SlewToTargetAsync");
            CheckRange(Scope.RaTarget, 0, 24, "SlewToTargetAsync", "TargetRightAscension");
            CheckRange(Scope.DecTarget, -90, 90, "SlewToTargetAsync", "TargetDeclination");
            CheckParked("SlewToTargetAsync");
            CheckTracking(true, "SlewToTargetAsync");
            var ra = Scope.RaTarget;
            var dec = Scope.DecTarget;
            Scope.SlewRaDec(ra, dec);
        }

        public bool Slewing
        {
            get
            {
                var r = Scope.IsSlewing;
                return r;
            }
        }

        public void SyncToAltAz(double Azimuth, double Altitude)
        {
            CheckCapability(Settings.CanSyncAltAz, "SyncToAltAz");
            CheckRange(Azimuth, 0, 360, "SyncToAltAz", "Azimuth");
            CheckRange(Altitude, -90, 90, "SyncToAltAz", "Altitude");
            CheckParked("SyncToAltAz");
            CheckTracking(false, "SyncToAltAz");
            Settings.AtPark = false;
            Scope.SyncToAltAzm(Altitude, Azimuth);
        }

        public void SyncToCoordinates(double RightAscension, double Declination)
        {
            CheckCapability(Settings.CanSync, "SyncToCoordinates");
            CheckRange(RightAscension, 0, 24, "SyncToCoordinates", "RightAscension");
            CheckRange(Declination, -90, 90, "SyncToCoordinates", "Declination");
            CheckParked("SyncToCoordinates");
            CheckTracking(true, "SyncToCoordinates");
            var radec = Coords.RaDec2Topo(RightAscension, Declination);
            Scope.DecTarget = radec[1];
            Scope.RaTarget = radec[0];
            Settings.AtPark = false;
            Scope.SyncToTargetRaDec();
        }

        public void SyncToTarget()
        {
            CheckCapability(Settings.CanSync, "SyncToTarget");
            CheckRange(Scope.RaTarget, 0, 24, "SyncToTarget", "TargetRightAscension");
            CheckRange(Scope.DecTarget, -90, 90, "SyncToTarget", "TargetDeclination");
            CheckParked("SyncToTarget");
            CheckTracking(true, "SyncToTarget");
            Settings.AtPark = false;
            Scope.SyncToTargetRaDec();
        }

        public double TargetDeclination
        {
            get
            {
                CheckCapability(Settings.CanSlew, "TargetDeclination", false);
                CheckRange(Scope.DecTarget, -90, 90, "TargetDeclination");
                var r = Coords.Topo2CoordDec(0,Scope.DecTarget);
                return r;
            }
            set
            {
                if (!Settings.AscomOn) return;

                CheckCapability(Settings.CanSlew, "TargetDeclination", true);
                CheckRange(value, -90, 90, "TargetDeclination");
                var radec = Coords.RaDec2Topo(0, value);
                Scope.DecTarget = radec[1];
            }
        }

        public double TargetRightAscension
        {
            get
            {
                CheckCapability(Settings.CanSlew, "TargetRightAscension", false);
                CheckRange(Scope.RaTarget, 0, 24, "TargetRightAscension");
                var r = Coords.Topo2CoordRa(Scope.RaTarget, 0);
                return r;
            }
            set
            {
                if (!Settings.AscomOn) return;
                CheckCapability(Settings.CanSlew, "TargetRightAscension", true);
                CheckRange(value, 0, 24, "TargetRightAscension");
                var radec = Coords.RaDec2Topo(value, 0);
                Scope.RaTarget = radec[0];
            }
        }

        public bool Tracking
        {
            get
            {
                var r = Scope.Tracking;
                return r;
            }
            set
            {
                if (!Settings.AscomOn) return;
                Scope.Tracking = value;
            }
        }

        public DriveRates TrackingRate
        {
            get
            {
                var r = Settings.TrackingRate;
                CheckVersionOne("TrackingRate", false);
                return r;
            }
            set
            {
                CheckVersionOne("TrackingRate", true);
                Settings.TrackingRate = value;
            }
        }

        public ITrackingRates TrackingRates
        {
            get
            {
                if (Settings.CanTrackingRates)
                {
                    return _mTrackingRates;
                }
                return _mTrackingRatesSimple;
            }
        }

         public DateTime UTCDate
        {
            get
            {
                var r = DateTime.UtcNow.AddSeconds(Settings.UTCDateOffset);
                return r;
            }
            set
            {
                var r = (int)value.Subtract(DateTime.UtcNow).TotalSeconds;
                Settings.UTCDateOffset = r;
            }
        }

        public void Unpark()
        {
            CheckCapability(Settings.CanUnpark, "UnPark");
            Settings.AtPark = false;
            Scope.Tracking = true;

        }

        #endregion

        #region Private Methods

        private void CheckRate(TelescopeAxes axis, double rate)
        {
            var rates = AxisRates(axis);
            var ratesStr = string.Empty;
            foreach (Rate item in rates)
            {
                if (Math.Abs(rate) >= item.Minimum && Math.Abs(rate) <= item.Maximum)
                {
                    return;
                }
                ratesStr = $"{ratesStr}, {item.Minimum} to {item.Maximum}";
            }
            throw new InvalidValueException("MoveAxis", rate.ToString(CultureInfo.InvariantCulture), ratesStr);
        }

        private static void CheckRange(double value, double min, double max, string propertyOrMethod, string valueName)
        {
            if (double.IsNaN(value))
            {
                throw new ValueNotSetException(propertyOrMethod + ":" + valueName);
            }

            if (value < min || value > max)
            {
                throw new InvalidValueException(propertyOrMethod, value.ToString(CultureInfo.CurrentCulture),
                    string.Format(CultureInfo.CurrentCulture, "{0}, {1} to {2}", valueName, min, max));
            }
        }

        private static void CheckRange(double value, double min, double max, string propertyOrMethod)
        {
            if (double.IsNaN(value))
            {

                throw new ValueNotSetException(propertyOrMethod);
            }

            if (value < min || value > max)
            {
                throw new InvalidValueException(propertyOrMethod, value.ToString(CultureInfo.CurrentCulture),
                    string.Format(CultureInfo.CurrentCulture, "{0} to {1}", min, max));
            }
        }

        private static void CheckVersionOne(string property, bool accessorSet)
        {
            CheckVersionOne(property);
            if (accessorSet)
            {
                //nothing
            }
            if (!Settings.VersionOne) return;

            throw new PropertyNotImplementedException(property, accessorSet);
        }

        private static void CheckVersionOne(string property)
        {
            if (!Settings.VersionOne) return;

            throw new PropertyNotImplementedException(property);
        }

        private static void CheckCapability(bool capability, string method)
        {
            if (capability) return;
            throw new MethodNotImplementedException(method);
        }

        private static void CheckCapability(bool capability, string property, bool setNotGet)
        {
            if (capability) return;
            throw new PropertyNotImplementedException(property, setNotGet);
        }

        private static void CheckParked(string property)
        {
            if (!Settings.AtPark) return;
            throw new ParkedException(property + @": Telescope parked");
        }

        /// <summary>
        /// Checks the slew type and tracking state and raises an exception if they don't match.
        /// </summary>
        /// <param name="raDecSlew">if set to <c>true</c> this is a Ra Dec slew is <c>false</c> an Alt Az slew.</param>
        /// <param name="method">The method name.</param>
        private static void CheckTracking(bool raDecSlew, string method)
        {
            if (raDecSlew == Scope.Tracking) return;
            throw new InvalidOperationException($"{method} is not allowed when tracking is {Scope.Tracking}");
        }

        private static void DoEvents()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        private static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            // free managed resources
            Connected = false;
            _mAxisRates[0].Dispose();
            _mAxisRates[1].Dispose();
            _mAxisRates[2].Dispose();
            _mAxisRates = null;
            _mTrackingRates.Dispose();
            _mTrackingRates = null;
            _mTrackingRatesSimple.Dispose();
            _mTrackingRatesSimple = null;
            _util.Dispose();
            _util = null;
            // free native resources if there are any.
        }
        
        #endregion
    }
}
