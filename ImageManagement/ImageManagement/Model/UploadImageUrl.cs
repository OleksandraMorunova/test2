using System.Text.Json.Serialization;

namespace ImageManagement.Model
{
    public class UploadImageUrl
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}