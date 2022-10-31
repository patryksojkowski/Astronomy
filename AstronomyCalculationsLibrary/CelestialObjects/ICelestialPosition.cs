using System;
using AstronomyCalculationsLibrary.Common;

namespace AstronomyCalculationsLibrary.CelestialObjects
{
  internal interface ICelestialPosition
  {
    string Name { get; set; }

    /// <summary>
    ///   Returns Vector2, where X = RA, Y = Dec
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    Vector2<decimal> EquatorialCoordinates(DateTime date);

    /// <summary>
    ///   Returns Vector2, where X = EclLon, Y = EclLat
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    Vector2<decimal> EclipticCoordinates(DateTime date);

    /// <summary>
    /// Returns Vector2, where X = AppEclLon, Y =AppEclLat
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    //Vector2<decimal> ApparentEclipticCoordinates(DateTime date);
  }
}