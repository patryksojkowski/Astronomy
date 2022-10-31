using System;
using System.Collections.Generic;
using AstronomyCalculationsLibrary.Common;
using Decimal;

namespace AstronomyCalculationsLibrary.CelestialObjects
{
  internal class MoonPosition : ICelestialPosition
  {
    private static readonly Func<decimal, decimal> ToRad = deg => deg * DecimalMath.Pi / 180;

    // Moon source: https://ntomasse.web.cern.ch/Notes/tomassetti_ams02algorithms.pdf
    private static readonly Func<decimal, decimal>
      FL = T => 218.3164477M
                + 481267.88123421M * T
                - 0.0015786M * DecimalMath.PowerN(T, 2)
                + DecimalMath.PowerN(T, 3) / 538841M
                - DecimalMath.PowerN(T, 4) / 65194000M;

    private static readonly Func<decimal, decimal>
      FMMoon = T => 134.9633964M
                    + 477198.8675055M * T
                    + 0.0087414M * DecimalMath.PowerN(T, 2)
                    - DecimalMath.PowerN(T, 3) / 24490000M;

    private static readonly Func<decimal, decimal>
      FF = T => 93.2720950M
                + 483202.0175233M * T
                - 0.0036539M * DecimalMath.PowerN(T, 2)
                - DecimalMath.PowerN(T, 3) / 3526000M
                + DecimalMath.PowerN(T, 4) / 863310000M;

    private static readonly Func<decimal, decimal>
      FD = T => 297.8501921M
                + 445267.114034M * T
                - 0.0018819M * DecimalMath.PowerN(T, 2)
                + DecimalMath.PowerN(T, 3) / 545868M
                - DecimalMath.PowerN(T, 4) / 113065000;

    private static readonly Func<decimal, decimal>
      FMSun = T => 357.5291092M
                   + 35999.0502909M * T
                   - 0.0001536M * DecimalMath.PowerN(T, 2)
                   + DecimalMath.PowerN(T, 3) / 24490000M;


    private static readonly Func<decimal, decimal>
      A1rad = T => (119.75M + 131.849m * T) * DecimalMath.DegToRad;

    private static readonly Func<decimal, decimal>
      A2rad = T => (53.09M + 479264.290M * T) * DecimalMath.DegToRad;

    private static readonly Func<decimal, decimal>
      AL = T => 3958M * DecimalMath.Sin(A1rad(T)) + 1962M * DecimalMath.Sin((FL(T) - FF(T)) * DecimalMath.DegToRad) +
                318M * DecimalMath.Sin(A2rad(T));

    private static readonly Func<decimal, decimal>
      FE = T => 1 - 0.002516M * T - 0.0000074M * T * T;

    //e^l_j
    private static readonly Func<decimal, int, decimal>
      Fe = (T, m) =>
      {
        if (m == 0) return 1M;
        var res = FE(T);
        if (Math.Abs(m) == 1)
          return res;
        if (Math.Abs(m) == 2)
          return res * res;
        throw new ArgumentOutOfRangeException();
      };

    private static readonly Func<decimal, PeriodicTerm, decimal>
      Fs = (T, terms) =>
      {
        return terms.d * ToRad(FD(T)) + terms.m * ToRad(FMSun(T)) + terms.m1 * ToRad(FMMoon(T)) +
               terms.f * ToRad(FF(T));
      };

    private static readonly List<PeriodicTerm> terms = new List<PeriodicTerm>();

    public MoonPosition(string name)
    {
      Name = name;
      Init();
    }

    public string Name { get; set; }

    public Vector2<decimal> EclipticCoordinates(DateTime date)
    {
      var DegToRad = DecimalMath.DegToRad;

      var T = JulianDateCalculator.ToJulianCenturiesJ2000(date);
      var L = CommonCalculations.From0To360(FL(T));
      var M = CommonCalculations.From0To360(FMMoon(T));
      var Mrad = M * DegToRad;
      var F = CommonCalculations.From0To360(FF(T));
      var Frad = F * DegToRad;

      var lambda = (L + 1e-6M * CalculateSumLongitude(date)).From0To360();
      var beta = 5.128m * DecimalMath.Sin(Frad);
#if DEBUG
      Console.WriteLine($"Julian centuries passed: {T}, L = {L}, M = {M}, F = {F}");
      Console.WriteLine($"Lambda: {lambda}, Beta : {beta}");
#endif


      return new Vector2<decimal>(lambda, beta);
    }

