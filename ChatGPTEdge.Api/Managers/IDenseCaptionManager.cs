namespace ChatGPTEdge.Api.Managers
{
    public interface IDenseCaptionManager
    {
        public DenseCaptionsResponse GetDenseCaptionsFromUrl(string imageUri);
        public DenseCaptionsResponse GetDenseCaptionsFromFile(string imagePath);
    }
}
