
using Microsoft.AspNetCore.Http;

namespace Accelerate.Foundations.Media.Services
{
    public interface IMediaService
    {
        Task<List<string>> UploadVideos(Guid userId, Dictionary<Guid, IFormFile> files);
        Task<List<string>> UploadImages(Guid userId, Dictionary<Guid, IFormFile> files);
        Task<List<string>> UploadFiles(Guid userId, Dictionary<Guid, IFormFile> files);
        Task<string> UploadFile(Guid fileId, string userId, IFormFile file);
        Task<string> UploadFile(string userId, IFormFile file);
        string GetPrivateLocation(string fileName);
        Task<string> GetPublicLocation(string id);
        string GetFileUrl(string id);
    }
}
