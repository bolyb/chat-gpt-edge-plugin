namespace ChatGPTEdge.Api.Managers
{
    public interface IDenseCaptionManager
    {
        public DenseCaptionsResponse GetDenseCaptions(string imageUri);
    }
}
