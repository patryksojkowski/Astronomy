using Decimal;
using System;
using AstronomyCalculationsLibrary.Common;

namespace AstronomyCalculationsLibrary.CelestialObjects
{
    internal class SunPosition : CelestialPosition
    {
        private static DateTime currentDate = DateTime.MinValue;
        private static Vector3<decimal> sunPosition = new Vector3<decimal>(0,0,0);

        public SunPosition SunInstance = null;

        public SunPosition(string name, Func<decimal, decimal> ascedningNode, Func<decimal, decimal> inclination, Func<decimal, decimal> peryhelion,
            Func<decimal, decimal> eccentricity, Func<decimal, decimal> meanAnomaly, Func<decimal, decimal> semimajorAxis)
            : base(name, ascedningNode, inclination, peryhelion, eccentricity, meanAnomaly, semimajorAxis)
        {
            SunInstance = this;
        }

        public override Vector2<decimal> EquatorialCoordinates(DateTime date)
        {

            var pos = EclipticEarth3D(date);
#if DEBUG
            Console.WriteLine($"Sun ecliptic position: X={pos.X} Y={pos.Y} Z={pos.Z}");
#endif

            var tiltRad = CommonCalculations.GetAxialTilt(date) * DecimalMath.DegToRad;
            var Xe = pos.X;
            var Ye = pos.Y * DecimalMath.Cos(tiltRad);
            var Ze = pos.Y * DecimalMath.Sin(tiltRad);

#if DEBUG
            Console.WriteLine($"Sun equatorial position: X={Xe} Y={Ye} Z={Ze} with tilt {tiltRad}");
#endif

            var RA = DecimalMath.Atan2(Ye, Xe) * DecimalMath.RadToDeg;
            var Dec = DecimalMath.Atan2(Ze, DecimalMath.Sqrt(Xe * Xe + Ye * Ye)) * DecimalMath.RadToDeg;

#if DEBUG
            Console.WriteLine($"Sun RA = {RA} Dec={Dec}");
#endif
            return new Vector2<decimal>(CommonCalculations.From0To360(RA), Dec);
        }

        public override Vector2<decimal> EclipticCoordinates(DateTime date)
        {
            var pos = EclipticEarth3D(date);
            var lon = DecimalMath.Atan2(pos.Y, pos.X) * DecimalMath.RadToDeg;
            var lat = DecimalMath.Asin(pos.Z / DecimalMath.Sqrt(pos.X * pos.X + pos.Y * pos.Y + pos.Z * pos.Z)) * DecimalMath.RadToDeg;
            return new Vector2<decimal>(CommonCalculations.From0To360(lon), lat);
        }

        public override Vector3<decimal> EclipticEarth3D(DateTime date)
        {
            if(!DateChanged(date))
            {
                return sunPosition;
            }
            var earthPos = EarthPosition.EarthInstance.EclipticSun3D(date);
            return sunPosition = new Vector3<decimal>(-earthPos.X, -earthPos.Y, -earthPos.Z);
        }

        public override Vector3<decimal> EclipticSun3D(DateTime date)
        {
            return new Vector3<decimal>(0, 0, 0);
        }

        private bool DateChanged(DateTime date)
        {
            if(date != currentDate)
            {
                currentDate = date;
                return true;
            }
            return false;
        }

        public override Vector3<decimal> ApparentEclipticEarth3D(DateTime date)
        {
            throw new NotImplementedException();
        }

        public override Vector2<decimal> ApparentEclipticEarth(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
