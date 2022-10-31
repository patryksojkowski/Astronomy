using System;
using AstronomyCalculationsLibrary.Common;

namespace AstronomyCalculationsLibrary.CelestialObjects
{
  internal class KetuPosition : ICelestialPosition
  {
    private readonly decimal n0 = 125.04452m;
    private readonly decimal n1 = -0.05295376484m;

    public KetuPosition(string name)
    {
      Name = name;
    }

    public string Name { get; set; }

    public Vector2<decimal> EquatorialCoordinates(DateTime date)
    {
      var d = JulianDateCalculator.ToJulianDaysJ2000(date);
      return new Vector2<decimal>((n0 + n1 * d + 180m).From0To360(), 0);
    }

    public Vector2<decimal> EclipticCoordinates(DateTime date)
    {
      var d = JulianDateCalculator.ToJulianDaysJ2000(date);
      return new Vector2<decimal>((n0 + n1 * d + 180m).From0To360(), 0);
    }
  }
}