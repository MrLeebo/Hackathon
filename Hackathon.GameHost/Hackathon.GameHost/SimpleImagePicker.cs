namespace Hackathon.GameHost
{
    public class SimpleImagePicker : IImagePicker
    {
        public ImageData Pick()
        {
            return new ImageData { Url = "http://www.google.com/images/srpr/logo3w.png" };
        }
    }
}
