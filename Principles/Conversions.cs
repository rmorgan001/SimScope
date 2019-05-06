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
    }
}
