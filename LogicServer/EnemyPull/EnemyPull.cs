using LogicServer.DataBase;
using LogicServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LogicServer.EnemyPull
{
    public class EnemyPull : IEnemyPull
    {
        private readonly DnDDbContext _dbContext;
        private readonly Random _random;
        public EnemyPull(DnDDbContext context)
        {
            _dbContext = context;
            _random = new Random();
        }
        public async Task<Entity> GetRandomEnemyAsync()
        {
            var entityCount = await _dbContext.Entities.CountAsync();

            if (entityCount == 0)
                throw new DataException("There is no any entity");

            var rndId = _random.Next(1, entityCount + 1);
            return await _dbContext.Entities.FirstAsync(x => x.Id == rndId);
        }
    }
}
