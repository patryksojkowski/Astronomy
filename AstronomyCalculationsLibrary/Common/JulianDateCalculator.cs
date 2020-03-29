using System;

namespace AstronomyCalculationsLibrary.Common
{
    internal static class JulianDateCalculator
    {
        private static readonly decimal secondsInJulianDay = 86400m;
        private static readonly decimal secondsInJulianCentury = 3155760000m;
        private static readonly decimal julianDaysJ2000 = 2451545m;

        public static decimal ToJulianCenturiesJ2000(DateTime date)
        {
            var days = ToJulianDays(date) - julianDaysJ2000;
            return days * secondsInJulianDay / secondsInJulianCentury;
        }

        public static decimal ToJulianDaysJ2000(DateTime date)
        {
            var days = ToJulianDays(date);
#if DEBUG
            Console.WriteLine($"Julian day: {days}");
#endif
            return days - julianDaysJ2000;
        }

        static private decimal ToJulianDays(DateTime date)
        {
            return Convert.ToDecimal(date.ToOADate() + 2415018.5);
        }
    }
}
