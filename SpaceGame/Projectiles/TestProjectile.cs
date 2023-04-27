using Microsoft.Xna.Framework;

namespace SpaceGame.Projectiles
{
    public class TestProjectile : ProjectileBase
    {
        public TestProjectile(FactionType faction, Vector2 position, Vector2 velocity)
            : base(faction,
                  position,
                  velocity,
                  texture: Art.Bullet,
                  timeToLiveInSeconds: 5)
        {
        }
    }
}
