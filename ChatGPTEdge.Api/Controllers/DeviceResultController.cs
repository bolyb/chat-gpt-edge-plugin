using ChatGPTEdge.Api.Managers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatGPTEdge.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceResultController : ControllerBase
    {
        private readonly ILogger<DeviceResultController> _logger;

        private readonly IDenseCaptionManager denseCaptionManager;

        public DeviceResultController(ILogger<DeviceResultController> logger, IDenseCaptionManager denseCaptionManager)
        {
            _logger = logger;
            this.denseCaptionManager = denseCaptionManager;
        }

        /// <summary>
        /// Get the list of deviceResults
        /// </summary>
        [HttpGet(Name = "GetDenseCaptions")]
        public IEnumerable<DeviceResult> Get(string imageUrl, string? location = null)
        {
            List<DeviceResult> response = new List<DeviceResult>();
            DenseCaptionsResponse denseCaptionsResponse = this.denseCaptionManager.GetDenseCaptions(imageUrl);
            response.Add(new DeviceResult() { Location = location, DenseCaptionsResponse = denseCaptionsResponse });
            //using (StreamReader r = new StreamReader("DenseCaptionsResult.json"))
            //{
            //    string json = r.ReadToEnd();
            //    DenseCaptionsResponse? result = JsonConvert.DeserializeObject<DenseCaptionsResponse>(json);
            //    response.Add(new DeviceResult() { Location = "Farm Crop", DenseCaptionsResponse = result });
            //}
            return response;
        }

        [HttpGet("swagger.json")]
        public IActionResult ServeOpenApiJson()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "swagger.json");
                if (System.IO.File.Exists(filePath))
                {
                    return PhysicalFile(filePath, "application/json");
                }
                else
                {
                    return NotFound("File not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}