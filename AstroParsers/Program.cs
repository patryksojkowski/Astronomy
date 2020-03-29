using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AstroParsers
{
    class Program
    {
        private static readonly string inputPath = @"C:\Users\ezsojpa\source\repos\Astronomy\OrbitalElementsParser\input.txt";
        private static readonly string outputPath = @"C:\Users\ezsojpa\source\repos\Astronomy\OrbitalElementsParser\output.txt";
        private static string LongAscNode, Inclination, Peryhelion, Eccentricity, MeanAnomaly, SemimajorAxis;

        private static Dictionary<string, string> titleToVariable = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            var content = File.ReadAllText(inputPath);
            var matches = Regex.Matches(content, @"([A-Z,a-z]+[\s]*=)[\s]*([\S]+)", RegexOptions.None);
            foreach (Match match in matches)
            {
                AssignToString(match.Groups[1].Value, match.Groups[2].Value);
            }
            string result = string.Empty;
            result += $"{titleToVariable["OM="]}m,";
            result += $" {titleToVariable["IN="]}m,";
            result += $" {titleToVariable["W ="]}m,";
            result += $" {titleToVariable["EC="]}m,";
            result += $" {titleToVariable["MA="]}m,";
            result += $" {titleToVariable["A ="]}m";
            File.WriteAllText(outputPath, result);


            System.Diagnostics.Process.Start(outputPath);
        }

        public static void AssignToString(string title, string value)
        {
            titleToVariable.Add(title, value);
        }
    }
}
