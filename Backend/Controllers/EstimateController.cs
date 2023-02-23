namespace Backend.Controllers { 

    [ApiController]
    [Route("[controller]")]
    public class EstimateController : ControllerBase
    {

        private readonly ILogger<EstimateController> _logger;

        public EstimateController(ILogger<EstimateController> logger)
        {
            _logger = logger;
        }

        /*
         * GET-Example
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        */
    }
}