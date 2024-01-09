namespace LogicServer.Models
{
    public class FightRequest
    {
        public Entity Person { get; set; } = null!;
        public Entity Enemy { get; set; } = null!;
    }
}
