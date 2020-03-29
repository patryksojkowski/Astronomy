using Decimal;
using System;
using AstronomyCalculationsLibrary.Common;

namespace AstronomyCalculationsLibrary.CelestialObjects
{
    public class PlanetPosition : CelestialPosition
    {
        public PlanetPosition(string name, Func<decimal, decimal> ascedningNode, Func<decimal, decimal> inclination, Func<decimal, decimal> peryhelion,
            Func<decimal, decimal> eccentricity, Func<decimal, decimal> meanAnomaly, Func<decimal, decimal> semimajorAxis)
            : base(name, ascedningNode, inclination, peryhelion, eccentricity, meanAnomaly, semimajorAxis) { }

        public override Vector2<decimal> EquatorialCoordinates(DateTime date)
        {
            var pos = EclipticEarth3D(date);
            var X = pos.X; var Y = pos.Y; var Z = pos.Z;
            var R = DecimalMath.Sqrt(X * X + Y * Y + Z * Z);
            var lambda = DecimalMath.Atan2(Y, X);
            var sinLambda = DecimalMath.Sin(lambda);
            var cosLambda = DecimalMath.Cos(lambda);
            var beta = DecimalMath.Asin(Z / R);
            var tanBeta = DecimalMath.Tan(beta);
            var sinBeta = DecimalMath.Sin(beta);
            var cosBeta = DecimalMath.Cos(beta);
            
            var tilt = CommonCalculations.GetAxialTilt(date) * DecimalMath.DegToRad;
            var cosTilt = DecimalMath.Cos(tilt);
            var sinTilt = DecimalMath.Sin(tilt);
            var RA = DecimalMath.Atan2(sinLambda * cosTilt - tanBeta * sinTilt, cosLambda) * DecimalMath.RadToDeg;
            var Dec = DecimalMath.Asin(sinBeta * cosTilt + cosBeta * sinTilt * sinLambda) * DecimalMath.RadToDeg;
            RA = CommonCalculations.From0To360(RA);
#if DEBUG
            Console.WriteLine($"R = {R} lambda = {lambda}, beta = {beta}, tilt = {tilt}, RA = {RA}, dec = {Dec}");
#endif

            return new Vector2<decimal>(RA, Dec);
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
            var earthPos = EarthPosition.EarthInstance.EclipticSun3D(date);
            var pos = EclipticSun3D(date);
#if DEBUG
            Console.WriteLine($"Geocentric ecliptic position: X = { pos.X - earthPos.X}, Y = { pos.Y - earthPos.Y}, Z = { pos.Z - earthPos.Z}");
#endif
            return new Vector3<decimal>(pos.X - earthPos.X, pos.Y - earthPos.Y, pos.Z - earthPos.Z);
        }

        public override Vector3<decimal> EclipticSun3D(DateTime date)
        {
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
                + e4 * (-(1m/6m) * sin2M + (1m/3m) * sin4M)
                + e5 * (-(27m/128m) * sin3M + (1m/192m) * sinM + (125m/384m) * sin5M)
                + e6 * ((1m/48m) * sin2M + (27/80) * sin6M - (4/15) * sin4M);

#if DEBUG
            Console.WriteLine($"Eccentric Anomaly {eccentricAnomalyRad * DecimalMath.RadToDeg}");
#endif

            // Calculate true anomaly
            var A = SemimajorAxis(days);
            var Xv = A * (DecimalMath.Cos(eccentricAnomalyRad) - e);
            var Yv = A * DecimalMath.Sqrt(1.0m - e * e) * DecimalMath.Sin(eccentricAnomalyRad);

            var v = DecimalMath.Atan2(Yv, Xv);
            var r = DecimalMath.Sqrt(Xv * Xv + Yv * Yv);

            //Calculate Heliocentric coordinates
            var N = LongAscedingNode(days) * degToRad;
            var cosN = DecimalMath.Cos(N);
            var sinN = DecimalMath.Sin(N);
            var i = Inclination(days) * degToRad;
            var cosi = DecimalMath.Cos(i);
            var sini = DecimalMath.Sin(i);
            var w = Peryhelion(days) * degToRad;
            var cosVW = DecimalMath.Cos(v + w);
            var sinVW = DecimalMath.Sin(v + w);

#if DEBUG
            Console.WriteLine($"LongAscNode : {LongAscedingNode(days)}, Inclination : {Inclination(days)}");
                Console.WriteLine($"Peryhelion: {Peryhelion(days)} true anomaly : {DecimalMath.Atan2(Yv, Xv) * DecimalMath.RadToDeg}");
#endif

            var Xh = r * (cosN * cosVW - sinN * sinVW * cosi);
            var Yh = r * (sinN * cosVW + cosN * sinVW * cosi);
            var Zh = r * (sinVW * sini);

#if DEBUG
            Console.WriteLine($"{Name} heliocentric coordinates {Xh} {Yh} {Zh}");
#endif

            return new Vector3 <decimal>(Xh, Yh, Zh);
        }

        public override Vector3<decimal> ApparentEclipticEarth3D(DateTime date)
        {
            var E = EarthPosition.EarthInstance.EclipticSun3D(date);
            Vector3<decimal> P;

            var tempDate = date;
            var TT = decimal.MaxValue;
            while(true)
            {
                P = EclipticSun3D(tempDate);
                var s = Vector3<decimal>.Distance(E, P);

                var newTT = s / CommonCalculations.LIGHTSPEED_AUperTick;
                if (DecimalMath.Abs(newTT - TT) < TimeSpan.TicksPerSecond) // try with date up to 1 second.
                {
                    break;
                }
                tempDate -= new TimeSpan(Convert.ToInt64(newTT));
                TT = newTT;
            }
            return new Vector3<decimal>(P.X - E.X, P.Y - E.Y, P.Z - E.Z);
        }

        public override Vector2<decimal> ApparentEclipticEarth(DateTime date)
        {
            var pos = ApparentEclipticEarth3D(date);
            var lon = DecimalMath.Atan2(pos.Y, pos.X) * DecimalMath.RadToDeg;
            var lat = DecimalMath.Asin(pos.Z / DecimalMath.Sqrt(pos.X * pos.X + pos.Y * pos.Y + pos.Z * pos.Z)) * DecimalMath.RadToDeg;
            lon = CommonCalculations.From0To360(lon);
            lat = CommonCalculations.From0To360(lat);
            return new Vector2<decimal>(lon, lat);
        }

    }
}
