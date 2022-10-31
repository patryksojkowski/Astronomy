using Decimal;
using System;
using AstronomyCalculationsLibrary.Common;
using AstronomyCalculationsLibrary.Interfaces;

namespace AstronomyCalculationsLibrary
{
    internal class AscendantCalculations : IAscendantCalculations
    {
        public decimal GetAscendant(double longitude, double latitude, DateTime dateTime)
        {
            var pi = DecimalMath.Pi;
            var degToRadCoefficient = pi / 180;

            var longitudeDec = Convert.ToDecimal(-longitude); // East should be positive, West negative
            var latitudeRad = Convert.ToDecimal(latitude) * degToRadCoefficient;
            var tilt = CommonCalculations.GetAxialTilt(dateTime);
            var tiltRad = tilt * degToRadCoefficient;
#if DEBUG
            Console.WriteLine($"tilt: {tilt}");
#endif
            var siderealTime = SiderealTime.Calculate(longitudeDec, dateTime);
            var siderealTimeRad = siderealTime * degToRadCoefficient;
#if DEBUG
            Console.WriteLine($"sidereal Time: {siderealTime}");
#endif
            var y = - DecimalMath.Cos(siderealTimeRad);
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
            var output = DecimalMath.ATan(y / x) * 180 / pi;

            if (output < 0)
            {
                output += 180;
            }
            if (siderealTimeRad > pi / 2 && siderealTimeRad < 3 * pi / 2)
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
