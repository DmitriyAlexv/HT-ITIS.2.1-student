using LogicServer.Models;
using System.Text.Json;

namespace LogicServer.FightCalc
{
    public class FightCalculator: IFightCalculator
    {
        private readonly IAttackAttemptCalculator _attackAttemptCalculator;
        public FightCalculator(IAttackAttemptCalculator attackAttemptCalculator)
        {
            _attackAttemptCalculator = attackAttemptCalculator;
        }

        public List<Turn> GetFightLog(Entity person, Entity enemy)
        {
            var log = new List<Turn>();
            var isPersonTurn = true;
            while(person.HitPoints > 0 && enemy.HitPoints > 0)
            {
                if (isPersonTurn)
                    log.Add(ProcessTurn(person, enemy));
                else
                    log.Add(ProcessTurn(enemy, person));
                isPersonTurn = !isPersonTurn;
            }
            return log;
        }

        private Turn ProcessTurn(Entity attacker, Entity defender)
        {
            var attacks = new List<AttackAttempt>();
            for (int i = 0; i < attacker.AttackPerRound; i++)
            {
                attacks.Add(_attackAttemptCalculator.CalculateAttempt(attacker, defender));
            }
            defender.HitPoints -= attacks.Sum(x => x.ResultDamage);
            return new Turn()
            {
                Attacker = Entity.Clone(attacker),
                Defender = Entity.Clone(defender),
                AttackAttempts = attacks
            };
        }
    }
}
