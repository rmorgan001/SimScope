using ASCOM.Astrometry.Transform;
using ASCOM.DeviceInterface;

namespace SimServer.Helpers
{
    public static class Coords
    {
        private static Transform xform;

        static Coords()
        {
            Update();
        }

        public static void Update()
        {
            xform = new Transform
            {
                SiteElevation = Settings.Elevation,
                SiteLatitude = Settings.Latitude,
                SiteLongitude = Settings.Longitude,
                Refraction = Settings.CanDoesRefraction,
                SiteTemperature = Settings.Temperature
            };
        }
        
        /// <summary>
        /// Converts RA and DEC to the required EquatorialCoordinateType
        /// Used for all RA and DEC corrdinates comming into the system 
        /// </summary>
        /// <param name="rightAscension"></param>
        /// <param name="declination"></param>
        /// <returns></returns>
        public static double[] RaDec2Topo(double rightAscension, double declination)
        {
            switch (Settings.EquatorialCoordinateType)
            {
                case EquatorialCoordinateType.equJ2000:
                    xform.SetJ2000(rightAscension, declination);
                    break;
                case EquatorialCoordinateType.equLocalTopocentric:
                    return new[] {rightAscension, declination};
                case EquatorialCoordinateType.equOther:
                    xform.SetApparent(rightAscension, declination);
                    break;
                case EquatorialCoordinateType.equJ2050:
                    xform.SetJ2000(rightAscension, declination);
                    break;
                case EquatorialCoordinateType.equB1950:
                    xform.SetJ2000(rightAscension, declination);
                    break;
                default:
                    return new[] { rightAscension, declination };
            }
            return new[] { xform.RATopocentric, xform.DECTopocentric };
        }

        /// <summary>
        /// Converts internal stored coords to the stored EquatorialCoordinateType
        /// Used for all RA and DEC corrdinates going out of the system  
        /// </summary>
        /// <param name="rightAscension"></param>
        /// <param name="declination"></param>
        /// <returns></returns>
        public static double[] Topo2Coords(double rightAscension, double declination)
        {
            var radec = new double[]{0,0};
            //internal is already topo so return it
            if (Settings.EquatorialCoordinateType == EquatorialCoordinateType.equLocalTopocentric) return new[]{rightAscension, declination};

            xform.SetTopocentric(rightAscension, declination);
            switch (Settings.EquatorialCoordinateType)
            {
                case EquatorialCoordinateType.equJ2000:
                    radec[0] = xform.RAJ2000;
                    radec[1] = xform.DecJ2000;
                    break;
                case EquatorialCoordinateType.equLocalTopocentric:
                    radec[0] = rightAscension;
                    radec[1] = declination;
                    break;
                case EquatorialCoordinateType.equOther:
                    radec[0] = xform.RAApparent;
                    radec[1] = xform.DECApparent;
                    break;
                case EquatorialCoordinateType.equJ2050:
                    radec[0] = xform.RAJ2000;
                    radec[1] = xform.DecJ2000;
                    break;
                case EquatorialCoordinateType.equB1950:
                    radec[0] = xform.RAJ2000;
                    radec[1] = xform.DecJ2000;
                    break;
                default:
                    radec[0] = rightAscension;
                    radec[1] = declination;
                    break;
            }
            return radec;
        }

        /// <summary>
        /// Converts internal stored coords to the stored EquatorialCoordinateType
        /// Used for all RA corrdinates going out of the system  
        /// </summary>
        /// <param name="rightAscension"></param>
        /// <param name="declination"></param>
        /// <returns></returns>
        public static double Topo2CoordRa(double rightAscension, double declination)
        {
            double ra;
            //internal is already topo so return it
            if (Settings.EquatorialCoordinateType == EquatorialCoordinateType.equLocalTopocentric) return rightAscension;

            xform.SetTopocentric(rightAscension, declination);
            switch (Settings.EquatorialCoordinateType)
            {
                case EquatorialCoordinateType.equJ2000:
                    ra = xform.RAJ2000;
                    //radec.Y = declination;
                    break;
                case EquatorialCoordinateType.equLocalTopocentric:
                    ra = rightAscension;
                    //radec.Y = xform.DECTopocentric;
                    break;
                case EquatorialCoordinateType.equOther:
                    ra = xform.RAApparent;
                    //radec.Y = xform.DECApparent;
                    break;
                case EquatorialCoordinateType.equJ2050:
                    ra = xform.RAJ2000;
                    //radec.Y = xform.DecJ2000;
                    break;
                case EquatorialCoordinateType.equB1950:
                    ra = xform.RAJ2000;
                    //radec.Y = xform.DecJ2000;
                    break;
                default:
                    ra = rightAscension;
                    //radec.Y = xform.DECTopocentric;
                    break;
            }
            return ra;
        }

        /// <summary>
        /// Converts internal stored topo coords to topocentric azimth
        /// Used for all RA corrdinates going out of the system  
        /// </summary>
        /// <param name="rightAscension"></param>
        /// <param name="declination"></param>
        /// <returns></returns>
        public static double Topo2CoordAz(double rightAscension, double declination)
        {

            xform.SetTopocentric(rightAscension, declination);
            var az = xform.AzimuthTopocentric;
            return az;
        }

        /// <summary>
        /// Converts internal stored coords to the stored EquatorialCoordinateType
        /// Used for all DEC corrdinates going out of the system  
        /// </summary>
        /// <param name="rightAscension"></param>
        /// <param name="declination"></param>
        /// <returns></returns>
        public static double Topo2CoordDec(double rightAscension, double declination)
        {
            double dec;
            //internal is already topo so return it
            if (Settings.EquatorialCoordinateType == EquatorialCoordinateType.equLocalTopocentric) return declination;
            xform.SetTopocentric(rightAscension, declination);
            switch (Settings.EquatorialCoordinateType)
            {
                case EquatorialCoordinateType.equJ2000:
                    //ra = rightAscension;
                    dec = xform.DecJ2000;
                    break;
                case EquatorialCoordinateType.equLocalTopocentric:
                    //ra = xform.RATopocentric;
                    dec = declination;
                    break;
                case EquatorialCoordinateType.equOther:
                    //ra = xform.RAApparent;
                    dec = xform.DECApparent;
                    break;
                case EquatorialCoordinateType.equJ2050:
                    //ra = xform.RAJ2000;
                    dec = xform.DecJ2000;
                    break;
                case EquatorialCoordinateType.equB1950:
                    //ra = xform.RAJ2000;
                    dec = xform.DecJ2000;
                    break;
                default:
                    //ra = xform.RATopocentric;
                    dec = xform.DECTopocentric;
                    break;
            }
            return dec;
        }
    }
}
