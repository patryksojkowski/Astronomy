using System;

namespace Decimal
{
    public static class DecimalMath
    {
        public const decimal DegToRad = 0.017453292519943295769236907684886127134428718885417254560M;

        public const decimal RadToDeg = 57.295779513082320876798154814105M;

        public const decimal Pi = 3.14159265358979323846264338327950288419716939937510M;

        public const decimal Epsilon = 0.0000000000000000001M;

        private const decimal PIx2 = 6.28318530717958647692528676655900576839433879875021M;

        public const decimal E = 2.7182818284590452353602874713526624977572470936999595749M;

        private const decimal PIdiv2 = 1.570796326794896619231321691639751442098584699687552910487M;

        private const decimal PIdiv4 = 0.785398163397448309615660845819875721049292349843776455243M;

        private const decimal Einv = 0.3678794411714423215955237701614608674458111310317678M;

        private const decimal Log10Inv = 0.434294481903251827651128918916605082294397005803666566114M;

        public const decimal Zero = 0.0M;

        public const decimal One = 1.0M;

        private const decimal Half = 0.5M;

        private const int MaxIteration = 100;

        public static decimal Exp(decimal x)
        {
            var count = 0;
            while (x > One)
            {
                x--;
                count++;
            }
            while (x < Zero)
            {
                x++;
                count--;
            }
            var iteration = 1;
            var result = One;
            var fatorial = One;
            decimal cachedResult;
            do
            {
                cachedResult = result;
                fatorial *= x / iteration++;
                result += fatorial;
            } while (cachedResult != result);
            if (count != 0) result = result * PowerN(E, count);
            return result;
        }

        public static decimal Power(decimal value, decimal pow)
        {
            if (pow == Zero) return One;
            if (pow == One) return value;
            if (value == One) return One;

            if (value == Zero && pow == Zero) return One;

            if (value == Zero)
            {
                if (pow > Zero)
                {
                    return Zero;
                }

                throw new Exception("Invalid Operation: zero base and negative power");
            }

            if (pow == -One) return One / value;

            var isPowerInteger = IsInteger(pow);
            if (value < Zero && !isPowerInteger)
            {
                throw new Exception("Invalid Operation: negative base and non-integer power");
            }

            if (isPowerInteger && value > Zero)
            {
                int powerInt = (int)(pow);
                return PowerN(value, powerInt);
            }

            if (isPowerInteger && value < Zero)
            {
                int powerInt = (int)pow;
                if (powerInt % 2 == 0)
                {
                    return Exp(pow * Log(-value));
                }
                else
                {
                    return -Exp(pow * Log(-value));
                }
            }

            return Exp(pow * Log(value));
        }

        private static bool IsInteger(decimal value)
        {
            long longValue = (long)value;
            if (Abs(value - longValue) <= Epsilon)
            {
                return true;
            }

            return false;
        }

        public static decimal PowerN(decimal value, int power)
        {
            if (power == Zero) return One;
            if (power < Zero) return PowerN(One / value, -power);

            var q = power;
            var prod = One;
            var current = value;
            while (q > 0)
            {
                if (q % 2 == 1)
                {
                    // detects the 1s in the binary expression of power
                    prod = current * prod; // picks up the relevant power
                    q--;
                }
                current *= current; // value^i -> value^(2*i)
                q = q / 2;
            }

            return prod;
        }

        public static decimal Log10(decimal x)
        {
            return Log(x) * Log10Inv;
        }

        public static decimal Log(decimal x)
        {
            if (x <= Zero)
            {
                throw new ArgumentException("x must be greater than zero");
            }
            var count = 0;
            while (x >= One)
            {
                x *= Einv;
                count++;
            }
            while (x <= Einv)
            {
                x *= E;
                count--;
            }
            x--;
            if (x == 0) return count;
            var result = Zero;
            var iteration = 0;
            var y = One;
            var cacheResult = result - One;
            while (cacheResult != result && iteration < MaxIteration)
            {
                iteration++;
                cacheResult = result;
                y *= -x;
                result += y / iteration;
            }
            return count - result;
        }

        public static decimal Cos(decimal x)
        {
            while (x > PIx2)
            {
                x -= PIx2;
            }
            while (x < -PIx2)
            {
                x += PIx2;
            }
            // now x in (-2pi,2pi)
            if (x >= Pi && x <= PIx2)
            {
                return -Cos(x - Pi);
            }
            if (x >= -PIx2 && x <= -Pi)
            {
                return -Cos(x + Pi);
            }
            x = x * x;
            //y=1-x/2!+x^2/4!-x^3/6!...
            var xx = -x * Half;
            var y = One + xx;
            var cachedY = y - One;//init cache  with different value
            for (var i = 1; cachedY != y && i < MaxIteration; i++)
            {
                cachedY = y;
                decimal factor = i * (i + i + 3) + 1; //2i^2+2i+i+1=2i^2+3i+1
                factor = -Half / factor;
                xx *= x * factor;
                y += xx;
            }
            return y;
        }

