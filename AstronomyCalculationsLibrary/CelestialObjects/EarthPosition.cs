using Decimal;
using System;
using AstronomyCalculationsLibrary.Common;

namespace AstronomyCalculationsLibrary.CelestialObjects
{
    public class EarthPosition : CelestialPosition
    {
        private static DateTime currentDate = DateTime.MinValue;
        private static Vector3<decimal> earthPosition = new Vector3<decimal>(0, 0, 0);

        public static EarthPosition EarthInstance = null;

        public EarthPosition(string name, Func<decimal, decimal> ascedningNode, Func<decimal, decimal> inclination, Func<decimal, decimal> peryhelion,
            Func<decimal, decimal> eccentricity, Func<decimal, decimal> meanAnomaly, Func<decimal, decimal> semimajorAxis)
            : base(name, ascedningNode, inclination, peryhelion, eccentricity, meanAnomaly, semimajorAxis)
        {
            EarthInstance = this;
        }

        public override Vector2<decimal> EquatorialCoordinates(DateTime date)
        {
            throw new NotImplementedException();
        }

        public override Vector2<decimal> EclipticCoordinates(DateTime date)
        {
            throw new NotImplementedException();
        }

        public override Vector3<decimal> EclipticEarth3D(DateTime date)
        {
            return new Vector3<decimal>(0, 0, 0);
        }

        public override Vector3<decimal> EclipticSun3D(DateTime date)
        {
            if(!DateChanged(date))
            {
                return earthPosition;
            }

            var degToRad = DecimalMath.Pi / 180;

            var days = JulianDateCalculator.ToJulianDaysJ2000(date);

            var M = MeanAnomaly(days);
            var MRad = M * degToRad;
            var e = Eccentricity(days);
#if DEBUG
            Console.WriteLine($"Days = {days} Mean Anomaly={M} Eccentricity = {e}");
#endif

            var sinM = DecimalMath.Sin(MRad);
            var sin2M = DecimalMath.Sin(2 * MRad);
            var sin3M = DecimalMath.Sin(3 * MRad);
            var sin4M = DecimalMath.Sin(4 * MRad);
            var sin5M = DecimalMath.Sin(5 * MRad);
            var sin6M = DecimalMath.Sin(6 * MRad);
            var e2 = DecimalMath.Power(e, 2);
            var e3 = DecimalMath.Power(e, 3);
            var e4 = DecimalMath.Power(e, 4);
            var e5 = DecimalMath.Power(e, 5);
            var e6 = DecimalMath.Power(e, 6);

            var eccentricAnomalyRad =
                MRad
                + e * sinM
                + e2 * 0.5m * sin2M
                + e3 * (0.375m * sin3M - 0.125m * sinM)
                + e4 * (-(1m / 6m) * sin2M + (1m / 3m) * sin4M)
                + e5 * (-(27m / 128m) * sin3M + (1m / 192m) * sinM + (125m / 384m) * sin5M)
                + e6 * ((1m / 48m) * sin2M + (27 / 80) * sin6M - (4 / 15) * sin4M);

#if DEBUG
            Console.WriteLine($"Eccentric Anomaly {eccentricAnomalyRad * DecimalMath.RadToDeg}");
#endif

            // Calculate true anomaly
            var A = SemimajorAxis(days);
            var Xv = A * (DecimalMath.Cos(eccentricAnomalyRad) - e);
            var Yv = A * DecimalMath.Sqrt(1.0m - e * e) * DecimalMath.Sin(eccentricAnomalyRad);
#if DEBUG
            Console.WriteLine($"SemimajorAxis = {A}, Xv = {Xv}, Yv = {Yv}");
#endif
            var v = (DecimalMath.Atan2(Yv, Xv));
            var r = DecimalMath.Sqrt(Xv * Xv + Yv * Yv);
#if DEBUG
            Console.WriteLine($"v = {v}, r = {r}");
#endif
            //Calculate Heliocentric coordinates
            var N = LongAscedingNode(days) * degToRad;
            var cosN = DecimalMath.Cos(N);
            var sinN = DecimalMath.Sin(N);
            var i = Inclination(days) * degToRad;
            var cosi = DecimalMath.Cos(i);
            var sini = DecimalMath.Sin(i);
            var wRad = Peryhelion(days) * degToRad;
            var cosVW = DecimalMath.Cos(v + wRad);
            var sinVW = DecimalMath.Sin(v + wRad);
#if DEBUG
            Console.WriteLine($"LongAscNode : {LongAscedingNode(days)}, Inclination : {Inclination(days)}");
            Console.WriteLine($"Peryhelion: {Peryhelion(days)} true anomaly : {DecimalMath.Atan2(Yv, Xv) * DecimalMath.RadToDeg}");
#endif

            var Xh = r * (cosN * cosVW - sinN * sinVW * cosi);
            var Yh = r * (sinN * cosVW + cosN * sinVW * cosi);
            var Zh = r * (sinVW * sini);
#if DEBUG
            Console.WriteLine($"Earth heliocentric coordinates {-Xh} {-Yh} {-Zh}");
#endif
            // Values are reversed because calculations are for Sun orbiting Earth
            return earthPosition = new Vector3<decimal>(-Xh, -Yh, -Zh);
        }

        private bool DateChanged(DateTime date)
        {
            if (date != currentDate)
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
