using System;
using ASCOM.DeviceInterface;
using Principles;

namespace SimServer.Helpers
{
    /// <summary>
    /// Convertion of mount axis positions 
    /// </summary>
    /// <remarks>Attention to the order of parameters (AltAz vs AzAlt) in the method names</remarks>
    internal static class Axes
    {
        /// <summary>
        /// convert a decimal Alt/Az positions to an axes positions.
        /// </summary>
        /// <param name="altAz"></param>
        /// <returns></returns>
        internal static double[] AltAzToAxesYX(double[] altAz)
        {
            var axes = altAz;
            double lst;
            switch (Settings.AlignmentMode)
            {
                case AlignmentModes.algAltAz:
                    break;
                case AlignmentModes.algGermanPolar:
                    lst = Scope.SiderealTime;
                    axes = Coordinate.AltAz2RaDec(altAz[0], altAz[1], Settings.Latitude, lst);
                    axes[0] = Coordinate.Ra2Ha(axes[0], lst) * 15; // ha in degrees

                    if (Scope.SouthernHemisphere) axes[1] = -axes[1];

                    axes = Range.RangeAzAlt(axes);
 
                    if (axes[0] > 180.0 || axes[0] < 0)
                    {
                        // adjust the targets to be through the pole
                        axes[0] += 180;
                        axes[1] = 180 - axes[1];
                    }
                    break;
                case AlignmentModes.algPolar:
                    lst = Scope.SiderealTime;
                    axes = Coordinate.AltAz2RaDec(altAz[0], altAz[1], Settings.Latitude, lst);
                    axes[0] = Coordinate.Ra2Ha(axes[0], lst) * 15; // ha in degrees

                    if (Scope.SouthernHemisphere) axes[1] = -axes[1];

                    axes = Range.RangeAzAlt(axes);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            axes = Range.RangeAxesXY(axes);
            return new[] { axes[1], axes[0]};
        }

        /// <summary>
        /// Conversion of mount axis positions in degrees to Alt and Az
        /// </summary>
        /// <param name="axes"></param>
        /// <returns>AzAlt</returns>
        internal static double[] AxesXYToAzAlt(double[] axes)
        {
            var a = AxesYXToAltAz(new[] {axes[1], axes[0]});
            var b = new[] {a[1], a[0]};
            return b;
        }

        /// <summary>
        /// Conversion of mount axis positions in degrees to Alt and Az
        /// </summary>
        /// <param name="axes"></param>
        /// <returns>AltAz</returns>
        private static double[] AxesYXToAltAz(double[] axes)
        {
            var altAz = axes;
            switch (Settings.AlignmentMode)
            {
                case AlignmentModes.algAltAz:
                    break;
                case AlignmentModes.algGermanPolar:
                    if (axes[0] > 90)
                    {
                         axes[1] += 180.0;
                         axes[0] = 180 - axes[0];
                         axes = Range.RangeAltAz(axes);
                    }

                    //southern hemi
                    if (Scope.SouthernHemisphere) axes[0] = -axes[0];

                    //axis degrees to ha
                    var ha = axes[1] / 15.0;
                    altAz = Coordinate.HaDec2AltAz(ha, axes[0], Settings.Latitude);
                    break;
                case AlignmentModes.algPolar:
                    //axis degrees to ha
                    ha = axes[1] / 15.0;
                    altAz = Coordinate.HaDec2AltAz(ha, axes[0], Settings.Latitude);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            altAz = Range.RangeAltAz(altAz);
            return altAz;
        }

        /// <summary>
        /// Conversion of mount axis positions in degrees to Ra and Dec
        /// </summary>
        /// <param name="axes"></param>
        /// <returns></returns>
        internal static double[] AxesXYToRaDec(double[] axes)
        {
            double[] raDec = { 0, 0 };
            switch (Settings.AlignmentMode)
            {
                case AlignmentModes.algAltAz:
                    var radec = Coordinate.AltAz2RaDec(Scope.Altitude, Scope.Azimuth, Settings.Latitude, Scope.SiderealTime);
                    axes[0] = Coordinate.Ra2Ha(radec[0], Scope.SiderealTime) * 15; // ha in degrees
                    axes[1] = radec[1];
                    break;
                case AlignmentModes.algGermanPolar:
                case AlignmentModes.algPolar:
                    if (axes[1] > 90)
                    {
                        axes[0] += 180.0;
                        axes[1] = 180 - axes[1];
                        axes = Range.RangeAzAlt(axes);
                    }                        

                    raDec[0] = Scope.SiderealTime - axes[0] / 15.0;
                    raDec[1] = axes[1];
                    //southern hemi
                    if (Scope.SouthernHemisphere) raDec[1] = -axes[1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            raDec = Range.RangeRaDec(raDec);
            return raDec;
        }

        /// <summary>
        /// convert a RaDec position to an axes positions. 
        /// </summary>
        /// <param name="raDec"></param>
        /// <param name="preserveSop"></param>
        /// <returns></returns>
        internal static double[] RaDecToAxesXY(double[] raDec, bool preserveSop = false)
        {
            var axes = raDec;
            switch (Settings.AlignmentMode)
            {
                case AlignmentModes.algAltAz:
                    axes = Range.RangeAzAlt(axes);
                    axes = Coordinate.RaDec2AltAz(axes[0], axes[1], Scope.SiderealTime, Settings.Latitude);
                    return axes;
                case AlignmentModes.algGermanPolar:
                    axes[0] = (Scope.SiderealTime - raDec[0]) * 15.0;
                    if (Scope.SouthernHemisphere) axes[1] = -axes[1];
                    axes[0] = Range.Range360(axes[0]);

                    if (axes[0] > 180.0 || axes[0] < 0)
                    {
                        // adjust the targets to be through the pole
                        axes[0] += 180;
                        axes[1] = 180 - axes[1];
                    }

                    var sop = Scope.SideOfPier;
                    var newsop = (axes[1] <= 90 && axes[1] >= -90) ? PierSide.pierEast : PierSide.pierWest;
                    if (preserveSop && newsop != sop)
                    {
                        if (Settings.NoSyncPastMeridian)
                            throw new InvalidOperationException("Sync is not allowed when the mount has tracked past the meridian");

                        axes[0] -= 180;
                        axes[1] = 180 - axes[1];
                    }
                    break;
                case AlignmentModes.algPolar:
                    axes[0] = (Scope.SiderealTime - raDec[0]) * 15.0;
                    axes[1] = (Scope.SouthernHemisphere) ? -raDec[1] : raDec[1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            axes = Range.RangeAxesXY(axes);
            return axes;
        }

        /// <summary>
        /// Conversion of simulator axis to physical mount 
        /// </summary>
        /// <param name="axes">X,Y format</param>
        /// <returns></returns>
        internal static double[] SimToMount(double[] axes)
        {
            var a = new[] { axes[0], axes[1] };
            if (!Scope.SouthernHemisphere) return a;
            a[0] = 180 - axes[0];
            a[1] = axes[1];
            return a;
        }

        /// <summary>
        /// Conversion of physical mount to simulator
        /// </summary>
        /// <param name="axes">X,Y format</param>
        /// <returns></returns>
        internal static double[] MountToSim(double[] axes)
        {
            var a = new[] { axes[0], axes[1] };
            if (!Scope.SouthernHemisphere) return a;
            a[0] = axes[0] * -1.0;
            a[1] = (180 - axes[1]);
            return a;
        }
    }
}
