using System;
using System.Collections.Generic;
using AstronomyCalculationsLibrary.Common;
using Decimal;
using Xunit;
using System.Collections;

namespace AstronomyCalculationsLibrary.Tests
{
    public class PlanetPositionTests
    {
        #region ApparentEclipticEarth
        public class ApparentEclipticEarth_Data : IEnumerable<object[]>
        {
            private readonly List<object[]> data = new List<object[]>
            {
                new object[] { new DateTime(2019, 11, 09, 0, 0, 0), "Jupiter",
                    new Vector2<decimal> (264.9289488m, 0.1782023m), 1m},
                new object[] { new DateTime(1992, 10, 17, 12, 0, 0), "Mars",
                    new Vector2<decimal> (107.6563793m, 0.8095542m)},
                new object[] { new DateTime(1950, 03, 26, 10, 0, 0), "Venus",
                    new Vector2<decimal> (319.9764698m, 2.6924449m), 1m},
            };

            public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(ApparentEclipticEarth_Data))]
        public void ApparentEclipticEarth_Success(DateTime date, string planetName, Vector2<decimal> expected, decimal epsilon = 0.1m)
        {
            // arrange
            var planet = PlanetManager.GetInstance().GetPlanet(planetName);

            // act
            var actual = planet.ApparentEclipticEarth(date);

            // assert
            Assert.True(Vector2<decimal>.IsClose(actual, expected, epsilon), $"actual {actual}, expected:{expected}");
        }

        [Fact]
        public void ApparentEclipticEarth_PlanetIsSun_Fail()
        {
            Assert.Throws<NotImplementedException>(() =>
           PlanetManager.GetInstance().GetPlanet("Sun").ApparentEclipticEarth(DateTime.Now));
        }
        #endregion ApparentEclipticEarth

        #region EclipticSun3D
        public class EclipticSun3D_Data : IEnumerable<object[]>
        {
            private readonly List<object[]> data = new List<object[]>
            {
                new object[] { new DateTime(1950, 03, 26, 10, 0, 0), "Venus",
                    new Vector3<decimal> (-5.604402137327833E-01m, -4.561862525317852E-01m, 2.619977536364409E-02m)},
                new object[] { new DateTime(1906, 06, 06, 10, 53, 0), "Mercury",
                    new Vector3<decimal> (1.396789958619602E-01m, 2.755596207756046E-01m, 9.622102442285804E-03m)},
                new object[] { new DateTime(1910, 06, 06, 10, 58, 0), "Saturn",
                    new Vector3<decimal> (8.087150261064014E+00m, 4.535109254095826E+00m, -4.007631219065616E-01m), 0.1m},
                new object[] { new DateTime(2019, 11, 9, 0, 0, 0), "Jupiter",
                    new Vector3<decimal> (1.315223271183669E-01m,-5.245076138127984E+00m, 1.884319806181774E-02m), 0.01m},
                new object[] { new DateTime(2020, 4, 10, 4, 2, 0), "Neptune",
                    new Vector3<decimal> (2.930676087468773E+01m, -6.057471197046284E+00m, -5.507493324541572E-01m), 0.01m},
            };

