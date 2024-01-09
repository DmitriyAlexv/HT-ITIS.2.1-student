using LogicServer.FightCalc;
using LogicServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LogicServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FightController : ControllerBase
    {
        private readonly IFightCalculator _fightCalculator;

        public FightController(IFightCalculator fightCalculator)
        {
            _fightCalculator = fightCalculator;
        }

        [HttpPost]
        public List<Turn> FightCalculate(FightRequest fightRequest)
        {
            return _fightCalculator.GetFightLog(fightRequest.Person, fightRequest.Enemy);
        }
    }
}
