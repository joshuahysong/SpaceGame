using Microsoft.Xna.Framework;
using SpaceGame.Scenes.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Generators
{
    public static class UniverseGenerator
    {
        public static List<SolarSystem> SolarSystems { get; private set; }
        public static Dictionary<string, SolarSystem> SolarSystemLookup { get; private set; }

        public static void GenerateSolarSystems()
        {
            var random = new Random();
            var minimumDistance = 50;
            var solarSystems = new List<SolarSystem>();
            var names = SolarSystemNameGenerator.GetNames(Constants.NumberOfSystems);
            while (solarSystems.Count < Constants.NumberOfSystems)
            {
                var newLocation = GetRandomPoint(random, Constants.UniverseRadius);

                // Check minimum distance (This is not a gross implementation but works)
                var closeNeighborFound = false;
                foreach (var mapLocation in solarSystems.Select(x => x.MapLocation))
                {
                    if (Vector2.Distance(newLocation, mapLocation) < minimumDistance)
                    {
                        closeNeighborFound = true;
                        break;
                    }
                }

                if (closeNeighborFound)
                    continue;

                var index = random.Next(0, Art.Backgrounds.All.Count);
                var background = Art.Backgrounds.All.ToArray()[index];
                var solarSystemName = names[solarSystems.Count];
                var newSystem = new SolarSystem(
                    solarSystemName,
                    FactionType.None,
                    new Vector2(newLocation.X, newLocation.Y),
                    CreateTestPlanet(random, solarSystemName),
                background);
                solarSystems.Add(newSystem);
            }

            SetupPaths(solarSystems);

            SolarSystems = solarSystems;
            SolarSystemLookup = SolarSystems.ToDictionary(x => x.Name, y => y);
        }

        private static Vector2 GetRandomPoint(Random random, int universeRadius)
        {
            var angle = random.NextDouble() * Math.PI * 2;
            var radius = Math.Sqrt(random.NextDouble()) * universeRadius;
            var x = 0 + radius * Math.Cos(angle);
            var y = 0 + radius * Math.Sin(angle);
            return new Vector2((float)x, (float)y);
        }

        private static void SetupPaths(List<SolarSystem> solarSystems)
        {
            // Set neighbors for each solar system to be ALL other solar systems. We will trim below.
            // TODO Instead of all systems create a delauny triangulation.
            solarSystems.ForEach(solarSystem => solarSystem.NeighborsByName.AddRange(solarSystems.Where(x => x != solarSystem).Select(x => x.Name)));

            // Create minimum spanning tree of connections
            var paths = new List<(Vector2, Vector2)>();
            var systemsToCheck = new List<SolarSystem> { solarSystems.First() };
            var solarSystemNameLookup = solarSystems.ToDictionary(x => x.Name, y => y);
            while (solarSystems.Any(x => systemsToCheck.All(y => y.Name != x.Name)))
            {
                var systemsToCheckNameLookup = systemsToCheck.ToDictionary(x => x.Name, y => y);
                var pathsToCompare = new List<(Vector2, Vector2)>();
                foreach (var systemToCheck in systemsToCheck)
                {
                    pathsToCompare.AddRange(systemToCheck.NeighborsByName
                        .Where(x => !systemsToCheckNameLookup.ContainsKey(x))
                        .Select(x => (systemToCheck.MapLocation, solarSystemNameLookup[x].MapLocation))
                        .Where(x => !paths.Contains(x) && !paths.Contains((x.Item2, x.Item1)))
                        .ToList());
                }

                var newPath = pathsToCompare
                    .Select(x => new { Path = x, Distance = Vector2.Distance(x.Item1, x.Item2) })
                    .OrderBy(x => x.Distance)
                    .Select(x => x.Path)
                    .First();
                paths.Add(newPath);

                systemsToCheck.Add(solarSystems.First(y => y.MapLocation == newPath.Item2));
            }

            foreach(var solarSystem in solarSystems)
            {
                var matchingPaths = paths.Where(x => x.Item1 == solarSystem.MapLocation || x.Item2 == solarSystem.MapLocation).ToList();
                var neighbors = solarSystems.Where(x => x != solarSystem && matchingPaths.Any(y => y.Item1 == x.MapLocation || y.Item2 == x.MapLocation));
                solarSystem.SetNeighbors(neighbors.Select(x => x.Name));
            }
        }

        private static List<Planet> CreateTestPlanet(Random random, string solarSystemName)
        {
            var index = random.Next(0, Art.Planets.All.Count);
            var texture = Art.Planets.All.ToArray()[index];

            // TODO Generated text
            var description = new List<string>();
            description.Add($"System Name: {solarSystemName}");
            description.Add($"Some interesting description here...");
            return new List<Planet> { new Planet(FactionType.None, Vector2.Zero, texture, description, solarSystemName) };
        }
    }
}