            public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(EclipticSun3D_Data))]
        public void EclipticSun3D_Success(DateTime date, string planetName, Vector3<decimal> expected, decimal epsilon = 0.001m)
        {
            //arrange
            var planet = PlanetManager.GetInstance().GetPlanet(planetName);

            // act
            var actual = planet.EclipticSun3D(date);

            // assert
            Assert.True(Vector3<decimal>.IsClose(actual, expected, epsilon), $"actual {actual}, expected:{expected}, distance: {Vector3<decimal>.Distance(actual, expected)}");
        }
        #endregion

        #region EquatorialCoordinates

        public class EquatorialCoordinates_Data : IEnumerable<object[]>
        {
            List<object[]> data = new List<object[]>
            {
                new object [] { new DateTime(2010, 10, 10, 10, 10, 0), "Venus", new Vector2<decimal> (218.179041666m, -22.62331m) },
                new object [] { new DateTime(1960, 1, 2, 20, 12, 0), "Neptune", new Vector2<decimal> (217.464416667m, -12.93997m) },
                new object [] { new DateTime(1950, 3, 26, 10, 0, 0), "Venus", new Vector2<decimal> (322.197875m, -12.04925m) },
                new object [] { new DateTime(2019, 11, 9, 0, 0, 0), "Jupiter", new Vector2<decimal> (264.1925m, -23.1509444444m) },
                new object [] { new DateTime(1910, 06, 06, 10, 58, 0), "Saturn", new Vector2<decimal> (32.089125m, 10.53225m), 0.25m },
            };

            public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(EquatorialCoordinates_Data))]
        public void EquatorialCoordinates_Success(DateTime date, string planetName, Vector2<decimal> expected, decimal epsilon = 0.05m)
        {
            // arrange & act
            var actual = PlanetManager.GetInstance()
                .GetPlanet(planetName)
                .EquatorialCoordinates(date);

            // assert
            Assert.True(Vector2<decimal>.IsClose(actual, expected, epsilon), $"actual {actual}, expected:{expected}");
        }
        #endregion

        #region OrbitalElements
        public class OrbitalElements_Data : IEnumerable<object []>
        {
            List<object[]> data = new List<object[]>
            {
                //new object [] {"yyyy-MM-dd hh:ss", "planetName", new OrbitalElements(0m, 0m, 0m, 0m, 0m, 0m) },
                new object [] { new DateTime(1910, 06, 06, 10, 58, 0), "Saturn",
                                new OrbitalElements(1.140048050818241E+02m, 2.485487912903695E+00m, 3.405116589899136E+02m, 5.426464727732826E-02m, 3.003109291364364E+02m, 9.519579575023517E+00m) },
                new object [] { new DateTime(2000, 03, 26, 20, 52, 0), "Venus",
                                new OrbitalElements(7.667832200392074E+01m, 3.394593200445165E+00m, 5.517187141942911E+01m, 6.765981781101608E-03m, 1.869027290405515E+02m, 7.233252052995509E-01m) },
                new object [] { new DateTime(2020, 4, 10, 4, 2, 0), "Neptune",
                                new OrbitalElements(1.318291620845057E+02m, 1.772446442149253E+00m, 2.466860817788901E+02m, 1.025759916225860E-02m, 3.304067478723460E+02m, 3.019987980963154E+01m) }
            };

            public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(OrbitalElements_Data))]
        public void OrbitalElemets_Success(DateTime date, string planetName, OrbitalElements expected)
        {
            var epsilon = 2m;
            // arrange & act
            var actual = PlanetManager.GetInstance().GetPlanet(planetName).GetOrbitalElements(date);

            // assert
            string output = string.Empty;
            output += !DecimalMath.IsClose(actual.LongAscedingNode, expected.LongAscedingNode, epsilon) ? $"\nLongAscendingNode\texpected : {expected.LongAscedingNode} actual {actual.LongAscedingNode}" : string.Empty;
            output += !DecimalMath.IsClose(actual.Inclination, expected.Inclination, epsilon) ? $"\nInclination\t\texpected : {expected.Inclination} actual {actual.Inclination}" : string.Empty;
            output += !DecimalMath.IsClose(actual.Peryhelion, expected.Peryhelion, epsilon) ? $"\nPeryhelion\t\texpected : {expected.Peryhelion} actual {actual.Peryhelion}" : string.Empty;
            output += !DecimalMath.IsClose(actual.Eccentricity, expected.Eccentricity, epsilon) ? $"\nEccentricity\t\texpected : {expected.Eccentricity} actual {actual.Eccentricity}" : string.Empty;
            output += !DecimalMath.IsClose(actual.MeanAnomaly, expected.MeanAnomaly, epsilon) ? $"\nMeanAnomaly\t\texpected : {expected.MeanAnomaly} actual {actual.MeanAnomaly}" : string.Empty;
            output += !DecimalMath.IsClose(actual.SemimajorAxis, expected.SemimajorAxis, epsilon) ? $"\nSemimajorAxis\t\texpected : {expected.SemimajorAxis} actual {actual.SemimajorAxis}" : string.Empty;
            Assert.True(string.IsNullOrEmpty(output), output);
        }

        #endregion
    }
}
