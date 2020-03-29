using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AstronomyCalculationsLibrary.Tests
{
    public class AstrologyCalculationsTests
    {
        public class GetPlanetsDegrees_Data : IEnumerable<object[]>
        {
            private List<object[]> data = new List<object[]>
            {
                new object[] {new DateTime(2010, 1, 1, 0, 0, 0), -0.118092, 51.509865,
                    new decimal[] {
                        187.35m,
                        280.451m,
                        288.953m,
                        277.849m,
                        138.817m,
                        326.358m,
                        184.506m,
                        353.090m,
                        324.582m,
                        273.308m,
                        103.244m,
                        291.633m,
                        111.633m
                    } },
                new object[] {new DateTime(1920, 6, 18, 0, 0, 0), -0.118092, 51.509865,
                    new decimal[] {
                        349.9666667m,
                        115.0416667m,
                        129.5333333m,
                        118.95m,
                        213.0333333m,
                        141.3666667m,
                        218.8833333m,
                        335.1333333m,
                        130.65m,
                        97.73333333m,
                        145.05m,
                        221.85m,
                        41.85m,
                    } }
            };

            public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(GetPlanetsDegrees_Data))]
        public void GetPlanetsDegrees_Success(DateTime date, double longitude, double latitude, decimal[] expected)
        {
            var actual = AstrologyCalculations.GetPlanetsDegrees(date, longitude, latitude);
            var epsilon = 0.25m;
            if(actual.Count != expected.Count())
            {
                throw new ArgumentOutOfRangeException();
            }
            for(var i = 0; i < actual.Count; i++)
            {
                Assert.InRange(actual[0], expected[0] - epsilon, expected[0] + epsilon);
            }
        }
    }
}
