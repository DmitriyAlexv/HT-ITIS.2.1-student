using LogicServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Numerics;
using System.Threading;

namespace WebUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        [BindProperty]
        public Entity Person { get; set; } = null!;
        public Entity Enemy { get; set; } = null!;
        public List<Turn> Log { get; set; } = null!;

        private readonly HttpClient _httpClient = new();
        private readonly Uri _urlCaclFight = new Uri("https://localhost:7186/Fight");
		private readonly Uri _urlGetRandom = new Uri("https://localhost:7186/DataBase");

		public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
        public async Task OnPost()
        {
            if (!ModelState.IsValid)
                return;

            Enemy = await _httpClient.GetFromJsonAsync<Entity>(_urlGetRandom) ?? throw new Exception("No enemy");

            var fightingCharacters = new FightRequest
            {
                Person = Person,
                Enemy = Enemy
            };

            var response = await _httpClient.PostAsJsonAsync(_urlCaclFight, fightingCharacters);
            Log = await response.Content.ReadFromJsonAsync<List<Turn>>() ?? throw new Exception("Cant process fight");
        }
    }
}