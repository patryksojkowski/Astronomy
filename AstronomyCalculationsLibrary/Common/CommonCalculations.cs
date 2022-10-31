using System;
using Decimal;

namespace AstronomyCalculationsLibrary.Common
{
  /// <summary>
  ///   Class contains common methods
  /// </summary>
  internal static class CommonCalculations
  {
    /// <summary>
    ///   Distance covered by light in one second expressed in astronomical units
    /// </summary>
    public const decimal LIGHTSPEED_AUperS = 0.00200398880410000381108365601891m;

    /// <summary>
    ///   Distance covered by light in one milisecond expressed in astronomical units
    /// </summary>
    public const decimal LIGHTSPEED_AUperMS = 0.00000200398880410000381108365601891m;

    /// <summary>
    ///   Distance covered by light in one tick expressed in astronomical units
    /// </summary>
    public const decimal LIGHTSPEED_AUperTick = 0.000000000200398880410000381108365601891m;

    // Axial tilt constants
    private static readonly decimal t0 = 23.439291111111m;
    private static readonly decimal t1 = -0.0130041666667m;
    private static readonly decimal t2 = -1.63888888889e-07m;
    private static readonly decimal t3 = 5.959274797e-09m;

    /// <summary>
    ///   Calculated using linear regression. For details see VedicShiftRegression.xlsx.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static decimal GetVedicShift(DateTime date)
    {
#if DEBUG
      Console.WriteLine($"Vedic shift: {date.Year * 0.013919781m - 3.98276936m}");
#endif
      return date.Year * 0.013919781m - 3.98276936m;
    }

    // Wolfram command
    // plot 23.439291111111 - 0.0130041666667*t - 1.63888888889e-07*t^2 + 5.959274797e-09*t^3 t from -1 to 1
    // where t is fraction of Julian Century passed from J2000
    /// <summary>
    ///   Get current axial tilt (epsilon) based on date.
    ///   Value of 23.4 degrees is slowly decreasing.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static decimal GetAxialTilt(DateTime dateTime)
    {
      var julianCenturies = JulianDateCalculator.ToJulianCenturiesJ2000(dateTime);

      return t0
             + t1 * julianCenturies
             + t2 * DecimalMath.Power(julianCenturies, 2)
             + t3 * DecimalMath.Power(julianCenturies, 3);
    }

    public static decimal From0To360(this decimal deg)
    {
      return (deg % 360 + 360) % 360;
    }
  }
}