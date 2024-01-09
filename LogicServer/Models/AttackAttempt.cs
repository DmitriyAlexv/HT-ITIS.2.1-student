namespace LogicServer.Models
{
    public class AttackAttempt
    {
        public DamageRoll DamageRoll { get; set; } = null!;
        public HitChanceRoll HitChanceRoll { get; set; } = null!;
        public int ResultDamage { get; set; }
    }
}
