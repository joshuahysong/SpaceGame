using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Generators
{
    public class SolarSystemNameGenerator
    {
        public static List<string> GetNames(int numberOfNames)
        {
            var random = new Random();
            var wordParts = new
            {
                One = new[] { "b", "c", "d", "f", "g", "h", "i", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" },
                Two = new[] { "a", "e", "o", "u" },
                Three = new[] { "br", "cr", "dr", "fr", "gr", "pr", "str", "tr", "bl", "cl", "fl", "gl", "pl", "sl", "sc", "sk", "sm", "sn", "sp", "st", "sw", "ch", "sh", "th", "wh" },
                Four = new[] { "ae", "ai", "ao", "au", "a", "ay", "ea", "ei", "eo", "eu", "e", "ey", "ua", "ue", "ui", "uo", "u", "uy", "ia", "ie", "iu", "io", "iy", "oa", "oe", "ou", "oi", "o", "oy" },
                Five = new[] { "turn", "ter", "nus", "rus", "tania", "hiri", "hines", "gawa", "nides", "carro", "rilia", "stea", "lia", "lea", "ria", "nov", "phus", "mia", "nerth", "wei", "ruta", "tov", "zuno", "vis", "lara", "nia", "liv", "tera", "gantu", "yama", "tune", "ter", "nus", "cury", "bos", "pra", "thea", "nope", "tis", "clite" },
                Six = new[] { "una", "ion", "iea", "iri", "illes", "ides", "agua", "olla", "inda", "eshan", "oria", "ilia", "erth", "arth", "orth", "oth", "illon", "ichi", "ov", "arvis", "ara", "ars", "yke", "yria", "onoe", "ippe", "osie", "one", "ore", "ade", "adus", "urn", "ypso", "ora", "iuq", "orix", "apus", "ion", "eon", "eron", "ao", "omia" }
            };

            var matrix = new[]
            {
                new[] { 1, 2, 5 },
                new[] { 2, 3, 6 },
                new[] { 3, 4, 5 },
                new[] { 4, 3, 6 },
                new[] { 3, 4, 2, 5 },
                new[] { 2, 1, 3, 6 },
                new[] { 3, 4, 2, 5 },
                new[] { 4, 3, 1, 6 },
                new[] { 3, 4, 1, 4, 5 },
                new[] { 4, 1, 4, 3, 6 },
            };

            var listOfNames = new List<string>();

            for (var x = 0; x < numberOfNames; x++)
            {
                var name = string.Empty;
                var parts = matrix[x % matrix.Length];
                for (var i = 0; i < parts.Length; i++)
                {
                    var index = parts[i];
                    var wordPartsToUse = index switch
                    {
                        1 => wordParts.One,
                        2 => wordParts.Two,
                        3 => wordParts.Three,
                        4 => wordParts.Four,
                        5 => wordParts.Five,
                        6 => wordParts.Six,
                        _ => null
                    };

                    name += wordPartsToUse[(int)Math.Floor(random.NextDouble() * wordPartsToUse.Length)];
                }
                listOfNames.Add(name);
            }

            // ensure names are unique by appending I as needed
            return listOfNames
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, y => y
                    .Select((z, i) => i == 0 ? z : $"{z} {new string('I', i + 1)}"))
                .SelectMany(x => x.Value)
                .OrderByDescending(x => x)
                .ToList();
        }
    }
}
