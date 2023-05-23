using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ImageManagement.Model
{
    [Table("SourceImage")]
    public class ImageEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonPropertyName("file_name")]
        [Column("file_name")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("image_data")]
        [Column("image_data")]
        public byte[] ImageData { get; set; } = null!;

        [JsonPropertyName("slice_100")]
        [Column("slice_100")]
        public byte[]? Slice100 { get; set; }

        [JsonPropertyName("slice_300")]
        [Column("slice_300")]
        public byte[]? Slice300 { get; set; }
    }
}