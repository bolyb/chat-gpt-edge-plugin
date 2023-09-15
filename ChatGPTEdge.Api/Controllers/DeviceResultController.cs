using ChatGPTEdge.Api.Managers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

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
        /// Get details from a video path locally
        /// </summary>
        [HttpGet(Name = "GetVideoDenseCaptions")]
        public IEnumerable<DeviceResult> Get(string videoPath, string location, string outputFramesFolder)
        {
            // Extract frames and save them as images
            ExtractFramesAndSave(videoPath, outputFramesFolder);

            List<DeviceResult> response = new List<DeviceResult>();

            string[] pngFiles = Directory.GetFiles(outputFramesFolder, "*.png");

            foreach (var imagePath in pngFiles)
            {
                DenseCaptionsResponse denseCaptionsResponse = this.denseCaptionManager.GetDenseCaptionsFromFile(imagePath);
                response.Add(new DeviceResult
                {
                    Location = location,
                    DenseCaptionsResponse = denseCaptionsResponse
                });
            }
            return response;
        }

        /// <summary>
        /// Get the list of image details from a list of image urls
        /// </summary>
        [HttpPost(Name = "PhotoDetails")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IEnumerable<DeviceResult> Post([FromBody] List<ImageInfo> images)
        {
            List<DeviceResult> response = new List<DeviceResult>();
            foreach (var imageInfo in images)
            {
                DenseCaptionsResponse denseCaptionsResponse = this.denseCaptionManager.GetDenseCaptionsFromUrl(imageInfo.ImageUrl);
                response.Add(new DeviceResult
                {
                    Location = imageInfo.Location,
                    DenseCaptionsResponse = denseCaptionsResponse
                });
            }
            // Serialize the DenseCaptionsResponse object to a JSON string
            string json = JsonConvert.SerializeObject(response, Formatting.Indented);

            // Save the JSON string to a file DenseCaptionsResult.json
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "DenseCaptionsResult.json");
            System.IO.File.WriteAllText(filePath, json);

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

        public static void ExtractFramesAndSave(string videoPath, string outputFramesFolder)
        {
            // Make sure the output folder exists
            Directory.CreateDirectory(outputFramesFolder);

            // Path to FFmpeg executable
            string ffmpegPath = "C:\\ProgramData\\chocolatey\\lib\\ffmpeg\\tools\\ffmpeg\\bin\\ffmpeg.exe";

            // Setup FFmpeg process

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = $"-i \"{videoPath}\" -vf \"fps=1/3\" \"{Path.Combine(outputFramesFolder, "frame%04d.png")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                // Start FFmpeg process
                process.Start();

                // Read the Standard Output and Standard Error to see if any error occurs
                string stdout = process.StandardOutput.ReadToEnd();
                string stderr = process.StandardError.ReadToEnd();

                // Wait for FFmpeg to finish
                process.WaitForExit();

                // Check for errors
                if (stderr != "")
                {
                    Console.WriteLine("Error: " + stderr);
                }
                else
                {
                    Console.WriteLine("Processing complete.");
                }
            }
        }
    }
}