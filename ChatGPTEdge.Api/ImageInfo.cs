namespace ChatGPTEdge.Api
{
    public class ImageInfo
    {
        public string ImageUrl { get; set; }
        public string? Location { get; set; }

        public ImageInfo(string imageUrl, string? location = null)
        {
            this.ImageUrl = imageUrl;
            this.Location = location;
        }
 
    }
}