    public Vector2<decimal> EquatorialCoordinates(DateTime date)
    {
      var DegToRad = DecimalMath.DegToRad;

      var d = JulianDateCalculator.ToJulianDaysJ2000(date);
      var L = FL(d);
      var Lrad = L * DegToRad;
      var M = FMMoon(d);
      var Mrad = M * DegToRad;
      var F = FF(d);
      var Frad = F * DegToRad;

      var lambda = (L + 6.289M * DecimalMath.Sin(Mrad)) * DegToRad;
      var beta = 5.128m * DecimalMath.Sin(Frad) * DegToRad;
      var tilt = CommonCalculations.GetAxialTilt(date) * DegToRad;

      var sinLambda = DecimalMath.Sin(lambda);
      var cosLambda = DecimalMath.Cos(lambda);

      var sinBeta = DecimalMath.Sin(beta);
      var cosBeta = DecimalMath.Cos(beta);
      var tanBeta = DecimalMath.Tan(beta);

      var sinTilt = DecimalMath.Sin(tilt);
      var cosTilt = DecimalMath.Cos(tilt);

      var RA = DecimalMath.Atan2(sinLambda * cosTilt - tanBeta * sinTilt, cosLambda) * DecimalMath.RadToDeg;
      var Dec = DecimalMath.Asin(sinBeta * cosTilt + cosBeta * sinTilt * sinLambda) * DecimalMath.RadToDeg;

      return new Vector2<decimal>(RA.From0To360(), Dec);
    }

    public static decimal CalculateSumLongitude(DateTime date)
    {
      var T = JulianDateCalculator.ToJulianCenturiesJ2000(date);
      var sum = AL(T);
      foreach (var term in terms)
      {
        var e = Fe(T, term.m);
        var sinFs = DecimalMath.Sin(Fs(T, term));
        var component = e * term.S * sinFs;
        sum += component;
#if DEBUG
        Console.WriteLine($"Component no{term.no}\t{component}\te {e} \tLCoeff\t{e * term.S}\tSinFs{sinFs}");
#endif
      }
#if DEBUG
      Console.WriteLine($"Sum SIGMA_L = {sum}");
#endif
      return sum;
    }

    private static void Init()
    {
      terms.Add(new PeriodicTerm(1, 0, 0, 1, 0, 6288774));
      terms.Add(new PeriodicTerm(2, 2, 0, -1, 0, 1274027));
      terms.Add(new PeriodicTerm(3, 2, 0, 0, 0, 658314));
      terms.Add(new PeriodicTerm(4, 0, 0, 2, 0, 213618));
      terms.Add(new PeriodicTerm(5, 0, 1, 0, 0, -185116));
      terms.Add(new PeriodicTerm(6, 0, 0, 0, 2, -114332));
      terms.Add(new PeriodicTerm(7, 2, 0, -2, 0, 58793));
      terms.Add(new PeriodicTerm(8, 2, -1, -1, 0, 57066));
      terms.Add(new PeriodicTerm(9, 2, 0, 1, 0, 53322));
      terms.Add(new PeriodicTerm(10, 2, -1, 0, 0, 45758));
      terms.Add(new PeriodicTerm(11, 0, 1, -1, 0, -40923));
      terms.Add(new PeriodicTerm(12, 1, 0, 0, 0, -34720));
      terms.Add(new PeriodicTerm(13, 0, 1, 1, 0, -30383));
      terms.Add(new PeriodicTerm(14, 2, 0, 0, -2, 15327));
      terms.Add(new PeriodicTerm(15, 0, 0, 1, 2, -12528));
      terms.Add(new PeriodicTerm(16, 0, 0, 1, -2, 10980));
      terms.Add(new PeriodicTerm(17, 4, 0, -1, 0, 10675));
      terms.Add(new PeriodicTerm(18, 0, 0, 3, 0, 10034));
      terms.Add(new PeriodicTerm(19, 4, 0, -2, 0, 8548));
      terms.Add(new PeriodicTerm(20, 2, 1, -1, 0, -7888));
      terms.Add(new PeriodicTerm(21, 2, 1, 0, 0, -6766));
      terms.Add(new PeriodicTerm(22, 1, 0, -1, 0, -5163));
      terms.Add(new PeriodicTerm(23, 1, 1, 0, 0, 4987));
      terms.Add(new PeriodicTerm(24, 2, -1, 1, 0, 4036));
      terms.Add(new PeriodicTerm(25, 2, 0, 2, 0, 3994));
      terms.Add(new PeriodicTerm(26, 4, 0, 0, 0, 3861));
      terms.Add(new PeriodicTerm(27, 2, 0, -3, 0, 3665));
      terms.Add(new PeriodicTerm(28, 0, 1, -2, 0, -2689));
      terms.Add(new PeriodicTerm(29, 2, 0, -1, 2, -2602));
      terms.Add(new PeriodicTerm(30, 2, -1, -2, 0, 2390));
    }

    private struct PeriodicTerm
    {
      public readonly int no;
      public readonly int d;
      public readonly int m;
      public readonly int m1;
      public readonly int f;
      public readonly int S;

      public PeriodicTerm(int no, int d, int m, int m1, int f, int S)
      {
        this.no = no;
        this.d = d;
        this.m = m;
        this.m1 = m1;
        this.f = f;
        this.S = S;
      }
    }
  }
}