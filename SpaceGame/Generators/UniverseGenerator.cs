using Microsoft.Xna.Framework;
using SpaceGame.Scenes.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Generators
{
    public static class UniverseGenerator
    {
        public static List<SolarSystem> GenerateSolarSystems(int universeRadius, int numberOfSystems)
        {
            var random = new Random();
            var tempName = 0;
            var minimumDistance = 50;
            var solarSystems = new List<SolarSystem>();
            while (solarSystems.Count < numberOfSystems)
            {
                var newLocation = GetRandomPoint(random, universeRadius);

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

                var index = random.Next(0, Art.Backgrounds.All.Count - 1);
                var background = Art.Backgrounds.All.ToArray()[index];
                var newSystem = new SolarSystem(
                    tempName.ToString(),
                    FactionType.None,
                    new Vector2(newLocation.X, newLocation.Y),
                    null,
                    CreateTestPlanet(random),
                background);

                solarSystems.Add(newSystem);
                tempName++;
            }

            return solarSystems;
        }

        private static Vector2 GetRandomPoint(Random random, int universeRadius)
        {
            var angle = random.NextDouble() * Math.PI * 2;
            var radius = Math.Sqrt(random.NextDouble()) * universeRadius;
            var x = 0 + radius * Math.Cos(angle);
            var y = 0 + radius * Math.Sin(angle);
            return new Vector2((float)x, (float)y);
        }

        private static List<Planet> CreateTestPlanet(Random random)
        {
            var index = random.Next(0, Art.Planets.All.Count - 1);
            var texture = Art.Planets.All.ToArray()[index];

            // TODO Generated text
            var description = new List<string>();
            //description.Add($"The sun beats down on the empty spaceport, its harsh rays reflecting off the sand dunes." +
            //    $"The only sound is the wind whistling through the abandoned buildings." +
            //    $"The spaceport is a reminder of a time when this planet was thriving, but now it is nothing more than a ghost town.");
            //description.Add($"The once-busy spaceport is now deserted. The docking bays are empty, the hangars are closed, and the control tower is silent." +
            //    $"The only signs of life are the occasional sand lizards that scuttle across the tarmac.");
            //description.Add($"The spaceport is a relic of a bygone era." +
            //    $"It was once a hub of commerce and trade, but now it is nothing more than a monument to a lost civilization. The buildings are crumbling," +
            //    $"the ships are rusting, and the sand is slowly encroaching on the tarmac.");
            //description.Add($"The spaceport is a reminder of the fragility of life." +
            //    $"It is a reminder that even the most prosperous civilizations can be brought to ruin. It is a reminder that nothing lasts forever.");

            description.Add($"Some interesting description here...");
            return new List<Planet> { new Planet(FactionType.None, Vector2.Zero, texture, description) };
        }
    }
}
