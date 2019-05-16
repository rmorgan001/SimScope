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

namespace Principles
{
    /// <summary>
    /// Forces parameters to be within a certain range 
    /// </summary>
    /// <remarks>Attention to the order of parameters (AltAz vs AzAlt) in the method names</remarks>
    public static class Range
    {
        /// <summary>
        /// Returns double in the range -12 to +12
        /// </summary>
        /// <param name="d">90.169444444444451</param>
        /// <returns>-5.8305555555555486</returns>
        public static double Range12(double d)
        {
            while ((d >= 12.0) || (d <= -12.0))
            {
                if (d <= -12.0) d += 24.0;
                if (d >= 12.0) d -= 24.0;
            }
            return d;
        }

        /// <summary>
        /// Returns double in the range 0 to 24.0
        /// </summary>
        /// <param name="d">90.169444444444451</param>
        /// <returns>18.169444444444451</returns>
        public static double Range24(double d)
        {
            while ((d >= 24.0) || (d < 0.0))
            {
                if (d < 0.0) d += 24.0;
                if (d >= 24.0) d -= 24.0;
            }
            return d;
        }

        /// <summary>
        /// Returns double in the range -90 to 90
        /// </summary>
        /// <param name="d">90.169444444444451</param>
        /// <returns>89.830555555555549</returns>
        public static double Range90(double d)
        {
            while ((d > 90.0) || (d < -90.0))
            {
                if (d < -90.0) d += 180.0;
                if (d > 90.0) d = 180.0 - d;
            }
            return d;
        }

        /// <summary>
        /// Returns double in the range -180 to 180
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Range180(double d)
        {
            while (d <= -180.0 || d > 180.0)
            {
                if (d <= -180.0) d += 360;
                if (d > 180) d -= 360;
            }
            return d;
        }

        /// <summary>
        /// Returns double in the range -90 to 0 to 90 to 180 to 270.
        /// </summary>
        /// <param name="d">290.169444444444451</param>
        /// <returns>-69.830555555555577</returns>
        public static double Range270(double d)
        {
            while ((d >= 270) || (d < -90))
            {
                if (d < -90) d += 360.0;
                if (d >= 270) d -= 360.0;
            }
            return d;
        }

        /// <summary>
        /// Returns double in the range 0 to 360
        /// </summary>
        /// <param name="d">590.169444444444451</param>
        /// <returns>230.16944444444448</returns>
        public static double Range360(double d)
        {
            while ((d >= 360.0) || (d < 0.0))
            {
                if (d < 0.0) d += 360.0;
                if (d >= 360.0) d -= 360.0;
            }
            return d;
        }

        /// <summary>
        /// Force range for Altitude and Azimuth
        /// </summary>
        ///  <remarks>Attention to the order given and received</remarks>
        /// <param name="altaz"></param>
        /// <returns></returns>
        public static double[] RangeAltAz(double[] altaz)
        {
           double[] altAz = { Range90(altaz[0]), Range360(altaz[1]) };
           return altAz;
        }

        /// <summary>
        /// Force range for Azimuth an Altitude
        /// </summary>
        /// <remarks>Attention to the order given and received</remarks>
        /// <param name="azalt"></param>
        /// <returns></returns>
        public static double[] RangeAzAlt(double[] azalt)
        {
            double[] azAlt = { Range360(azalt[0]), Range90(azalt[1]) };
            return azAlt;
        }

        /// <summary>
        /// Force range for Right Ascension and Declination
        /// </summary>
        /// <param name="radec"></param>
        /// <returns></returns>
        public static double[] RangeRaDec(double[] radec)
        {
            double[] raDec = { Range24(radec[0]), Range90(radec[1]) };
            return raDec;
        }

        /// <summary>
        /// Force range for primary and secondary axes
        /// </summary>
        /// <param name="axes"></param>
        /// <returns></returns>
        public static double[] RangeAxesXY(double[] axes)
        {
            double[] xy = { Range360(axes[0]), Range270(axes[1]) };
            return xy;
        }

        /// <summary>
        /// Force range for secondar and primary axes
        /// </summary>
        /// <param name="axes"></param>
        /// <returns></returns>
        public static double[] RangeAxesYX(double[] axes)
        {
            double[] xy = { Range270(axes[1]), Range360(axes[0]) };
            return xy;
        }
    }
}