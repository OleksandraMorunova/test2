namespace ImageManagement.Exeptions
{
    public class ImageException : Exception
    {
        public ImageException(string message) : base(message)
        {
            StatusCode = 400;
        }

        public int StatusCode { get; set; }
    }
}
