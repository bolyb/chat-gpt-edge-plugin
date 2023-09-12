namespace ChatGPTEdge.Api
{
    public class DenseCaptionsResponse
    {
        public DenseCaptionsResult? DenseCaptionsResult { get; set; }

        public string? ModelVersion { get; set; }

        public DenseCaptionsResultMetadata? Metadata { get; set; }
    }
}
