using ImageManagement.Configuration;
using ImageManagement.Model;

namespace ImageManagement.Repository
{
    public class ImageRepositoryIml : IImageRepository
    {
        private readonly MyDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ImageRepositoryIml> _logger;

        public ImageRepositoryIml(MyDbContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<ImageRepositoryIml> logger)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<string> uploadIamge(ImageEntity entity)
        {
            await _dbContext.ImageEntity.AddAsync(entity);
            _dbContext.SaveChanges();
            string path = getUrlServer() + entity.FileName;
            return path;
        }

        public async Task<DimensionWithFileName> getPreView(int id, string size)
        {
            var searchResult = await _dbContext.ImageEntity.FindAsync(id);
            if (searchResult == null)
            {
                _logger.LogError($"Image with id {id}  don't find.");
                return null;
            }
            if (size == "100")
            {
                if (searchResult.Slice100.Equals(null))
                {
                    _logger.LogError($"Image with parametr {size} don't find.");
                    return null;
                }
                return new DimensionWithFileName
                {
                    FileName = searchResult.FileName,
                    Slice = searchResult.Slice100
                };
            }
            if (searchResult.Slice300.Equals(null))
            {
                _logger.LogError($"Image with parametr {size} don't find.");
                return null;
            }
            return new DimensionWithFileName
            {
                FileName = searchResult.FileName,
                Slice = searchResult.Slice300
            };
        }

        public async Task<string> getPathImageById(int id)
        {
            var result = await _dbContext.ImageEntity.FindAsync(id);
            if(result == null)
            {
                _logger.LogError($"Image with id {id} don't find.");
                return null;
            }
            var path = getUrlServer() + result.FileName.ToString();
            return path;
        }

        public async Task<bool> removeImageById(int id)
        {
            var entity = await _dbContext.ImageEntity.FindAsync(id);
            if(entity == null)
            {
                return false;
            }
            _dbContext.ImageEntity.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }

        private string getUrlServer()
        {
            return $"{_httpContextAccessor?.HttpContext?.Request.Scheme}://{_httpContextAccessor?.HttpContext?.Request.Host}/";
        }
    }
}