using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ChatGPTEdge.Api.Managers
{
    public class DenseCaptionManager : IDenseCaptionManager
    {
        private readonly ILogger<DenseCaptionManager> logger;
        public DenseCaptionManager(ILogger<DenseCaptionManager> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Get DenseCaptionsResponse using Image Analysis 4.0 API 
        /// </summary>
        public DenseCaptionsResponse GetDenseCaptions(string imageUrl)
        {
            // To set the environment variable for your key and endpoint, open a console window and run
            // setx VISION_KEY your-key 
            // setx VISION_ENDPOINT your-endpoint
            // For VS Local Debugging we may need to add Environment Variables under launchSettings.json depending on which ActiveDebugProfile we are using
            var serviceOptions = new VisionServiceOptions(
            Environment.GetEnvironmentVariable("VISION_ENDPOINT"),
            new AzureKeyCredential(Environment.GetEnvironmentVariable("VISION_KEY")));

            using var imageSource = VisionSource.FromUrl(new Uri(imageUrl));

            var analysisOptions = new ImageAnalysisOptions()
            {
                Features = ImageAnalysisFeature.DenseCaptions,

                Language = "en",

                GenderNeutralCaption = true
            };

            using var analyzer = new ImageAnalyzer(serviceOptions, imageSource, analysisOptions);

            var result = analyzer.Analyze();

            if (result.Reason == ImageAnalysisResultReason.Analyzed)
            {
                List<DenseCaption> denseCaptions = new();
                if (result.DenseCaptions != null)
                {
                    logger.LogInformation(" Successfully retrieved Dense Captions. ");
                    foreach (var caption in result.DenseCaptions)
                    {
                        denseCaptions.Add(new DenseCaption
                        {
                            Text = caption.Content,
                            Confidence = caption.Confidence,
                            BoundingBox = new DenseCaptionBoundingBox { X = caption.BoundingBox.X, Y = caption.BoundingBox.Y, W = caption.BoundingBox.Width, H = caption.BoundingBox.Height }
                        });
                    }

                    var denseCaptionsResponse = new DenseCaptionsResponse
                    {
                        DenseCaptionsResult = new DenseCaptionsResult { Values = denseCaptions },
                        ModelVersion = result.ModelVersion,
                        Metadata = new DenseCaptionsResultMetadata { Width = result.ImageWidth ?? 0, Height = result.ImageHeight ?? 0 }
                    };

                    // Serialize the DenseCaptionsResponse object to a JSON string
                    string json = JsonConvert.SerializeObject(denseCaptionsResponse, Formatting.Indented);

                    // Save the JSON string to a file DenseCaptionsResult.json
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "DenseCaptionsResult.json");
                    File.WriteAllText(filePath, json);

                    logger.LogInformation($"Saved DenseCaptionsResult to {filePath}");

                    return denseCaptionsResponse;
                }
                else
                {
                    logger.LogWarning("Analysis succeeded but no Dense Captions were retrieved.");
                }
            }
            else
            {
                var errorDetails = ImageAnalysisErrorDetails.FromResult(result);
                logger.LogWarning($" Analysis failed.    Error reason : {errorDetails.Reason}    Error code : {errorDetails.ErrorCode}   Error message: {errorDetails.Message}");
            }
            throw new Exception("Unable to get image analysis result.");
        }

    }
}
