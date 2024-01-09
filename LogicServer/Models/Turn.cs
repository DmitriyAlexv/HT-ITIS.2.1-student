namespace LogicServer.Models
{
    public class Turn
    {
        public Entity Attacker { get; set; } = null!;
        public Entity Defender { get; set; } = null!;
        public List<AttackAttempt> AttackAttempts { get; set; } = null!;
    }
}
