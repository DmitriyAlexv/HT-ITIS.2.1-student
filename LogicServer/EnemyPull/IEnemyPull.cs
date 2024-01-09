using LogicServer.Models;

namespace LogicServer.EnemyPull
{
    public interface IEnemyPull
    {
        Task<Entity> GetRandomEnemyAsync();
    }
}
