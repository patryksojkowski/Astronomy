using System;
using System.Collections.Generic;
using AstronomyCalculationsLibrary.Common;

namespace AstronomyCalculationsLibrary
{
  /// <summary>
  ///   Descriptions for planet numbers:
  ///   { 0, "Ascendant" },
  ///   { 1, "Sun"    },
  ///   { 2, "Mercury"},
  ///   { 3, "Venus"  },
  ///   { 4, "Mars"},
  ///   { 5, "Jupiter"},
  ///   { 6, "Saturn"  },
  ///   { 7, "Uranus" },
  ///   { 8, "Neptune" },
  ///   { 9, "Pluto"},
  ///   { 10, "Moon"},
  ///   { 11, "Rahu"},
  ///   { 12, "Ketu"}
  /// </summary>
  public static class AstrologyCalculations
  {
    /// <summary>
    ///   Returns ascendant integer value.
    ///   For instance 1 is ascendant between 0 and 30 degrees (Aries),
    ///   2 is ascendant between 30 and 60 degrees (Taurus).
    ///   For more details see https://en.wikipedia.org/wiki/Astrological_sign
    /// </summary>
    /// <param name="birthDateTime">UTC time at moment of birth</param>
    /// <param name="longitude">West are negative values, East are positive values</param>
    /// <param name="latitude">North are positive values, South are negative values</param>
    /// <param name="isVedic"></param>
    /// <returns></returns>
    public static int GetAscendant(DateTime birthDateTime, double longitude, double latitude, bool isVedic = true)
    {
      if (CoordinatesInvalid()) throw new ArgumentOutOfRangeException("Invalid longitude");

      var asc = AscendantCalculations.GetAscendant(longitude, latitude, birthDateTime, isVedic);
      if (isVedic)
      {
        var shift = CommonCalculations.GetVedicShift(birthDateTime);
        asc -= shift;
      }
#if DEBUG
      Console.WriteLine(asc);
#endif
      return (int) asc / 30 + 1;

      bool CoordinatesInvalid()
      {
        return longitude > 180 || longitude < -180 || latitude > 90 || latitude < -90;
      }
    }

    /// <summary>
    ///   Returns two objects: HousesPlanets and PlanetDegrees.
    ///   HousesPlanets is list of 12 int arrays.
    ///   Each array corresponds to astrology one House.
    ///   Int values in arrays corresponds to planets numbers that are placed in this House.
    ///   PlanetDegrees is list of (int, decimal) Pairs.
    ///   Int value is planet number and decimal value represents longitude value in House.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="isVedic"></param>
    /// <param name="HousesPlanets"></param>
    /// <param name="PlanetDegrees"></param>
    /// <returns></returns>
    public static void GetHousesAndDegrees(DateTime dateTime, double longitude, double latitude,
      out List<int[]> HousesPlanets, out List<Pair<int, decimal>> PlanetDegrees,
      bool isVedic = true)
    {
      PlanetManager.GetInstance()
        .CalculatePositions(dateTime, longitude, latitude, isVedic, out HousesPlanets, out PlanetDegrees);
    }

    /// <summary>
    ///   Get longitudes for all planets.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="longitude"></param>
    /// <param name="latitude"></param>
    /// <returns></returns>
    public static List<decimal> GetPlanetsDegrees(DateTime dateTime, double longitude, double latitude)
    {
      return PlanetManager.GetInstance().CalculatePositions(dateTime, longitude, latitude);
    }
  }
}