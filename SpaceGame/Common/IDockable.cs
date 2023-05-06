using System.Collections.Generic;

namespace SpaceGame.Common
{
    public interface IDockable : ICollidable
    {
        public List<string> Description { get; }
    }
}
