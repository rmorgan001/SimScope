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
    public static class Conversions
    {
        /// <summary>
        /// Milliseconds to seconds per arcseconds
        /// </summary>
        /// <param name="millseconds"></param>
        /// <param name="rate">Arcseconds per second</param>
        /// <param name="prate">Perentage of rate</param>
        /// <returns></returns>
        public static double Ms2Arcsec(int millseconds, double rate, double prate)
        {
            var a = millseconds / 1000.0;
            var b = GuideRate(rate, prate);
            var c = a * b * 3600;
            return c;
        }

        /// <summary>
        /// Calculate guiderate from rate in arcseconds per second
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="prate"></param>
        /// <returns></returns>
        public static double GuideRate(double rate, double prate)
        {
            var a = ArcSec2Deg(rate);
            var b = a * prate;
            return b;
        }

        /// <summary>
        /// Steps in arcseconds per second
        /// </summary>
        /// <param name="prate"></param>
        /// <param name="totalsteps"></param>
        /// <param name="milliseconds"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static double Rate2Steps(int milliseconds, double rate, double prate, double totalsteps)
        {
            var a = StepPerArcsec(totalsteps);
            var b = a * Ms2Arcsec(milliseconds, rate, prate);
            return b;
        }

        /// <summary>
        /// Calculates steps per arcsecond
        /// </summary>
        /// <param name="totalsteps"></param>
        /// <returns></returns>
        public static double StepPerArcsec(double totalsteps)
        {
            var a = totalsteps / 360 / 3600;
            return a;
        }

        /// <summary>
        /// Arcseconds to degrees
        /// </summary>
        /// <param name="arcsec"></param>
        /// <returns></returns>
        public static double ArcSec2Deg(double arcsec)
        {
            return arcsec / 3600.0;
        }

        /// <summary>
        /// Degrees to Arcseconds
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double Deg2ArcSec(double degrees)
        {
            return degrees * 3600.0;
        }
    }
}
