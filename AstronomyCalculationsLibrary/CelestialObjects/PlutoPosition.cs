using Decimal;
using System;
using AstronomyCalculationsLibrary.Common;

namespace AstronomyCalculationsLibrary.CelestialObjects
{
    class PlutoPosition : ICelestialPosition
    {
        public string Name { get; set ; }

        readonly Func<decimal, decimal> FS = d => 50.03m + 0.033459652m * d;
        readonly Func<decimal, decimal> FP = d => 238.95m + 0.003968789m * d;

        private decimal sinP;
        private decimal sin2P;
        private decimal sin3P;
        private decimal sin4P;
        private decimal sin5P;
        private decimal sin6P;
        private decimal sinS_P;

        private decimal cosP;
        private decimal cos2P;
        private decimal cos3P;
        private decimal cos4P;
        private decimal cos5P;
        private decimal cos6P;
        private decimal cosS_P;

        public PlutoPosition(string name)
        {
            Name = name;
        }

        private void SetTrigonometry(decimal days)
        {
            var S = FS(days) * DecimalMath.DegToRad;
            var P = FP(days) * DecimalMath.DegToRad;
            sinP = DecimalMath.Sin(P);
            sin2P = DecimalMath.Sin(2 * P);
            sin3P = DecimalMath.Sin(3 * P);
            sin4P = DecimalMath.Sin(4 * P);
            sin5P = DecimalMath.Sin(5 * P);
            sin6P = DecimalMath.Sin(6 * P);
            sinS_P = DecimalMath.Sin(S - P);

            cosP = DecimalMath.Cos(P);
            cos2P = DecimalMath.Cos(2 * P);
            cos3P = DecimalMath.Cos(3 * P);
            cos4P = DecimalMath.Cos(4 * P);
            cos5P = DecimalMath.Cos(5 * P);
            cos6P = DecimalMath.Cos(6 * P);
            cosS_P = DecimalMath.Cos(S - P);
        }


        private Vector3<decimal> EclipticEarthXYZ(DateTime date)
        {
            var pos = EclipticSunXYZ(date);
            var earthPos = EarthPosition.EarthInstance.EclipticSun3D(date);
#if DEBUG
            Console.WriteLine($"Pluto ecliptic Earth  Xe= {pos.X - earthPos.X}, Ye = {pos.Y - earthPos.Y}, Ze = {pos.Z - earthPos.Z}");
#endif
            return new Vector3<decimal>(pos.X - earthPos.X, pos.Y - earthPos.Y, pos.Z - earthPos.Z);
        }

        private Vector3<decimal> EclipticSunXYZ(DateTime date)
        {
            var days = JulianDateCalculator.ToJulianDaysJ2000(date);
            SetTrigonometry(days);
            var S = FS(days);
            var P = FP(days);
            var lonecl = 238.9508m + 0.00400703m * days
                        - 19.799m * sinP + 19.848m * cosP
                         + 0.897m * sin2P - 4.956m * cos2P
                         + 0.610m * sin3P + 1.211m * cos3P
                         - 0.341m * sin4P - 0.190m * cos4P
                         + 0.128m * sin5P - 0.034m * cos5P
                         - 0.038m * sin6P + 0.031m * cos6P
                         + 0.020m * sinS_P - 0.010m * cosS_P;

            var latecl = - 3.9082m
                         - 5.453m * sinP - 14.975m * cosP
                         + 3.527m * sin2P + 1.673m * cos2P
                         - 1.051m * sin3P + 0.328m * cos3P
                         + 0.179m * sin4P - 0.292m * cos4P
                         + 0.019m * sin5P + 0.100m * cos5P
                         - 0.031m * sin6P - 0.026m * cos6P
                                          + 0.011m * cosS_P;
            var r = 40.72m
                    + 6.68m * sinP + 6.90m * cosP
                    - 1.18m * sin2P - 0.03m * cos2P
                    + 0.15m * sin3P - 0.14m * cos3P;

#if DEBUG
            Console.WriteLine($"Pluto lonecl = {lonecl}, latecl = {latecl}, r = {r}");
#endif

            var sinlon = DecimalMath.Sin(lonecl * DecimalMath.DegToRad);
            var coslon = DecimalMath.Cos(lonecl * DecimalMath.DegToRad);
            var sinlat = DecimalMath.Sin(latecl * DecimalMath.DegToRad);
            var coslat = DecimalMath.Cos(latecl * DecimalMath.DegToRad);

            var xe = r * coslon * coslat;
            var ye = r * sinlon * coslat;
            var ze = r * sinlat;

#if DEBUG
            Console.WriteLine($"Pluto ecliptic Sun  Xe= {xe}, Ye = {ye}, Ze = {ze}");
#endif
            return new Vector3<decimal>(xe, ye, ze);

        }

        public Vector2<decimal> EquatorialCoordinates(DateTime date)
        {
            var tilt = CommonCalculations.GetAxialTilt(date) * DecimalMath.DegToRad;
            var sintilt = DecimalMath.Sin(tilt);
            var costilt = DecimalMath.Cos(tilt);

            var pos = EclipticEarthXYZ(date);
            var xe = pos.X;
            var ye = pos.Y * costilt - pos.Z * sintilt;
            var ze = pos.Y * sintilt - pos.Z * costilt;

#if DEBUG
            Console.WriteLine($"Pluto equatorial X= {xe}, Y = {ye}, Z = {ze}");
#endif

            var RA = DecimalMath.Atan2(ye, xe) * DecimalMath.RadToDeg;
            var Dec = DecimalMath.Atan2(ze, DecimalMath.Sqrt(xe * xe, ye * ye)) * DecimalMath.RadToDeg;

#if DEBUG
            Console.WriteLine($"Pluto Ra= {RA}, DEC = {Dec}");
#endif
            return new Vector2<decimal>(CommonCalculations.From0To360(RA), Dec);
        }

        public Vector2<decimal> EclipticCoordinates(DateTime date)
        {
            var pos = EclipticEarthXYZ(date);
            var lon = DecimalMath.Atan2(pos.Y, pos.X) * DecimalMath.RadToDeg;
            var lat = DecimalMath.Asin(pos.Z / DecimalMath.Sqrt(pos.X * pos.X + pos.Y * pos.Y + pos.Z * pos.Z)) * DecimalMath.RadToDeg;
            return new Vector2<decimal>(CommonCalculations.From0To360(lon), lat);
        }
    }
}
