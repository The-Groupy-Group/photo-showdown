using Microsoft.AspNetCore.Mvc;

namespace PhotoShowdownBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly PhotoShowdownDbContext _db;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, PhotoShowdownDbContext db)
        {
            _logger = logger;
            this._db = db;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet]
        public async Task<IEnumerable<User>> GetUsers()
        {
            _db.Users.Add(new User { Username = "Donfil", Password = "Donfil", Email = "" });
            _db.SaveChanges();
            return await _db.Users.ToListAsync();
        }
    }
}