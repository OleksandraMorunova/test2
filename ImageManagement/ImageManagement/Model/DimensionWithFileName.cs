namespace ImageManagement.Model
{
    public class DimensionWithFileName
    {
        public string FileName { get; set; } = string.Empty;
        public byte[]? Slice { get; set; }
    }
}
