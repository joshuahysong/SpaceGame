using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame
{
    public static class Art
    {
        #region Backgrounds
        public static class Backgrounds
        {
            public static Texture2D BlueNebula1 { get; set; }
            public static Texture2D BlueNebula2 { get; set; }
            public static Texture2D BlueNebula3 { get; set; }
            public static Texture2D BlueNebula4 { get; set; }
            public static Texture2D BlueNebula5 { get; set; }
            public static Texture2D BlueNebula6 { get; set; }
            public static Texture2D BlueNebula7 { get; set; }
            public static Texture2D GreenNebula1 { get; set; }
            public static Texture2D GreenNebula2 { get; set; }
            public static Texture2D GreenNebula3 { get; set; }
            public static Texture2D GreenNebula4 { get; set; }
            public static Texture2D PurpleNebula1 { get; set; }
            public static Texture2D PurpleNebula2 { get; set; }
            public static Texture2D PurpleNebula3 { get; set; }
            public static Texture2D PurpleNebula4 { get; set; }
            public static Texture2D PurpleNebula5 { get; set; }
            public static Texture2D Starfield1 { get; set; }
            public static List<Texture2D> All => new()
            {
                BlueNebula1,
                BlueNebula2,
                BlueNebula3,
                BlueNebula4,
                BlueNebula5,
                BlueNebula6,
                BlueNebula7,
                GreenNebula1,
                GreenNebula2,
                GreenNebula3,
                GreenNebula4,
                PurpleNebula1,
                PurpleNebula2,
                PurpleNebula3,
                PurpleNebula4,
                PurpleNebula5
            };
        }
        #endregion

        #region Planets
        public static class Planets
        {
            public static class Airless
            {
                public static Texture2D Airless1 { get; set; }
                public static Texture2D Airless2 { get; set; }
                public static Texture2D Airless3 { get; set; }
                public static Texture2D Airless4 { get; set; }
                public static Texture2D Airless5 { get; set; }
            }

            public static class Aquamarine
            {
                public static Texture2D Aquamarine1 { get; set; }
                public static Texture2D Aquamarine2 { get; set; }
                public static Texture2D Aquamarine3 { get; set; }
                public static Texture2D Aquamarine4 { get; set; }
                public static Texture2D Aquamarine5 { get; set; }
            }

            public static class Arid
            {
                public static Texture2D Arid1 { get; set; }
                public static Texture2D Arid2 { get; set; }
                public static Texture2D Arid3 { get; set; }
                public static Texture2D Arid4 { get; set; }
                public static Texture2D Arid5 { get; set; }
            }

            public static class Barren
            {
                public static Texture2D Barren1 { get; set; }
                public static Texture2D Barren2 { get; set; }
                public static Texture2D Barren3 { get; set; }
                public static Texture2D Barren4 { get; set; }
                public static Texture2D Barren5 { get; set; }
            }

            public static class Cloudy
            {
                public static Texture2D Cloudy1 { get; set; }
                public static Texture2D Cloudy2 { get; set; }
                public static Texture2D Cloudy3 { get; set; }
                public static Texture2D Cloudy4 { get; set; }
                public static Texture2D Cloudy5 { get; set; }
            }

            public static class Cratered
            {
                public static Texture2D Cratered1 { get; set; }
                public static Texture2D Cratered2 { get; set; }
                public static Texture2D Cratered3 { get; set; }
                public static Texture2D Cratered4 { get; set; }
                public static Texture2D Cratered5 { get; set; }
            }

            public static class Dry
            {
                public static Texture2D Dry1 { get; set; }
                public static Texture2D Dry2 { get; set; }
                public static Texture2D Dry3 { get; set; }
                public static Texture2D Dry4 { get; set; }
                public static Texture2D Dry5 { get; set; }
            }

            public static class Frozen
            {
                public static Texture2D Frozen1 { get; set; }
                public static Texture2D Frozen2 { get; set; }
                public static Texture2D Frozen3 { get; set; }
                public static Texture2D Frozen4 { get; set; }
                public static Texture2D Frozen5 { get; set; }
            }

            public static class Generic
            {
                public static Texture2D Blue { get; set; }
                public static Texture2D Brown { get; set; }
                public static Texture2D Cyan { get; set; }
                public static Texture2D Green { get; set; }
                public static Texture2D Orange { get; set; }
                public static Texture2D Purple { get; set; }
                public static Texture2D Red { get; set; }
            }

            public static class Glacial
            {
                public static Texture2D Glacial1 { get; set; }
                public static Texture2D Glacial2 { get; set; }
                public static Texture2D Glacial3 { get; set; }
                public static Texture2D Glacial4 { get; set; }
                public static Texture2D Glacial5 { get; set; }
            }

            public static class Icy
            {
                public static Texture2D Icy1 { get; set; }
                public static Texture2D Icy2 { get; set; }
                public static Texture2D Icy3 { get; set; }
                public static Texture2D Icy4 { get; set; }
                public static Texture2D Icy5 { get; set; }
            }

            public static class Lunar
            {
                public static Texture2D Lunar1 { get; set; }
                public static Texture2D Lunar2 { get; set; }
                public static Texture2D Lunar3 { get; set; }
                public static Texture2D Lunar4 { get; set; }
                public static Texture2D Lunar5 { get; set; }
            }

            public static class Lush
            {
                public static Texture2D Lush1 { get; set; }
                public static Texture2D Lush2 { get; set; }
                public static Texture2D Lush3 { get; set; }
                public static Texture2D Lush4 { get; set; }
                public static Texture2D Lush5 { get; set; }
            }

            public static class Magma
            {
                public static Texture2D Magma1 { get; set; }
                public static Texture2D Magma2 { get; set; }
                public static Texture2D Magma3 { get; set; }
                public static Texture2D Magma4 { get; set; }
                public static Texture2D Magma5 { get; set; }
            }

            public static class Muddy
            {
                public static Texture2D Muddy1 { get; set; }
                public static Texture2D Muddy2 { get; set; }
                public static Texture2D Muddy3 { get; set; }
                public static Texture2D Muddy4 { get; set; }
                public static Texture2D Muddy5 { get; set; }
            }

            public static class Oasis
            {
                public static Texture2D Oasis1 { get; set; }
                public static Texture2D Oasis2 { get; set; }
                public static Texture2D Oasis3 { get; set; }
                public static Texture2D Oasis4 { get; set; }
                public static Texture2D Oasis5 { get; set; }
            }

            public static class Ocean
            {
                public static Texture2D Ocean1 { get; set; }
                public static Texture2D Ocean2 { get; set; }
                public static Texture2D Ocean3 { get; set; }
                public static Texture2D Ocean4 { get; set; }
                public static Texture2D Ocean5 { get; set; }
            }

            public static class Rocky
            {
                public static Texture2D Rocky1 { get; set; }
                public static Texture2D Rocky2 { get; set; }
                public static Texture2D Rocky3 { get; set; }
                public static Texture2D Rocky4 { get; set; }
                public static Texture2D Rocky5 { get; set; }
            }

            public static class Snowy
            {
                public static Texture2D Snowy1 { get; set; }
                public static Texture2D Snowy2 { get; set; }
                public static Texture2D Snowy3 { get; set; }
                public static Texture2D Snowy4 { get; set; }
                public static Texture2D Snowy5 { get; set; }
            }

            public static class Terrestrial
            {
                public static Texture2D Terrestrial1 { get; set; }
                public static Texture2D Terrestrial2 { get; set; }
                public static Texture2D Terrestrial3 { get; set; }
                public static Texture2D Terrestrial4 { get; set; }
                public static Texture2D Terrestrial5 { get; set; }
            }

            public static class Tropical
            {
                public static Texture2D Tropical1 { get; set; }
                public static Texture2D Tropical2 { get; set; }
                public static Texture2D Tropical3 { get; set; }
                public static Texture2D Tropical4 { get; set; }
                public static Texture2D Tropical5 { get; set; }
            }

            public static List<Texture2D> All => new()
            {
                Airless.Airless1,
                Airless.Airless2,
                Airless.Airless3,
                Airless.Airless4,
                Airless.Airless5,
                Arid.Arid1,
                Arid.Arid2,
                Arid.Arid3,
                Arid.Arid4,
                Arid.Arid5,
                Barren.Barren1,
                Barren.Barren2,
                Barren.Barren3,
                Barren.Barren4,
                Barren.Barren5,
                Cloudy.Cloudy1,
                Cloudy.Cloudy2,
                Cloudy.Cloudy3,
                Cloudy.Cloudy4,
                Cloudy.Cloudy5,
                Cratered.Cratered1,
                Cratered.Cratered2,
                Cratered.Cratered3,
                Cratered.Cratered4,
                Cratered.Cratered5,
                Dry.Dry1,
                Dry.Dry2,
                Dry.Dry3,
                Dry.Dry4,
                Dry.Dry5,
                Frozen.Frozen1,
                Frozen.Frozen2,
                Frozen.Frozen3,
                Frozen.Frozen4,
                Frozen.Frozen5,
                Generic.Blue,
                Generic.Brown,
                Generic.Cyan,
                Generic.Green,
                Generic.Orange,
                Generic.Purple,
                Generic.Red,
                Glacial.Glacial1,
                Glacial.Glacial2,
                Glacial.Glacial3,
                Glacial.Glacial4,
                Glacial.Glacial5,
                Icy.Icy1,
                Icy.Icy2,
                Icy.Icy3,
                Icy.Icy4,
                Icy.Icy5,
                Lunar.Lunar1,
                Lunar.Lunar2,
                Lunar.Lunar3,
                Lunar.Lunar4,
                Lunar.Lunar5,
                Lush.Lush1,
                Lush.Lush2,
                Lush.Lush3,
                Lush.Lush4,
                Lush.Lush5,
                Magma.Magma1,
                Magma.Magma2,
                Magma.Magma3,
                Magma.Magma4,
                Magma.Magma5,
                Muddy.Muddy1,
                Muddy.Muddy2,
                Muddy.Muddy3,
                Muddy.Muddy4,
                Muddy.Muddy5,
                Oasis.Oasis1,
                Oasis.Oasis2,
                Oasis.Oasis3,
                Oasis.Oasis4,
                Oasis.Oasis5,
                Ocean.Ocean1,
                Ocean.Ocean2,
                Ocean.Ocean3,
                Ocean.Ocean4,
                Ocean.Ocean5,
                Rocky.Rocky1,
                Rocky.Rocky2,
                Rocky.Rocky3,
                Rocky.Rocky4,
                Rocky.Rocky5,
                Snowy.Snowy1,
                Snowy.Snowy2,
                Snowy.Snowy3,
                Snowy.Snowy4,
                Snowy.Snowy5,
                Terrestrial.Terrestrial1,
                Terrestrial.Terrestrial2,
                Terrestrial.Terrestrial3,
                Terrestrial.Terrestrial4,
                Terrestrial.Terrestrial5,
                Tropical.Tropical1,
                Tropical.Tropical2,
                Tropical.Tropical3,
                Tropical.Tropical4,
                Tropical.Tropical5,
            };
        }
        #endregion

        #region Ships
        public static class Ships
        {
            public static Texture2D TestShip1 { get; set; }
            public static Texture2D TestShip2 { get; set; }
        }
        #endregion

        #region Parts
        public static class Parts
        {
            public static Texture2D Thruster1 { get; set; }
            public static Texture2D Turret1 { get; set; }
        }
        #endregion

        #region Projectiles
        public static class Projectiles
        {
            public static Texture2D GreenBullet { get; set; }
            public static Texture2D OrangeLaser { get; set; }
        }
        #endregion

        #region Fonts
        public static class Fonts
        {
            public static SpriteFont DebugFont { get; set; }
            public static SpriteFont HeaderFont { get; set; }
            public static SpriteFont UISmalFont { get; set; }
            public static SpriteFont UIMediumFont { get; set; }
            public static SpriteFont UILargeFont { get; set; }
        }
        #endregion

        #region Misc
        public static class Misc
        {
            public static Texture2D Pixel { get; set; }
            public static Texture2D SolarSystem { get; set; }
        }
        #endregion

        public static void Load(ContentManager content)
        {
            Backgrounds.BlueNebula1 = content.Load<Texture2D>("Backgrounds/blue-nebula-1");
            Backgrounds.BlueNebula2 = content.Load<Texture2D>("Backgrounds/blue-nebula-2");
            Backgrounds.BlueNebula3 = content.Load<Texture2D>("Backgrounds/blue-nebula-3");
            Backgrounds.BlueNebula4 = content.Load<Texture2D>("Backgrounds/blue-nebula-4");
            Backgrounds.BlueNebula5 = content.Load<Texture2D>("Backgrounds/blue-nebula-5");
            Backgrounds.BlueNebula6 = content.Load<Texture2D>("Backgrounds/blue-nebula-6");
            Backgrounds.BlueNebula7 = content.Load<Texture2D>("Backgrounds/blue-nebula-7");
            Backgrounds.GreenNebula1 = content.Load<Texture2D>("Backgrounds/green-nebula-1");
            Backgrounds.GreenNebula2 = content.Load<Texture2D>("Backgrounds/green-nebula-2");
            Backgrounds.GreenNebula3 = content.Load<Texture2D>("Backgrounds/green-nebula-3");
            Backgrounds.GreenNebula4 = content.Load<Texture2D>("Backgrounds/green-nebula-4");
            Backgrounds.PurpleNebula1 = content.Load<Texture2D>("Backgrounds/purple-nebula-1");
            Backgrounds.PurpleNebula2 = content.Load<Texture2D>("Backgrounds/purple-nebula-2");
            Backgrounds.PurpleNebula3 = content.Load<Texture2D>("Backgrounds/purple-nebula-3");
            Backgrounds.PurpleNebula4 = content.Load<Texture2D>("Backgrounds/purple-nebula-4");
            Backgrounds.PurpleNebula5 = content.Load<Texture2D>("Backgrounds/purple-nebula-5");
            Backgrounds.Starfield1 = content.Load<Texture2D>("Backgrounds/starfield-1");

            Planets.Airless.Airless1 = content.Load<Texture2D>("Planets/Airless/airless-1");
            Planets.Airless.Airless2 = content.Load<Texture2D>("Planets/Airless/airless-2");
            Planets.Airless.Airless3 = content.Load<Texture2D>("Planets/Airless/airless-3");
            Planets.Airless.Airless4 = content.Load<Texture2D>("Planets/Airless/airless-4");
            Planets.Airless.Airless5 = content.Load<Texture2D>("Planets/Airless/airless-5");
            Planets.Aquamarine.Aquamarine1 = content.Load<Texture2D>("Planets/Aquamarine/aquamarine-1");
            Planets.Aquamarine.Aquamarine2 = content.Load<Texture2D>("Planets/Aquamarine/aquamarine-2");
            Planets.Aquamarine.Aquamarine3 = content.Load<Texture2D>("Planets/Aquamarine/aquamarine-3");
            Planets.Aquamarine.Aquamarine4 = content.Load<Texture2D>("Planets/Aquamarine/aquamarine-4");
            Planets.Aquamarine.Aquamarine5 = content.Load<Texture2D>("Planets/Aquamarine/aquamarine-5");
            Planets.Arid.Arid1 = content.Load<Texture2D>("Planets/Arid/arid-1");
            Planets.Arid.Arid2 = content.Load<Texture2D>("Planets/Arid/arid-2");
            Planets.Arid.Arid3 = content.Load<Texture2D>("Planets/Arid/arid-3");
            Planets.Arid.Arid4 = content.Load<Texture2D>("Planets/Arid/arid-4");
            Planets.Arid.Arid5 = content.Load<Texture2D>("Planets/Arid/arid-5");
            Planets.Barren.Barren1 = content.Load<Texture2D>("Planets/Barren/barren-1");
            Planets.Barren.Barren2 = content.Load<Texture2D>("Planets/Barren/barren-2");
            Planets.Barren.Barren3 = content.Load<Texture2D>("Planets/Barren/barren-3");
            Planets.Barren.Barren4 = content.Load<Texture2D>("Planets/Barren/barren-4");
            Planets.Barren.Barren5 = content.Load<Texture2D>("Planets/Barren/barren-5");
            Planets.Barren.Barren5 = content.Load<Texture2D>("Planets/Barren/barren-5");
            Planets.Cloudy.Cloudy1 = content.Load<Texture2D>("Planets/Cloudy/cloudy-1");
            Planets.Cloudy.Cloudy2 = content.Load<Texture2D>("Planets/Cloudy/cloudy-2");
            Planets.Cloudy.Cloudy3 = content.Load<Texture2D>("Planets/Cloudy/cloudy-3");
            Planets.Cloudy.Cloudy4 = content.Load<Texture2D>("Planets/Cloudy/cloudy-4");
            Planets.Cloudy.Cloudy5 = content.Load<Texture2D>("Planets/Cloudy/cloudy-5");
            Planets.Cratered.Cratered1 = content.Load<Texture2D>("Planets/Cratered/cratered-1");
            Planets.Cratered.Cratered2 = content.Load<Texture2D>("Planets/Cratered/cratered-2");
            Planets.Cratered.Cratered3 = content.Load<Texture2D>("Planets/Cratered/cratered-3");
            Planets.Cratered.Cratered4 = content.Load<Texture2D>("Planets/Cratered/cratered-4");
            Planets.Cratered.Cratered5 = content.Load<Texture2D>("Planets/Cratered/cratered-5");
            Planets.Dry.Dry1 = content.Load<Texture2D>("Planets/Dry/dry-1");
            Planets.Dry.Dry2 = content.Load<Texture2D>("Planets/Dry/dry-2");
            Planets.Dry.Dry3 = content.Load<Texture2D>("Planets/Dry/dry-3");
            Planets.Dry.Dry4 = content.Load<Texture2D>("Planets/Dry/dry-4");
            Planets.Dry.Dry5 = content.Load<Texture2D>("Planets/Dry/dry-5");
            Planets.Frozen.Frozen1 = content.Load<Texture2D>("Planets/Frozen/frozen-1");
            Planets.Frozen.Frozen2 = content.Load<Texture2D>("Planets/Frozen/frozen-2");
            Planets.Frozen.Frozen3 = content.Load<Texture2D>("Planets/Frozen/frozen-3");
            Planets.Frozen.Frozen4 = content.Load<Texture2D>("Planets/Frozen/frozen-4");
            Planets.Frozen.Frozen5 = content.Load<Texture2D>("Planets/Frozen/frozen-5");
            Planets.Generic.Blue = content.Load<Texture2D>("Planets/Generic/blue");
            Planets.Generic.Brown = content.Load<Texture2D>("Planets/Generic/brown");
            Planets.Generic.Cyan = content.Load<Texture2D>("Planets/Generic/cyan");
            Planets.Generic.Green = content.Load<Texture2D>("Planets/Generic/green");
            Planets.Generic.Orange = content.Load<Texture2D>("Planets/Generic/orange");
            Planets.Generic.Purple = content.Load<Texture2D>("Planets/Generic/purple");
            Planets.Generic.Red = content.Load<Texture2D>("Planets/Generic/red");
            Planets.Glacial.Glacial1 = content.Load<Texture2D>("Planets/Glacial/glacial-1");
            Planets.Glacial.Glacial2 = content.Load<Texture2D>("Planets/Glacial/glacial-2");
            Planets.Glacial.Glacial3 = content.Load<Texture2D>("Planets/Glacial/glacial-3");
            Planets.Glacial.Glacial4 = content.Load<Texture2D>("Planets/Glacial/glacial-4");
            Planets.Glacial.Glacial5 = content.Load<Texture2D>("Planets/Glacial/glacial-5");
            Planets.Icy.Icy1 = content.Load<Texture2D>("Planets/Icy/icy-1");
            Planets.Icy.Icy2 = content.Load<Texture2D>("Planets/Icy/icy-2");
            Planets.Icy.Icy3 = content.Load<Texture2D>("Planets/Icy/icy-3");
            Planets.Icy.Icy4 = content.Load<Texture2D>("Planets/Icy/icy-4");
            Planets.Icy.Icy5 = content.Load<Texture2D>("Planets/Icy/icy-5");
            Planets.Lunar.Lunar1 = content.Load<Texture2D>("Planets/Lunar/lunar-1");
            Planets.Lunar.Lunar2 = content.Load<Texture2D>("Planets/Lunar/lunar-2");
            Planets.Lunar.Lunar3 = content.Load<Texture2D>("Planets/Lunar/lunar-3");
            Planets.Lunar.Lunar4 = content.Load<Texture2D>("Planets/Lunar/lunar-4");
            Planets.Lunar.Lunar5 = content.Load<Texture2D>("Planets/Lunar/lunar-5");
            Planets.Lush.Lush1 = content.Load<Texture2D>("Planets/Lush/lush-1");
            Planets.Lush.Lush2 = content.Load<Texture2D>("Planets/Lush/lush-2");
            Planets.Lush.Lush3 = content.Load<Texture2D>("Planets/Lush/lush-3");
            Planets.Lush.Lush4 = content.Load<Texture2D>("Planets/Lush/lush-4");
            Planets.Lush.Lush5 = content.Load<Texture2D>("Planets/Lush/lush-5");
            Planets.Magma.Magma1 = content.Load<Texture2D>("Planets/Magma/magma-1");
            Planets.Magma.Magma2 = content.Load<Texture2D>("Planets/Magma/magma-2");
            Planets.Magma.Magma3 = content.Load<Texture2D>("Planets/Magma/magma-3");
            Planets.Magma.Magma4 = content.Load<Texture2D>("Planets/Magma/magma-4");
            Planets.Magma.Magma5 = content.Load<Texture2D>("Planets/Magma/magma-5");
            Planets.Muddy.Muddy1 = content.Load<Texture2D>("Planets/Muddy/muddy-1");
            Planets.Muddy.Muddy2 = content.Load<Texture2D>("Planets/Muddy/muddy-2");
            Planets.Muddy.Muddy3 = content.Load<Texture2D>("Planets/Muddy/muddy-3");
            Planets.Muddy.Muddy4 = content.Load<Texture2D>("Planets/Muddy/muddy-4");
            Planets.Muddy.Muddy5 = content.Load<Texture2D>("Planets/Muddy/muddy-5");
            Planets.Oasis.Oasis1 = content.Load<Texture2D>("Planets/Oasis/oasis-1");
            Planets.Oasis.Oasis2 = content.Load<Texture2D>("Planets/Oasis/oasis-2");
            Planets.Oasis.Oasis3 = content.Load<Texture2D>("Planets/Oasis/oasis-3");
            Planets.Oasis.Oasis4 = content.Load<Texture2D>("Planets/Oasis/oasis-4");
            Planets.Oasis.Oasis5 = content.Load<Texture2D>("Planets/Oasis/oasis-5");
            Planets.Ocean.Ocean1 = content.Load<Texture2D>("Planets/Ocean/ocean-1");
            Planets.Ocean.Ocean2 = content.Load<Texture2D>("Planets/Ocean/ocean-2");
            Planets.Ocean.Ocean3 = content.Load<Texture2D>("Planets/Ocean/ocean-3");
            Planets.Ocean.Ocean4 = content.Load<Texture2D>("Planets/Ocean/ocean-4");
            Planets.Ocean.Ocean5 = content.Load<Texture2D>("Planets/Ocean/ocean-5");
            Planets.Rocky.Rocky1 = content.Load<Texture2D>("Planets/Rocky/rocky-1");
            Planets.Rocky.Rocky2 = content.Load<Texture2D>("Planets/Rocky/rocky-2");
            Planets.Rocky.Rocky3 = content.Load<Texture2D>("Planets/Rocky/rocky-3");
            Planets.Rocky.Rocky4 = content.Load<Texture2D>("Planets/Rocky/rocky-4");
            Planets.Rocky.Rocky5 = content.Load<Texture2D>("Planets/Rocky/rocky-5");
            Planets.Snowy.Snowy1 = content.Load<Texture2D>("Planets/Snowy/snowy-1");
            Planets.Snowy.Snowy2 = content.Load<Texture2D>("Planets/Snowy/snowy-2");
            Planets.Snowy.Snowy3 = content.Load<Texture2D>("Planets/Snowy/snowy-3");
            Planets.Snowy.Snowy4 = content.Load<Texture2D>("Planets/Snowy/snowy-4");
            Planets.Snowy.Snowy5 = content.Load<Texture2D>("Planets/Snowy/snowy-5");
            Planets.Terrestrial.Terrestrial1 = content.Load<Texture2D>("Planets/Terrestrial/terrestrial-1");
            Planets.Terrestrial.Terrestrial2 = content.Load<Texture2D>("Planets/Terrestrial/terrestrial-2");
            Planets.Terrestrial.Terrestrial3 = content.Load<Texture2D>("Planets/Terrestrial/terrestrial-3");
            Planets.Terrestrial.Terrestrial4 = content.Load<Texture2D>("Planets/Terrestrial/terrestrial-4");
            Planets.Terrestrial.Terrestrial5 = content.Load<Texture2D>("Planets/Terrestrial/terrestrial-5");
            Planets.Tropical.Tropical1 = content.Load<Texture2D>("Planets/Tropical/tropical-1");
            Planets.Tropical.Tropical2 = content.Load<Texture2D>("Planets/Tropical/tropical-2");
            Planets.Tropical.Tropical3 = content.Load<Texture2D>("Planets/Tropical/tropical-3");
            Planets.Tropical.Tropical4 = content.Load<Texture2D>("Planets/Tropical/tropical-4");
            Planets.Tropical.Tropical5 = content.Load<Texture2D>("Planets/Tropical/tropical-5");

            Ships.TestShip1 = content.Load<Texture2D>("Ships/TestShip1");
            Ships.TestShip2 = content.Load<Texture2D>("Ships/TestShip2");

            Projectiles.GreenBullet = content.Load<Texture2D>("Projectiles/green_bullet");
            Projectiles.OrangeLaser = content.Load<Texture2D>("Projectiles/orange_laser");

            Parts.Thruster1 = content.Load<Texture2D>("Parts/thruster-1");
            Parts.Turret1 = content.Load<Texture2D>("Parts/turret-1");

            Fonts.DebugFont = content.Load<SpriteFont>("Fonts/Debug");
            Fonts.HeaderFont = content.Load<SpriteFont>("Fonts/Header");
            Fonts.UISmalFont = content.Load<SpriteFont>("Fonts/UI_Small");
            Fonts.UIMediumFont = content.Load<SpriteFont>("Fonts/UI_Medium");
            Fonts.UILargeFont = content.Load<SpriteFont>("Fonts/UI_Large");

            Misc.Pixel = new Texture2D(MainGame.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Misc.Pixel.SetData(new[] { Color.White });
            Misc.SolarSystem = content.Load<Texture2D>("Misc/solar-system");
        }

        public static Texture2D CreateRectangleTexture(int width, int height, Color fillColor, Color borderColor, int thickness = 1)
        {
            Texture2D texture = new Texture2D(MainGame.Instance.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (var i = 0; i < data.Length; ++i)
            {
                if (i < width * thickness ||
                    i > width * height - width * thickness)
                {
                    data[i] = borderColor;
                }
                else
                {
                    data[i] = fillColor;
                }
                for (var x = 0; x < thickness; x++)
                {
                    if (i % width - x == 0 ||
                        (i + 1 + x) % width == 0)
                    {
                        data[i] = borderColor;
                    }
                }
            }
            texture.SetData(data);
            return texture;
        }

        public static Texture2D CreateCircleTexture(int radius, Color borderColor)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(MainGame.Instance.GraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = borderColor;
            }

            texture.SetData(data);
            return texture;
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(Misc.Pixel, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static Color[] GetScaledTextureData(Texture2D texture, float scale)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            if (scale == ScaleType.Full)
                return data;

            // TODO Add support for other scales
            if (scale != ScaleType.Half)
                throw new ArgumentException("Must be a valid scale", nameof(scale));

            float modulus = 2;
            var columnsToKeep = Enumerable.Range(0, texture.Width - 1).Where((x, i) => i % modulus == 0).ToList();
            var rowsToKeep = Enumerable.Range(0, texture.Height - 1).Where((x, i) => i % modulus == 0).ToList();
            var numberOfRows = texture.Height;
            var bothRemoved = new List<Color>();
            var keptRows = new List<Color[]>();
            rowsToKeep.ToList().ForEach(x =>
            {
                keptRows.Add(data.Where((z, i) => i >= x * texture.Width && i < (x + 1) * texture.Width).ToArray());
            });

            keptRows.ForEach(x =>
            {
                bothRemoved.AddRange(x.Where((y, i) => columnsToKeep.Contains(i)));
            });

            return bothRemoved.ToArray();
        }
    }
}