using Decimal;
using System;
using AstronomyCalculationsLibrary.Common;

namespace AstronomyCalculationsLibrary
{
    internal static class AscendantCalculations
    {
        public static decimal GetAscendant(double longi, double lat, DateTime dateTime, bool isVedic = true)
        {
            var Pi = DecimalMath.Pi;
            var DegToRad = Pi / 180;
            var longitude = Convert.ToDecimal(-longi); // East should be positive, West negative
            var latitudeRad = Convert.ToDecimal(lat) * DegToRad;
            var tilt = CommonCalculations.GetAxialTilt(dateTime);
            var tiltRad = tilt * DegToRad;
#if DEBUG
            Console.WriteLine($"tilt: {tilt}");
#endif
            var siderealTime = SiderealTime.Calculate(longitude, dateTime);
            var siderealTimeRad = siderealTime * DegToRad;
#if DEBUG
            Console.WriteLine($"sidereal Time: {siderealTime}");
#endif
            var y = -DecimalMath.Cos(siderealTimeRad);
#if DEBUG
            Console.WriteLine($"y :{y}");
            Console.WriteLine(
                $"sin(RAMC) = {DecimalMath.Sin(siderealTimeRad)}" +
                $"\ncos(TILT) = {DecimalMath.Cos(tiltRad)}" +
                $"\ntan(LAT) = {DecimalMath.Tan(latitudeRad)}" +
                $"\nsin(TILT) = {DecimalMath.Sin(tiltRad)}");
#endif
            var x = DecimalMath.Sin(siderealTimeRad) * DecimalMath.Cos(tiltRad)
                + DecimalMath.Tan(latitudeRad) * DecimalMath.Sin(tiltRad);
#if DEBUG
            Console.WriteLine($"x :{x}");
            Console.WriteLine($"Decimal y/x: {y / x}");
            Console.WriteLine($"Atan(y/x): {DecimalMath.ATan(y / x)}");
#endif
            var output = DecimalMath.ATan(y / x) * 180 / Pi;

            if (output < 0)
            {
                output += 180;
            }
            if (siderealTimeRad > Pi / 2 && siderealTimeRad < 3 * Pi / 2)
            {
                output += 180;
                output %= 360;
            }
#if DEBUG
            Console.WriteLine($"output before applying sidereal diff{output}");
#endif
            return output;
        }
    }
}