        public static decimal Tan(decimal x)
        {
            var cos = Cos(x);
            if (cos == Zero) throw new ArgumentException(nameof(x));
            return Sin(x) / cos;
        }

        public static decimal Sin(decimal x)
        {
            var cos = Cos(x);
            var moduleOfSin = Sqrt(One - (cos * cos));
            var sineIsPositive = IsSignOfSinePositive(x);
            if (sineIsPositive) return moduleOfSin;
            return -moduleOfSin;
        }

        private static bool IsSignOfSinePositive(decimal x)
        {
            //truncating to  [-2*PI;2*PI]
            while (x >= PIx2)
            {
                x -= PIx2;
            }

            while (x <= -PIx2)
            {
                x += PIx2;
            }

            //now x in [-2*PI;2*PI]
            if (x >= -PIx2 && x <= -Pi) return true;
            if (x >= -Pi && x <= Zero) return false;
            if (x >= Zero && x <= Pi) return true;
            if (x >= Pi && x <= PIx2) return false;

            //will not be reached
            throw new ArgumentException(nameof(x));
        }

        public static decimal Sqrt(decimal x, decimal epsilon = Zero)
        {
            if (x < Zero) throw new OverflowException("Cannot calculate square root from a negative number");
            //initial approximation
            decimal current = (decimal)Math.Sqrt((double)x), previous;
            do
            {
                previous = current;
                if (previous == Zero) return Zero;
                current = (previous + x / previous) * Half;
            } while (Abs(previous - current) > epsilon);
            return current;
        }

        public static decimal Sinh(decimal x)
        {
            var y = Exp(x);
            var yy = One / y;
            return (y - yy) * Half;
        }

        public static decimal Cosh(decimal x)
        {
            var y = Exp(x);
            var yy = One / y;
            return (y + yy) * Half;
        }

        public static int Sign(decimal x)
        {
            return x < Zero ? -1 : (x > Zero ? 1 : 0);
        }

        public static decimal Tanh(decimal x)
        {
            var y = Exp(x);
            var yy = One / y;
            return (y - yy) / (y + yy);
        }

        public static decimal Abs(decimal x)
        {
            if (x <= Zero)
            {
                return -x;
            }
            return x;
        }

        public static decimal Asin(decimal x)
        {
            if (x > One || x < -One)
            {
                throw new ArgumentException("x must be in [-1,1]");
            }
            //known values
            if (x == Zero) return Zero;
            if (x == One) return PIdiv2;
            //asin function is odd function
            if (x < Zero) return -Asin(-x);

            //my optimize trick here

            // used a math formula to speed up :
            // asin(x)=0.5*(pi/2-asin(1-2*x*x)) 
            // if x>=0 is true

            var newX = One - 2 * x * x;

            //for calculating new value near to zero than current
            //because we gain more speed with values near to zero
            if (Abs(x) > Abs(newX))
            {
                var t = Asin(newX);
                return Half * (PIdiv2 - t);
            }
            var y = Zero;
            var result = x;
            decimal cachedResult;
            var i = 1;
            y += result;
            var xx = x * x;
            do
            {
                cachedResult = result;
                result *= xx * (One - Half / (i));
                y += result / (2 * i + 1);
                i++;
            } while (cachedResult != result);
            return y;
        }

        public static decimal ATan(decimal x)
        {
            if (x == Zero) return Zero;
            if (x == One) return PIdiv4;
            return Asin(x / Sqrt(One + x * x));
        }

        public static decimal Acos(decimal x)
        {
            if (x == Zero) return PIdiv2;
            if (x == One) return Zero;
            if (x < Zero) return Pi - Acos(-x);
            return PIdiv2 - Asin(x);
        }

        public static decimal Atan2(decimal y, decimal x)
        {
            if (x > Zero)
            {
                return ATan(y / x);
            }
            if (x < Zero && y >= Zero)
            {
                return ATan(y / x) + Pi;
            }
            if (x < Zero && y < Zero)
            {
                return ATan(y / x) - Pi;
            }
            if (x == Zero && y > Zero)
            {
                return PIdiv2;
            }
            if (x == Zero && y < Zero)
            {
                return -PIdiv2;
            }
            throw new ArgumentException("invalid atan2 arguments");
        }

        public static bool IsClose(decimal d1, decimal d2, decimal epsilon = 1m)
        {
            return Abs(d1 - d2) < epsilon;
        }
    }
}