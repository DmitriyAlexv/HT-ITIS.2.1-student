using LogicServer.DataBase;
using LogicServer.EnemyPull;
using LogicServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogicServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataBaseController : ControllerBase
    {
        private readonly IEnemyPull _enemyPull;
        public DataBaseController(IEnemyPull enemyPull)
        {
           _enemyPull = enemyPull;
        }

        [HttpGet]
        public async Task<Entity> GetRandomEnemy() 
        {
            return await _enemyPull.GetRandomEnemyAsync();
        }
    }
}
