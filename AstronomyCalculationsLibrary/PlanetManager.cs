using System;
using System.Collections.Generic;
using System.Linq;
using AstronomyCalculationsLibrary.CelestialObjects;
using AstronomyCalculationsLibrary.Common;

namespace AstronomyCalculationsLibrary
{
  /// <summary>
  /// Class manages planet orbital properties and triggers calculating of positions.
  /// Is meant to has single instance. To get instance use GetInstance().
  /// </summary>
  public class PlanetManager
  {
    private static PlanetManager Instance = null; // Singleton

    /// <summary>
    /// Contains following: Sun, Mercury, Venus, Mars, Jupiter, Saturn, Uranus, Neptune Moon, Pluto, Rahu, Ketu
    /// </summary>
    private List<ICelestialPosition> PlanetPositions = new List<ICelestialPosition>();

    /// <summary>
    /// Creates instance of PlanetManager
    /// </summary>
    /// <returns></returns>
    public static PlanetManager GetInstance()
    {
      if (Instance is null)
      {
        Instance = new PlanetManager();
      }
      return Instance;
    }

    /// <summary>
    /// Returns CelestialPosition object if found or null.
    /// Test purposes
    /// </summary>
    /// <param name="planetName"></param>
    /// <returns></returns>
    public CelestialPosition GetPlanet(string planetName)
    {
      if (planetName == "Earth")
      {
        return EarthPosition.EarthInstance;
      }

      var output = PlanetPositions.FirstOrDefault(x => x.Name == planetName);
      if (output is null)
      {
        throw new ArgumentException($"Planet {planetName} not found.", nameof(planetName));
      }
      if (!(output is CelestialPosition) && output is ICelestialPosition)
      {
        throw new InvalidCastException("Given planet is not CelestialPosition type.");
      }

      return (CelestialPosition)output;
    }


    /// <summary>
    /// Not accesible ctor. Use GetInstance() instead.
    /// </summary>
    private PlanetManager()
    {
      foreach (var p in PlanetInfoList)
      {
        var longAscNode = GetLinearFuncDegreesPositive(p.LongAscendNode, p.LongAscendNodeDelta);
        var inclination = GetLinearFuncDegrees(p.Inclination, p.InclinationDelta);
        var peryhelion = GetLinearFuncDegreesPositive(p.Peryhelion, p.PeryhelionDelta);
        var eccent = GetLinearFunc(p.Eccent, p.EccentDelta);
        var meanAnomaly = GetLinearFuncDegreesPositive(p.MeanAnomaly, p.MeanAnomalyDelta);
        var semimajor = GetLinearFunc(p.SemimajorAxis, p.SemimajorAxisDelta);

        if (p.Name == "Sun")
        {
          PlanetPositions.Add(
              new SunPosition(p.Name, longAscNode, inclination, peryhelion, eccent, meanAnomaly, semimajor));
        }
        else if (p.Name == "Earth")
        {
          new EarthPosition(p.Name, longAscNode, inclination, peryhelion, eccent, meanAnomaly, semimajor); // Do not add Earth to PlanetPositions list.
        }
        else
        {
          PlanetPositions.Add(
              new PlanetPosition(p.Name, longAscNode, inclination, peryhelion, eccent, meanAnomaly, semimajor));
        }
      }

      // Following elements are calculated using unique methods
      PlanetPositions.Add(new PlutoPosition("Pluto"));
      PlanetPositions.Add(new MoonPosition("Moon"));
      PlanetPositions.Add(new RahuPosition("Rahu"));
      PlanetPositions.Add(new KetuPosition("Ketu"));


      Func<decimal, decimal> GetLinearFunc(decimal value, decimal delta)
      {
        return d => value + delta * d;
      }

      Func<decimal, decimal> GetLinearFuncDegreesPositive(decimal value, decimal delta)
      {
        return d =>
        {
          var output = value + delta * d;
          output %= 360;
          if (output < 0)
          {
            output += 360;
          }
          return output;
        };
      }

      Func<decimal, decimal> GetLinearFuncDegrees(decimal value, decimal delta)
      {
        return d =>
        {
          var output = value + delta * d;
          output %= 360;
          return output;
        };
      }
    }

    public List<decimal> CalculatePositions(DateTime date, double longitude, double latitude)
    {
      var numberToResult = new Dictionary<int, decimal>
      {
          { 0, new AscendantCalculations().GetAscendant(longitude, latitude, date) }
      };

      var result = new List<decimal>();

      foreach (var planet in PlanetPositions)
      {
        numberToResult.Add(planetToNumbers[planet.Name], planet.EclipticCoordinates(date).X);
      }

      foreach (var entry in planetToNumbers.Values)
      {
        result.Add(numberToResult[entry]);
      }
      return result;
    }

