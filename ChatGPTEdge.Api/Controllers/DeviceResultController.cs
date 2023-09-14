using ChatGPTEdge.Api.Managers;
using Microsoft.AspNetCore.Mvc;

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
        public IEnumerable<DeviceResult> Get(string imageUrl, string location)
        {
            List<DeviceResult> response = new List<DeviceResult>();
            DenseCaptionsResponse denseCaptionsResponse = this.denseCaptionManager.GetDenseCaptions(imageUrl);
            response.Add(new DeviceResult() { Location = location, DenseCaptionsResponse = denseCaptionsResponse });
            //using (StreamReader r = new StreamReader("ExampleResult.json"))
            //{
            //    string json = r.ReadToEnd();
            //    DenseCaptionsResponse? result = JsonConvert.DeserializeObject<DenseCaptionsResponse>(json);
            //    response.Add(new DeviceResult() { Location = "Farm Crop", DenseCaptionsResponse = result });
            //}
            return response;
        }
    }
}