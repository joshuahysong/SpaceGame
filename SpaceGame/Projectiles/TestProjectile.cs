using Microsoft.Xna.Framework;

namespace SpaceGame.Projectiles
{
    public class TestProjectile : ProjectileBase
    {
        public TestProjectile(Vector2 position, Vector2 velocity)
            : base(position, 
                  velocity,
                  texture: Art.Bullet,
                  timeToLiveInSeconds: 5)
        {
        }
    }
}
