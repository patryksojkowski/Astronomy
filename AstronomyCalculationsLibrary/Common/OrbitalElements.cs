namespace AstronomyCalculationsLibrary.Common
{
  public struct OrbitalElements
  {
    public decimal LongAscedingNode;
    public decimal Inclination;
    public decimal Peryhelion;
    public decimal Eccentricity;
    public decimal MeanAnomaly;
    public decimal SemimajorAxis;

    public OrbitalElements(decimal _longAscendingNode, decimal _inclination, decimal _peryhelion, decimal _eccentricity,
      decimal _meanAnomaly, decimal _semimajorAxis)
    {
      LongAscedingNode = _longAscendingNode;
      Inclination = _inclination;
      Peryhelion = _peryhelion;
      Eccentricity = _eccentricity;
      MeanAnomaly = _meanAnomaly;
      SemimajorAxis = _semimajorAxis;
    }
  }
}