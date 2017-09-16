using System.IO;


namespace FileManagement
{
    public class ImageResizer:IImageResizer
    {        
        private string GenerateRandomFileName(string fileName)
        {
            var randomFileName = Path.GetRandomFileName();
            return Path.ChangeExtension(randomFileName, Path.GetExtension(fileName).TrimStart('.'));
        }
    }
}

