
using Accelerate.Foundations.Integrations.AzureStorage.Models;
using Accelerate.Foundations.Media.Models.Data;
using ImageMagick;
using Microsoft.AspNetCore.Http;

namespace Accelerate.Foundations.Media.Services
{
    public interface IMediaService
    {
        Task<List<MediaBlobUploadResult>> UploadImages(Guid userId, IEnumerable<MediaBlobUploadRequest> files);
        Task<List<MediaBlobUploadResult>> UploadVideos(Guid userId, IEnumerable<MediaBlobUploadRequest> files);
        Task<List<MediaBlobUploadResult>> UploadFiles(Guid userId, IEnumerable<MediaBlobUploadRequest> files);
        Task<string> UploadFile(Guid fileId, string userId, IFormFile file);
        Task<string> UploadFile(string userId, IFormFile file);
        string GetPrivateLocation(string fileName);
        Task<string> GetPublicLocation(string id);
        string GetFileUrl(string id);
        bool FileExists(string filePath);

        string GetParameterPath(string filePath, int h, int w);
        string ResizeImage(string filePath, int h, int w);
        byte[] CompressImage(IFormFile file);
    }
}
