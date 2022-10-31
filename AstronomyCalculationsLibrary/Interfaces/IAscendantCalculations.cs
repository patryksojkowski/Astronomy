using System;

namespace AstronomyCalculationsLibrary.Interfaces
{
  public interface IAscendantCalculations
  {
    decimal GetAscendant(double longitude, double latitude, DateTime dateTime);
  }
}
