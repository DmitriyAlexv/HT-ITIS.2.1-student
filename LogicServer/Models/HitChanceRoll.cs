namespace LogicServer.Models
{
    public class HitChanceRoll
    {
        public int DiceRoll { get; set; }
        public int TotalAttack { get; set; }
        public int DamageMul { get; set; }
        public bool IsCrit { get; set; }
        public bool IsHit { get; set; }
    }
}
