using ImageManagement.Model;

namespace ImageManagement.Service
{
    public interface IImageService
    {
        public Task<string> uploadIamge(byte[] file, string fileName, string path);
        public Task<DimensionWithFileName> getPreView(string id, string size);
        public Task<string> getPathImageById(string id);
        public Task<bool> removeImageById(string id);
    }
}