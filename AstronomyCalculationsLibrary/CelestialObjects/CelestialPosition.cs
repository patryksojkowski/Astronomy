using System;
using AstronomyCalculationsLibrary.Common;

namespace AstronomyCalculationsLibrary.CelestialObjects
{
    /// <summary>
    /// Base class for Positions classes
    /// </summary>
    public abstract class CelestialPosition : ICelestialPosition
    {
        public string Name { get; set; }
        public Func<decimal, decimal> LongAscedingNode { get; set; }
        public Func<decimal, decimal> Inclination { get; set; }
        public Func<decimal, decimal> Peryhelion { get; set; }
        public Func<decimal, decimal> Eccentricity { get; set; }
        public Func<decimal, decimal> MeanAnomaly { get; set; }
        public Func<decimal, decimal> SemimajorAxis { get; set; }

        public CelestialPosition(string name, Func<decimal, decimal> ascedningNode, Func<decimal, decimal> inclination, Func<decimal, decimal> peryhelion,
            Func<decimal, decimal> eccentricity, Func<decimal, decimal> meanAnomaly, Func<decimal, decimal> semimajorAxis)
        {
            Name = name;
            LongAscedingNode = ascedningNode;
            Inclination = inclination;
            Peryhelion = peryhelion;
            Eccentricity = eccentricity;
            MeanAnomaly = meanAnomaly;
            SemimajorAxis = semimajorAxis;
        }

        /// <summary>
        /// Returns OrbitalElements struct with values for given DateTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public OrbitalElements GetOrbitalElements(DateTime date)
        {
            var d = JulianDateCalculator.ToJulianDaysJ2000(date);
            return new OrbitalElements(LongAscedingNode(d), Inclination(d), Peryhelion(d), Eccentricity(d), MeanAnomaly(d), SemimajorAxis(d));
        }

        /// <summary>
        /// Calculate Earth-centered Right Ascesion and Declination
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public abstract Vector2<decimal> EquatorialCoordinates(DateTime date);

        /// <summary>
        /// Calculate Earth-centered Ecliptic latitude and longitude
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public abstract Vector2<decimal> EclipticCoordinates(DateTime date);

        /// <summary>
        /// Calculate Ecliptic 3D coordinates with origin set to Earth
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public abstract Vector3<decimal> EclipticEarth3D(DateTime date);

        /// <summary>
        /// Calculate Ecliptic 3D coordinates with origin set to Sun
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public abstract Vector3<decimal> EclipticSun3D(DateTime date);

        /// <summary>
        /// Calculate apparent Ecliptic 3D coorinates with origin set to Earth
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public abstract Vector3<decimal> ApparentEclipticEarth3D(DateTime date);
        
        /// <summary>
        /// Calculate apparent latitude and longitude with origin set to Earth
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public abstract Vector2<decimal> ApparentEclipticEarth(DateTime date);
    }
}
