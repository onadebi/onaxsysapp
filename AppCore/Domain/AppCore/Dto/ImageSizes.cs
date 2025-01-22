namespace AppCore.Domain.AppCore.Dto
{
    public static class ImageSizes
    {

        public static Dictionary<string, string> ImageSizesList
        {
            get
            {

                return new Dictionary<string, string>
                {
                    {"Small","256x256"},
                    {"Medium","512x512"},
                    {"Large","1024x1024"},
                };
            }
        }

    }
}
