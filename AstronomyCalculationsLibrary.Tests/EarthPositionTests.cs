using System;
using System.Collections;
using System.Collections.Generic;
using AstronomyCalculationsLibrary.Common;
using Xunit;

namespace AstronomyCalculationsLibrary.Tests
{
    public class EarthPositionTests
    {
        #region
        public class EclipticSun3D_Data : IEnumerable<object[]>
        {
            List<object[]> data = new List<object[]>
            {
                new object[] { new DateTime(1950, 03, 26, 10, 0, 0), new Vector3<decimal> (-9.924468716825268E-01m, -1.022711968523091E-01m, -2.348907200477060E-05m) },
                new object[] { new DateTime(2019, 11, 09, 0, 0, 0), new Vector3<decimal> (6.880273757010908E-01m, 7.127705882445894E-01m, -3.146936884169265E-05m) },
            };

            public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(EclipticSun3D_Data))]
        public void EclipticSun3D_Success(DateTime date, Vector3<decimal> expected)
        {
            // arrange
            var earth = PlanetManager.GetInstance().GetPlanet("Earth");

            // act
            var actual = earth.EclipticSun3D(date);

            //assert
            Assert.True(Vector3<decimal>.IsClose(actual, expected, 0.001m), $"actual: {actual}, expected: {expected}, distance: {Vector3<decimal>.Distance(actual, expected)}");
        }

        #endregion
    }
}
