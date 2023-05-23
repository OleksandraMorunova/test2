using ImageManagement.Model;
using ImageManagement.Repository;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ImageManagement.Service
{
    public class ImageServiceIml : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<ImageRepositoryIml> _logger;

        public ImageServiceIml(IImageRepository imageRepository, ILogger<ImageRepositoryIml> logger)
        {
            _imageRepository = imageRepository;
            _logger  = logger;
        }

        private static readonly object pblock = new object();

        public async Task<string> uploadIamge(byte[] file, string fileName, string path)
        {
            byte[]? slice100 = null;
            byte[]? slice300 = null;

            Thread thread1 = new Thread(() =>
            {
                lock (pblock)
                {
                    slice100 = ResizeImage(file, 100, 100);
                    _logger.LogInformation("100x100 image sliced.");
                }
            });

            Thread thread2 = new Thread(() =>
            {
                lock (pblock)
                {
                    slice300 = ResizeImage(file, 300, 300);
                    _logger.LogInformation("300x300 image sliced.");
                }
            });

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            var entity = new ImageEntity
            {
                FileName = fileName,
                ImageData = file,    
                Slice100 = slice100,
                Slice300 = slice300
            };

            return await _imageRepository.uploadIamge(entity);
        }

        public static byte[] ResizeImage(byte[] imageData, int newWidth, int newHeight) //This code targets Windows.
        {
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                using (Bitmap originalImage = new Bitmap(ms))
                { 
                    using (Bitmap resizedImage = new Bitmap(newWidth, newHeight))
                    {
                        using (Graphics g = Graphics.FromImage(resizedImage))
                        {
                            g.InterpolationMode = InterpolationMode.Bilinear;
                            g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                        }
                        using (MemoryStream ms2 = new MemoryStream())
                        {
                            resizedImage.Save(ms2, originalImage.RawFormat);
                            byte[] resizedImageData = ms2.ToArray();
                            return resizedImageData;
                        }
                    }
                }
            }
        }

        public async Task<DimensionWithFileName> getPreView(string id, string size)
        {
            if (!int.TryParse(id, out var intId))
            {
                throw new ArgumentException("Input data is not an integer");
            }
            return await _imageRepository.getPreView(intId, size);
        }

        public async Task<string> getPathImageById(string id)
        {
            if (!int.TryParse(id, out var intId))
            {
                throw new ArgumentException("Input data is not an integer");
            }
            return await _imageRepository.getPathImageById(intId);
        }

        public async Task<bool> removeImageById(string id)
        {
            if (!int.TryParse(id, out var intId))
            {
                throw new ArgumentException("Input data is not an integer");
            }
            return await _imageRepository.removeImageById(intId);
        }
    }
}