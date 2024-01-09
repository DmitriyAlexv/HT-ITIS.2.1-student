using LogicServer.Models;

namespace LogicServer.FightCalc
{
    public interface IAttackAttemptCalculator
    {
        AttackAttempt CalculateAttempt(Entity attacker, Entity defender);
    }
}