    /// <summary>
    /// Returns dictionary translating int Houses to lists of tuples (int Planet, decimal Degree)
    /// Degrees values are ecliptical longitudes (longitudes % 30).
    /// Complete longitude value may be retrieved from correspodning House number.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="longitude"></param>
    /// <param name="latitude"></param>
    /// <param name="isVedic"></param>
    /// <param name="HousesPlanets">12 item list. Each item corresponds to one House. Each item contains array of numbers of planets</param>
    /// <param name="PlanetDegrees">List of planets with longitudes</param>
    /// <returns></returns>
    public void CalculatePositions(DateTime date, double longitude, double latitude, bool isVedic,
        out List<int[]> HousesPlanets, out List<Pair<int, decimal>> PlanetDegrees)
    {
      // Initialize output lists
      List<int[]> housesPlanets;
      List<Pair<int, decimal>> planetDegrees;
      InitializeLists();

      // Calculate vedic shift
      var vedicShift = isVedic ? CommonCalculations.GetVedicShift(date) : 0m;

      // Add ascendant to output list
      AddAscendant();

      // Add planets to outputlist
      AddPlanets();

      // Assign results to output references
      HousesPlanets = housesPlanets;
      PlanetDegrees = planetDegrees;

      // Local functions
      void InitializeLists()
      {
        housesPlanets = new List<int[]>();
        for (var i = 0; i < 12; i++)
        {
          housesPlanets.Add(new int[] { });
        }
        planetDegrees = new List<Pair<int, decimal>>(PlanetPositions.Count);
      }

      void AddAscendant()
      {
        var asc = new AscendantCalculations().GetAscendant(longitude, latitude, date);
        if (isVedic)
        {
          asc -= vedicShift;
        }
        From0_To360(ref asc);
        var house = (int)(asc / 30);
        var currentArray = housesPlanets[house];
        var newArray = new int[currentArray.Length + 1];
        currentArray.CopyTo(newArray, 0);
        newArray[currentArray.Length] = planetToNumbers["Ascendant"];
        housesPlanets[house] = newArray;

        planetDegrees.Add(new Pair<int, decimal>(0, asc % 30));
      }

      void AddPlanets()
      {
        foreach (var planet in PlanetPositions)
        {
          var planetNumber = planetToNumbers[planet.Name];
          var eclLon = planet.EclipticCoordinates(date).X;

          From0_To360(ref eclLon);
          if (isVedic)
          {
            eclLon -= vedicShift;
          }
          From0_To360(ref eclLon);
#if DEBUG
          Console.WriteLine($"{planet.Name} degrees: {eclLon}\n\n\n\n");
#endif
          var house = (int)(eclLon / 30);

          var currentArray = housesPlanets[house];
          var newArray = new int[currentArray.Length + 1];
          currentArray.CopyTo(newArray, 0);
          newArray[currentArray.Length] = planetToNumbers[planet.Name];
          housesPlanets[house] = newArray;

          planetDegrees.Add(new Pair<int, decimal>(planetNumber, eclLon % 30));
        }
      }

      // degrees should be in [0,360)
      void From0_To360(ref decimal deg)
      {
        deg %= 360;
        deg += 360;
        deg %= 360;
      }
    }

    public readonly Dictionary<string, int> planetToNumbers = new Dictionary<string, int>
        {
            { "Ascendant", 0},
            { "Sun",     1 },
            { "Mercury", 2 },
            { "Venus",   3 },
            { "Mars",    4 },
            { "Jupiter", 5 },
            { "Saturn",  6 },
            { "Uranus",  7 },
            { "Neptune", 8 },
            { "Pluto",   9 },
            { "Moon",    10},
            { "Rahu",    11},
            { "Ketu",    12},

        };

