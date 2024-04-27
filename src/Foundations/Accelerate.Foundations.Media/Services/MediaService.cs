
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Integrations.AzureStorage.Models;
using Accelerate.Foundations.Integrations.AzureStorage.Services;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using static Accelerate.Foundations.Integrations.AzureStorage.Constants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Accelerate.Foundations.Media.Services
{
    public class MediaService : IMediaService
    {
        private static Dictionary<string, string> _publicFiles = new Dictionary<string, string>();
        IMetaContentService _contentService;
        IBlobStorageService _blobStorageService;
        private FileExtensionContentTypeProvider _contentTypeProvider;
        public MediaService(
            IMetaContentService contentService,
            IBlobStorageService blobStorageService
        )
        {
            _contentService = contentService;
            _blobStorageService = blobStorageService;
            this._contentTypeProvider = new FileExtensionContentTypeProvider();
        }
        private string GetFileType(string fileName)
        {
            string contentType;
            if (!_contentTypeProvider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
        public async Task<List<string>> UploadVideos(Guid userId, Dictionary<Guid, IFormFile> files)
        {
            var blobFiles = files.Select(x => CreateVideoBlob(userId, x.Key, x.Value)).ToList();
            return await _blobStorageService.UploadManyAsync(userId, blobFiles);
        }
        public async Task<List<string>> UploadImages(Guid userId, Dictionary<Guid, IFormFile> files)
        {
            var blobFiles = files.Select(x => CreateImageBlob(userId, x.Key, x.Value)).ToList();
            return await _blobStorageService.UploadManyAsync(userId, blobFiles);
        }
        public async Task<List<string>> UploadFiles(Guid userId, Dictionary<Guid, IFormFile> files)
        {
            var blobFiles = files.Select(x => CreateFileBlob(userId, x.Key, x.Value)).ToList();
            return await _blobStorageService.UploadManyAsync(userId, blobFiles);
        }
        private BlobFile CreateImageBlob(Guid userId, Guid blobId, IFormFile file)
        {
            return CreateBlob(userId, blobId, file, _blobStorageService.GetImagePath(userId.ToString(), file.FileName));
        }
        private BlobFile CreateVideoBlob(Guid userId, Guid blobId, IFormFile file)
        {
            return CreateBlob(userId, blobId, file, _blobStorageService.GetVideoPath(userId.ToString(), file.FileName));
        }
        private BlobFile CreateFileBlob(Guid userId, Guid blobId, IFormFile file)
        {
            return CreateBlob(userId, blobId, file, _blobStorageService.GetOtherPath(userId.ToString(), file.FileName));
        }
        private BlobFile CreateBlob(Guid userId, Guid blobId, IFormFile file, string blobPath)
        {
            byte[] data;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                data = ms.ToArray();
            }
            return new BlobFile()
            {
                id = blobId,
                fileName = file.FileName,
                fileMimeType = GetFileType(file.FileName),
                fileData = data,
                blobPath = blobPath
            };
        }
        public async Task<string> UploadFile(Guid fileId, string userId, IFormFile file)
        {
            // Upload media
            byte[] data;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                data = ms.ToArray();
            }
            return await _blobStorageService.UploadOther(fileId, userId, file.FileName, data, GetFileType(file.FileName));
        }
        public async Task<string> UploadFile(string userId, IFormFile file)
        {
            // Upload media
            byte[] data;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                data = ms.ToArray();
            }
            return await _blobStorageService.UploadOther(userId, file.FileName, data, GetFileType(file.FileName));
        }
        public string GetPrivateLocation(string fileName)
        {
            return _publicFiles.FirstOrDefault(x => x.Value == fileName).Key;
        }
        public string GetFileUrl(string id)
        {
            return $"https://localhost:7220/media/files/{id}";
        }
        public async Task<string> GetPublicLocation(string id)
        {
            var container = await _blobStorageService.GetContainer();
            if (_publicFiles.ContainsKey(id)) return _publicFiles[id];

            string downloadPath = GetTempPath(id);
            var taggedBlob = await _blobStorageService.FindBlobByTagId(id);
            if(taggedBlob == null)
            {
                return null;
            }
            var blob = await _blobStorageService.GetBlobByTag(taggedBlob);
            await blob.DownloadToAsync(downloadPath);
            _publicFiles.Add(id, downloadPath);
            return downloadPath;
        }
        private string GetTempPath(string name)
        {
            try
            {
                var temp = Path.GetTempPath();
                var fileName = name.Split("/").LastOrDefault();
                var path = @$"{temp}/{fileName}";
                return path;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
