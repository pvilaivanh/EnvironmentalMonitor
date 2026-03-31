using Microsoft.AspNetCore.Mvc;

namespace EnvironmentalMonitor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly string _reportDirectory;

        public ReportsController()
        {
            // This MUST match where your reports are saved
            _reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "EnvironmentalMonitor", "Reports");

            Console.WriteLine(_reportDirectory);

            // Create folder if it doesn't exist
            if (!Directory.Exists(_reportDirectory))
            {
                Directory.CreateDirectory(_reportDirectory);
            }
        }

        // GET: api/reports
        [HttpGet]
        public IActionResult GetReports()
        {
            var files = Directory.GetFiles(_reportDirectory, "*.json", SearchOption.AllDirectories);

            var reports = new List<object>();

            foreach (var file in files)
            {
                try
                {
                    var json = System.IO.File.ReadAllText(file);

                    var data = System.Text.Json.JsonSerializer.Deserialize<object>(json);

                    if (data != null)
                    {
                        reports.Add(data);
                    }
                }
                catch
                {
                    // skip files that can't be read or deserialized
                }
            }

            return Ok(reports);
        }

        // GET: api/reports/{*fileName}
        [HttpGet("{*fileName}")]
        public IActionResult GetReport(string fileName)
        {
            var filePath = Path.Combine(_reportDirectory, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Report not found");
            }

            var content = System.IO.File.ReadAllText(filePath);

            return Ok(content);
        }
    }
}