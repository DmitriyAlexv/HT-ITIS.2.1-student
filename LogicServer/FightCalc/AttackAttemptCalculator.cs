using LogicServer.Models;

namespace LogicServer.FightCalc
{
    public class AttackAttemptCalculator : IAttackAttemptCalculator
    {
        private readonly Random _random = new();
        public AttackAttempt CalculateAttempt(Entity attacker, Entity defender)
        {
            var hitChanceRoll = GetHitChanceRoll(attacker, defender);
            var damageRoll = GetDamageRoll(attacker);
            return new AttackAttempt()
            {
                HitChanceRoll = hitChanceRoll,
                DamageRoll = damageRoll,
                ResultDamage = hitChanceRoll.DamageMul * (damageRoll.TotalDamage + damageRoll.DiceRoll)
            };
        }

        private HitChanceRoll GetHitChanceRoll(Entity attacker, Entity defender)
        {
            var diceRoll = _random.Next(1, 21);
            var totalAttack = attacker.AttackModifier + attacker.Weapon;
            var damageMul = diceRoll == 1 ? 0 :
                diceRoll == 20 ? 2 :
                diceRoll + totalAttack >= defender.ArmorClass ? 1 : 0;
            return new HitChanceRoll()
            {
                DiceRoll = diceRoll,
                TotalAttack = totalAttack,
                DamageMul = damageMul,
                IsCrit = diceRoll == 20,
                IsHit = damageMul != 0,
            };
        }
        private DamageRoll GetDamageRoll(Entity attacker)
        {
            var diceRoll = _random.Next(attacker.MinDamage, attacker.MaxDamage + 1);
            var totalDamage = attacker.DamageModifier + attacker.Weapon;
            return new DamageRoll()
            {
                TotalDamage = totalDamage,
                DiceRoll = diceRoll
            };
        }
    }
}