    private readonly List<PlanetInfo> PlanetInfoList = new List<PlanetInfo>
        {

            new PlanetInfo(/* Name */ "Earth", /* AscNode */ 0m, 0m, /* Inclination */ -0.00001531m, -3.5446078e-7m, /* ArgPery */ 282.93768193m, 0.00000885074m,
                /* Eccent */ 0.01671123m, -1.20246407e-9m, /* Mean anomaly */ 357.52688973m, 0.98560025124m, /* Axis */ 1.00000261m, 1.5386721e-10m),

            new PlanetInfo("Sun",      0m, 0m, 0m, 0m, 282.9404m, 4.70935e-5m, 0.016709m, -1.151e-9m, 356.0470m, 0.9856002585m, 1m),

            new PlanetInfo(/* Name */ "Mercury", /* AscNode */ 48.33076593m, -0.00000343164m, /* Inclination */ 7.00497902m, -1.62833402e-7m, /* ArgPery */ 29.12703035m, 0.00000782525m,
                /* Eccent */ 0.20563593m, 5.2183436e-10m, /* Mean anomaly */ 174.79252722m, 4.09233439111m, /* Axis */ 0.38709927m, 1.0130048e-11m),

            new PlanetInfo(/* Name */ "Venus", /* AscNode */ 76.67984255m, -0.00000760285m, /* Inclination */ 3.39467605m, -2.15989049e-8m, /* ArgPery */ 54.92262463m, 0.00000767631m,
                /* Eccent */ 0.00677672m, -1.12443532e-9m, /* Mean anomaly */ 50.37663232m, 1.60213039573m, /* Axis */ 0.72333566m, -1.0677618e-10m),

            new PlanetInfo(/* Name */ "Mars", /* AscNode */ 49.55953891m, -0.00000801022m, /* Inclination */ 1.84969142m, -2.22623135e-7m, /* ArgPery */ 286.4968315m, 0.00002017753m,
                /* Eccent */ 0.09339410m, 2.15797399e-9m, /* Mean anomaly */ 19.39019754m, 0.52402076m, /* Axis */ 1.52371034m, 5.13073E-10m),

            new PlanetInfo(/* Name */ "Jupiter", /* AscNode */ 100.4739091m, 5.60414E-06m, /* Inclination */ 1.30439695m, -5.02982E-08m, /* ArgPery */ 274.2545707m, 2.14528E-07m,
                /* Eccent */ 0.04838624m, -3.62847E-09m, /* Mean anomaly */ 19.66796068m, 0.083081002m, /* Axis */ 5.202887m, -3.17782E-09m),

            new PlanetInfo(/* Name */ "Saturn", /* AscNode */ 113.66242448m, -0.00000790357125256674m, /* Inclination */ 2.48599187m, 0.00000005295249828884m, /* ArgPery */ 338.93645383m, -0.00000356726132785763m,
                /* Eccent */ 0.05386179m, -0.00000013960574948665m, /* Mean anomaly */ 317.35536592m, 0.0334815220854209m, /* Axis */ 9.53667594m, -0.00000003423956194387m),

            new PlanetInfo(/* Name */ "Uranus", /* AscNode */ 74.01692503m, 1.16100999315537E-06m, /* Inclination */ 0.77263783m, 6.65130732375086E-08m, /* ArgPery */ 96.93735127m, 1.00108670773443E-05m,
                /* Eccent */ 0.04725744m, -1.20383299110198E-08m, /* Mean anomaly */ 142.28382821m, 1.17200266951403E-02m, /* Axis */ 19.18916464m, -5.37100616016427E-08m),

            new PlanetInfo(/* Name */ "Neptune", /* AscNode */ 131.78422574m, -1.39264613278576E-06m, /* Inclination */ 1.77004347m, 9.68432580424367E-09m, /* ArgPery */ 273.18053653m, -7.43458562628337E-06m,
                /* Eccent */ 0.00859048m, 1.39767282683094E-09m, /* Mean anomaly */ 259.91520804m, 5.98992109212868E-03m, /* Axis */ 30.06992276m, 2.35194524298426E-07m),
        };

    /// <summary>
    /// Struct used to contain all orbital properties.
    /// </summary>
    protected struct PlanetInfo
    {
      public string Name { get; }

      // Longitude of asceding node = N
      public decimal LongAscendNode { get; }
      public decimal LongAscendNodeDelta { get; }

      // Inclination = i
      public decimal Inclination { get; }
      public decimal InclinationDelta { get; }

      //Argument of Peryhelion = omega
      public decimal Peryhelion { get; }
      public decimal PeryhelionDelta { get; }

      //Eccentricity = e
      public decimal Eccent { get; }
      public decimal EccentDelta { get; }

      //Mean anomaly = M
      public decimal MeanAnomaly { get; }
      public decimal MeanAnomalyDelta { get; }

      // Semimajor Axis = A;
      public decimal SemimajorAxis { get; }
      public decimal SemimajorAxisDelta { get; }

      /// <summary>
      /// 
      /// </summary>
      public PlanetInfo(string name, decimal N, decimal NDelta, decimal i, decimal iDelta, decimal omega, decimal omegaDelta,
          decimal e, decimal eDelta, decimal M, decimal MDelta, decimal A, decimal Adelta = 0)
      {
        Name = name;

        LongAscendNode = N;
        LongAscendNodeDelta = NDelta;

        Inclination = i;
        InclinationDelta = iDelta;

        Peryhelion = omega;
        PeryhelionDelta = omegaDelta;

        Eccent = e;
        EccentDelta = eDelta;

        MeanAnomaly = M;
        MeanAnomalyDelta = MDelta;

        SemimajorAxis = A;
        SemimajorAxisDelta = Adelta;
      }
    }
  }
}
