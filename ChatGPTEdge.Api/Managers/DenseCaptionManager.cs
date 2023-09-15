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
        private readonly string visionUrl;
        private readonly string visionKey;
        public DenseCaptionManager(ILogger<DenseCaptionManager> logger, IOptions<VisionOptions> serviceOptions)
        {
            if (string.IsNullOrEmpty(serviceOptions.Value.VisionUrl) || serviceOptions.Value.VisionUrl == "<ADD_VISION_URL_HERE>")
            {
                throw new ArgumentException("Error initialize vision url configuration");
            }
            if (string.IsNullOrEmpty(serviceOptions.Value.VisionKey) || serviceOptions.Value.VisionKey == "<ADD_VISION_KEY_HERE>")
            {
                throw new ArgumentException("Error initialize vision key configuration");
            }

            this.visionUrl = serviceOptions.Value.VisionUrl;
            this.visionKey = serviceOptions.Value.VisionKey;

            this.logger = logger;
        }

        /// <summary>
        /// Get DenseCaptionsResponse using Image Analysis 4.0 API from image url
        /// </summary>
        public DenseCaptionsResponse GetDenseCaptionsFromUrl(string imageUrl)
        {
            using var imageSource = VisionSource.FromUrl(new Uri(imageUrl));

            return GetImageAnalyisResults(imageSource);
        }

        /// <summary>
        /// Get DenseCaptionsResponse using Image Analysis 4.0 API from file
        /// </summary>
        public DenseCaptionsResponse GetDenseCaptionsFromFile(string imagePath)
        {
            using var imageSource = VisionSource.FromFile(imagePath);

            return GetImageAnalyisResults(imageSource);
        }

        private DenseCaptionsResponse GetImageAnalyisResults(VisionSource imageSource)
        {
            var serviceOptions = new VisionServiceOptions(visionUrl, new AzureKeyCredential(visionKey));

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
