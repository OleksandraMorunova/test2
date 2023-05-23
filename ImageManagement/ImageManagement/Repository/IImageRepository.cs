using ImageManagement.Model;
using Microsoft.AspNetCore.Mvc;

namespace ImageManagement.Repository
{
    public interface IImageRepository
    {
        public Task<string> uploadIamge(ImageEntity entity);
        public Task<DimensionWithFileName> getPreView(int id, string size);
        public Task<string> getPathImageById(int id);
        public Task<bool> removeImageById(int id);
    }
}