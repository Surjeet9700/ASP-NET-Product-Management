using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAspNetCoreApp.Data;
using MyAspNetCoreApp.Models;
using System.Diagnostics;
using System.Text.Json;

namespace MyAspNetCoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalApiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExternalApiController> _logger;
        private readonly IConfiguration _configuration;

        public ExternalApiController(
            IHttpClientFactory httpClientFactory,
            ApplicationDbContext context,
            ILogger<ExternalApiController> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        // GET: api/ExternalApi/weather
        /// <summary>
        /// Get current weather data for a location
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <returns>Weather data</returns>
        [HttpGet("weather")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<WeatherResponse>> GetWeather(
            [FromQuery] double latitude = 52.52,
            [FromQuery] double longitude = 13.41)
        {
            var stopwatch = Stopwatch.StartNew();
            var apiLog = new ExternalApiLog
            {
                ApiName = "Open-Meteo Weather API",
                RequestMethod = "GET",
                RequestedAt = DateTime.UtcNow
            };

            try
            {
                var weatherApiUrl = _configuration["ExternalApis:WeatherApiUrl"];
                var url = $"{weatherApiUrl}?latitude={latitude}&longitude={longitude}&current=temperature_2m,wind_speed_10m,weather_code";
                apiLog.RequestUrl = url;

                _logger.LogInformation("Calling Weather API: {Url}", url);

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(url);

                stopwatch.Stop();
                apiLog.ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                apiLog.StatusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    apiLog.ResponseBody = content;
                    apiLog.IsSuccessful = true;

                    // Parse the weather response
                    var weatherData = JsonSerializer.Deserialize<JsonElement>(content);
                    var weatherResponse = new WeatherResponse
                    {
                        Latitude = weatherData.GetProperty("latitude").GetDouble(),
                        Longitude = weatherData.GetProperty("longitude").GetDouble(),
                        Current = new CurrentWeather
                        {
                            Time = weatherData.GetProperty("current").GetProperty("time").GetString(),
                            Temperature = weatherData.GetProperty("current").GetProperty("temperature_2m").GetDouble(),
                            WindSpeed = weatherData.GetProperty("current").GetProperty("wind_speed_10m").GetDouble(),
                            WeatherCode = weatherData.GetProperty("current").GetProperty("weather_code").GetInt32()
                        }
                    };

                    // Log to database
                    _context.ExternalApiLogs.Add(apiLog);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Weather API call successful. Response time: {ResponseTime}ms", apiLog.ResponseTimeMs);

                    return Ok(weatherResponse);
                }
                else
                {
                    apiLog.IsSuccessful = false;
                    apiLog.ErrorMessage = $"API returned status code: {response.StatusCode}";
                    _context.ExternalApiLogs.Add(apiLog);
                    await _context.SaveChangesAsync();

                    _logger.LogWarning("Weather API call failed. Status: {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, new { message = "Failed to fetch weather data" });
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                apiLog.ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                apiLog.IsSuccessful = false;
                apiLog.ErrorMessage = ex.Message;

                _context.ExternalApiLogs.Add(apiLog);
                await _context.SaveChangesAsync();

                _logger.LogError(ex, "Error calling Weather API");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/ExternalApi/exchange-rates
        /// <summary>
        /// Get current exchange rates
        /// </summary>
        /// <param name="baseCurrency">Base currency code (default: USD)</param>
        /// <returns>Exchange rates</returns>
        [HttpGet("exchange-rates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExchangeRateResponse>> GetExchangeRates(
            [FromQuery] string baseCurrency = "USD")
        {
            var stopwatch = Stopwatch.StartNew();
            var apiLog = new ExternalApiLog
            {
                ApiName = "Exchange Rate API",
                RequestMethod = "GET",
                RequestedAt = DateTime.UtcNow
            };

            try
            {
                var exchangeRateApiUrl = _configuration["ExternalApis:ExchangeRateApiUrl"];
                var url = $"{exchangeRateApiUrl}/{baseCurrency}";
                apiLog.RequestUrl = url;

                _logger.LogInformation("Calling Exchange Rate API: {Url}", url);

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(url);

                stopwatch.Stop();
                apiLog.ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                apiLog.StatusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    apiLog.ResponseBody = content;
                    apiLog.IsSuccessful = true;

                    var exchangeRateResponse = JsonSerializer.Deserialize<ExchangeRateResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Log to database
                    _context.ExternalApiLogs.Add(apiLog);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Exchange Rate API call successful. Response time: {ResponseTime}ms", apiLog.ResponseTimeMs);

                    return Ok(exchangeRateResponse);
                }
                else
                {
                    apiLog.IsSuccessful = false;
                    apiLog.ErrorMessage = $"API returned status code: {response.StatusCode}";
                    _context.ExternalApiLogs.Add(apiLog);
                    await _context.SaveChangesAsync();

                    _logger.LogWarning("Exchange Rate API call failed. Status: {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, new { message = "Failed to fetch exchange rates" });
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                apiLog.ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                apiLog.IsSuccessful = false;
                apiLog.ErrorMessage = ex.Message;

                _context.ExternalApiLogs.Add(apiLog);
                await _context.SaveChangesAsync();

                _logger.LogError(ex, "Error calling Exchange Rate API");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/ExternalApi/logs
        /// <summary>
        /// Get all API call logs
        /// </summary>
        /// <returns>List of API logs</returns>
        [HttpGet("logs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ExternalApiLog>>> GetApiLogs()
        {
            try
            {
                var logs = await _context.ExternalApiLogs
                    .OrderByDescending(l => l.RequestedAt)
                    .Take(100)
                    .ToListAsync();

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching API logs");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/ExternalApi/logs/{id}
        /// <summary>
        /// Get a specific API log by ID
        /// </summary>
        /// <param name="id">Log ID</param>
        /// <returns>API log details</returns>
        [HttpGet("logs/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExternalApiLog>> GetApiLog(int id)
        {
            try
            {
                var log = await _context.ExternalApiLogs.FindAsync(id);

                if (log == null)
                {
                    return NotFound(new { message = $"Log with ID {id} not found" });
                }

                return Ok(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching API log with ID: {LogId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
