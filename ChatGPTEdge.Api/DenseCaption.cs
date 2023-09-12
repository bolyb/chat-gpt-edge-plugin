namespace ChatGPTEdge.Api
{
    public class DenseCaption
    {
        public string? Text { get; set; }

        public double Confidence { get; set; }

        public DenseCaptionBoundingBox? BoundingBox { get; set; }
    }
}
