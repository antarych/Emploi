namespace Frontend.Models
{
    public class ImageModel
    {
        public ImageModel(string bigPhotoName)
        {
            BigPhotoName = bigPhotoName;
        }

        public string BigPhotoName { get; set; }
    }
}