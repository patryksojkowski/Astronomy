using System;
using AstronomyCalculationsLibrary.CelestialObjects;
using Xunit;

namespace AstronomyCalculationsLibrary.Tests
{
    public class PlanetManagerTests
    {
        //SHOULD BE MOVED TO DIFFERENT CLASS SINCE IT TESTS PlanetManager
        [Fact]
        public void GetPlanet_PlanetNotCelestialPosition_Fail()
        {
            Assert.Throws<InvalidCastException>(() => PlanetManager.GetInstance().GetPlanet("Rahu"));
        }

        [Fact]
        public void GetPlanet_PlanetIsEarth_Success()
        {
            var earth = PlanetManager.GetInstance().GetPlanet("Earth");
            Assert.Same(earth, EarthPosition.EarthInstance);
        }
    }
}
