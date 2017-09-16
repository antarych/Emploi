using System;

namespace Common
{
    public class Image
    {
        public Image()
        {
            
        }

        public Image(Uri bigImage)
        {
            BigImage = bigImage;
        }

        public virtual Uri BigImage { get; set; }
    }
}
