using System;

namespace AstronomyCalculationsLibrary.Common
{
    internal static class SiderealTime
    {
        private static readonly decimal L0 = 99.967794687m;
        private static readonly decimal L1 = 360.98564736628603m;
        private static readonly decimal L2 = 0.0000000000002907879m;
        private static readonly decimal L3 = -0.0000000000000000000005302m;

        private static readonly DateTime referenceTime = new DateTime(2000, 1, 1, 0, 0, 0);

        public static decimal Calculate(decimal longitude, DateTime dateTime)
        {
            double daysPassed = (dateTime - referenceTime).TotalDays;

            // sidereal time formula https://www.aa.quae.nl/en/reken/sterrentijd.html
            var theta = L0 
                + L1 * Convert.ToDecimal(daysPassed)
                + L2 * Convert.ToDecimal(Math.Pow(daysPassed, 2))
                + L3 * Convert.ToDecimal(Math.Pow(daysPassed, 3))
                - longitude;

            theta %= 360;
            theta += theta < 0 ? 360 : 0;
            var ret = theta;
#if DEBUG
            theta /= 15; // to hours
            TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(theta));
            int minutes = ts.Minutes;
            var minutesFormat = minutes >= 10 ? $"{minutes}" : $"0{minutes}";
            Console.WriteLine( $"sidereal time: {ts.Hours}:{minutesFormat}");
#endif
            return ret;
        }
    }
}
