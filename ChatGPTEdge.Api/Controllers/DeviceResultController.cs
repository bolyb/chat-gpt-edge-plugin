using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatGPTEdge.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceResultController : ControllerBase
    {
        private readonly ILogger<DeviceResultController> _logger;

        public DeviceResultController(ILogger<DeviceResultController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get the list of deviceResults
        /// </summary>
        [HttpGet(Name = "GetDenseCaptions")]
        public IEnumerable<DeviceResult> Get()
        {
            List<DeviceResult> response = new List<DeviceResult>();
            using (StreamReader r = new StreamReader("ExampleResult.json"))
            {
                string json = r.ReadToEnd();
                DenseCaptionsResponse? result = JsonConvert.DeserializeObject<DenseCaptionsResponse>(json);
                response.Add(new DeviceResult() { Location = "Farm Crop", DenseCaptionsResponse = result });
            }

            return response;
        }
    }
}