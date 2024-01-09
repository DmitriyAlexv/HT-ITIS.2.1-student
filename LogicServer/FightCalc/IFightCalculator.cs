using LogicServer.Models;

namespace LogicServer.FightCalc
{
    public interface IFightCalculator
    {
        List<Turn> GetFightLog(Entity person, Entity enemy);
    }
}
