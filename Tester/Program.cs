using System;
using System.Collections.Generic;
using AstronomyCalculationsLibrary;
using AstronomyCalculationsLibrary.Common;

namespace Tester
{
    class Tester
    {
        public static readonly Dictionary< int, string> planetToNumbers = new Dictionary<int, string>
        {
            { 0, "Ascendant" },
            { 1, "Sun"    },
            { 2, "Mercury"},
            { 3, "Venus"  },
            { 4, "Mars"},
            { 5, "Jupiter"},
            { 6, "Saturn"  },
            { 7, "Uranus" },
            { 8, "Neptune" },
            { 9, "Pluto"},
            { 10, "Moon"},
            { 11, "Rahu"},
            { 12, "Ketu"}
        };

        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
            //Test1();
            Test2();
        }
        
        private static void Test2()
        {
            var date = Convert.ToDateTime("1/1/2010 12:00:00 am");
            Console.WriteLine(date.ToLongDateString() + date.ToLongTimeString());
            var houses = new List<int[]>();
            var planets = new List<Pair<int, decimal>>();
            var longitude = -0.11;
            var latitude = 51.51;
            AstrologyCalculations.GetHousesAndDegrees(date, longitude, latitude, out houses, out planets, false);
            var house = 0;
            foreach(var planetArray in houses)
            {
                Console.WriteLine($"House:{house}");
                for(var  i = 0; i < planetArray.Length; i++)
                {
                    var planetNumber = planetArray[i];
                    foreach(var tuple in planets)
                    {
                        if(tuple.X == planetNumber)
                        {
                            var name = planetToNumbers[tuple.X];
                            Console.WriteLine($"Planet {name} Degrees {tuple.Y}");
                        }
                    }
                }
                house++;
            }

            Console.ReadKey();
        }

        private static void Test1()
        {
            var BDate = Convert.ToDateTime("14/10/1994 12:15:00 AM");
            var longtitude = -72.6190;
            var lattitude = 22.3181;
            var Asc = AstrologyCalculations.GetAscendant(BDate, longtitude, lattitude);
            Console.WriteLine(Asc);

            ////Mehul InCorrect Asc = 5 instead of 4 | 03 / 09 / 1997 05:10:00 AM(IST)
            BDate = Convert.ToDateTime("02/09/1997 11:40:00 PM");
            longtitude = -72.86;
            lattitude = 22.69;
            Asc = AstrologyCalculations.GetAscendant(BDate, longtitude, lattitude);
            Console.WriteLine(Asc);

            //Mehul InCorrect Asc = 5 instead of 4 | 03 / 09 / 1997 05:10:00 AM(IST)
            BDate = Convert.ToDateTime("02/09/1997 11:40:00 PM");
            longtitude = -72.86;
            lattitude = 22.69;
            Asc = AstrologyCalculations.GetAscendant(BDate, longtitude, lattitude);
            Console.WriteLine(Asc);

            //Arun Correct Asc=8
            BDate = Convert.ToDateTime("22/06/1987 11:20:00 AM");
            longtitude = -72.86;
            lattitude = 22.69;
            Asc = AstrologyCalculations.GetAscendant(BDate, longtitude, lattitude);
            Console.WriteLine(Asc);

            //Mehul InCorrect Asc=5 instead of 4
            BDate = Convert.ToDateTime("02/09/1997 11:40:00 PM");
            longtitude = -72.86;
            lattitude = 22.69;
            Asc = AstrologyCalculations.GetAscendant(BDate, longtitude, lattitude);
            Console.WriteLine(Asc);

            //Vaibhavi InCorrect Asc=3 instead of 2
            BDate = Convert.ToDateTime("25/11/1996 1:00:00 PM");
            longtitude = -72.86;
            lattitude = 22.69;
            Asc = AstrologyCalculations.GetAscendant(BDate, longtitude, lattitude);
            Console.WriteLine(Asc);

            //Raj Desai InCorrect Asc=5 instead of 4
            BDate = Convert.ToDateTime("25/06/1988 03:50:00 AM");
            longtitude = -72.8777;
            lattitude = 19.0760;
            Asc = AstrologyCalculations.GetAscendant(BDate, longtitude, lattitude);
            Console.WriteLine(Asc);

            // Arun Correct Asc = 8 | 22 / 06 / 1987 16:50:00 PM(IST)
            BDate = Convert.ToDateTime("22/06/1987 11:20:00 AM");
            longtitude = -72.86;
            lattitude = 22.69;
            Asc = AstrologyCalculations.GetAscendant(BDate, longtitude, lattitude);
            Console.WriteLine(Asc);

            //Mehul InCorrect Asc=5 instead of 4 | 03/09/1997 05:10:00 AM(IST)
            BDate = Convert.ToDateTime("02/09/1997 11:40:00 PM");
            longtitude = -72.86;
            lattitude = 22.69;
            Asc = AstrologyCalculations.GetAscendant(BDate, longtitude, lattitude);
            Console.WriteLine(Asc);

            //Raj Desai InCorrect Asc=5 instead of 4 | 25/06/1988 09:20:00 AM(IST)
            BDate = Convert.ToDateTime("25/06/1988 03:50:00 AM");
            longtitude = -72.8777;
            lattitude = 19.0760;
            Asc = AstrologyCalculations.GetAscendant(BDate, longtitude, lattitude);
            Console.WriteLine(Asc);

            //Kevin Correct Asc=7 | 31/08/1962 03:00:00 PM (Florida)
            BDate = Convert.ToDateTime("31/08/1962 03:00:00 PM");
            longtitude = 85.6602;
            lattitude = 30.1588;
            Asc = AstrologyCalculations.GetAscendant(BDate, longtitude, lattitude);
            Console.WriteLine(Asc);
            Console.ReadKey();
        }

        //private static void AscendantTests()
        //{
        //    var date = new DateTime(2050, 1, 1, 0, 0, 0);
        //    //Eschele -6.89583, 52.21833
        //    //London -0.118092, 51.50
        //    for (var i = 0; i < 12; i++)
        //    {
        //        var asc = AscendantCalculations.GetAscendant(-6.9m, 52.2166666666667m, date);
        //        Console.WriteLine($"{date.ToShortDateString()} {asc}\n Degree of sign {Math.Floor(asc % 30)} {Math.Floor((asc % 30 - Math.Floor(asc % 30)) * 60)} \n");
        //        date = date.AddMonths(1);
        //    }
        //}




        //private static void SiderealTimeTests()
        //{
        //    // Berlin 2/2/1950 14:55:00
        //    var utcOffset = -1;
        //    var longitude = -13.41m;
        //    var dateTime = new DateTime(1950, 2, 2, 14, 55, 0).AddHours(utcOffset);
        //    Console.WriteLine(SiderealTime.Calculate(longitude, dateTime));


        //    // Belgium 01/12/2006 23:00:00
        //    utcOffset = -1;
        //    longitude = -5m;
        //    dateTime = new DateTime(2006, 12, 1, 23, 0, 0).AddHours(utcOffset);
        //    Console.WriteLine(SiderealTime.Calculate(longitude, dateTime));

        //    // Warsaw now
        //    Console.WriteLine(SiderealTime.Calculate(-21.03m));

        //    // Rio de Janeiro now
        //    Console.WriteLine(SiderealTime.Calculate(43.28m));

        //}
    }
}
