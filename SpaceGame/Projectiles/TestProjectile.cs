using Microsoft.Xna.Framework;

namespace SpaceGame.Projectiles
{
    public class TestProjectile : ProjectileBase
    {
        public TestProjectile(Vector2 position, Vector2 velocity)
            : base(position, 
                  velocity,
                  image: Art.Bullet,
                  timeToLiveInSeconds: 5)
        {
        }
    }
}
