﻿using Microsoft.Xna.Framework;
using SpaceGame.Weapons;

namespace SpaceGame.Ships
{
    public class TestEnemyShip : ShipBase
    {
        public TestEnemyShip(Vector2 spawnPosition, float spawnHeading)
            : base(spawnPosition,
                spawnHeading,
                texture: Art.TestEnemyShip,
                thrust: 200f,
                maxTurnRate: 2f,
                maneuveringThrust: 0.05f,
                maxVelocity: 500f,
                scale: Constants.Scales.Half)
        {
            _weapons.Add(new TestWeapon(Vector2.Zero + new Vector2(Texture.Width / 2, 0)));
        }
    }
}
