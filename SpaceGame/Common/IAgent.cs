namespace SpaceGame.Common
{
    public interface IAgent : IEntity
    {
        public string CurrentSolarSystemName { get; set; }
    }
}
