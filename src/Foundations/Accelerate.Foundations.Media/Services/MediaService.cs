﻿
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Integrations.AzureStorage.Models;
using Accelerate.Foundations.Integrations.AzureStorage.Services;
using Accelerate.Foundations.Media.EventBus;
using Accelerate.Foundations.Media.Models.Data;
using Accelerate.Foundations.Media.Models.Entities;
using Azure.Storage.Blobs;
using Elastic.Clients.Elasticsearch;
using ImageMagick;
using ImageMagick.ImageOptimizers;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Threading.Tasks;
using static Accelerate.Foundations.Integrations.AzureStorage.Constants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Accelerate.Foundations.Media.Services
{
    public class MediaService : IMediaService
    {
        ImageOptimizer _optimizer;
        SiteConfiguration _siteConfig;
        private static Dictionary<string, string> _publicFiles = new Dictionary<string, string>();
        IMetaContentService _contentService;
        IBlobStorageService _blobStorageService;
        IEntityService<MediaBlobEntity> _entityService;
        Bind<IMediaBlobEventBus, IPublishEndpoint> _publishEndpoint;
        private FileExtensionContentTypeProvider _contentTypeProvider;
        public MediaService(
            IMetaContentService contentService,
            IBlobStorageService blobStorageService,
            Bind<IMediaBlobEventBus, IPublishEndpoint> publishEndpoint,
            IEntityService<MediaBlobEntity> entityService,
            IOptions<SiteConfiguration> siteConfig
        )
        {
            _publishEndpoint = publishEndpoint;
            _contentService = contentService;
            _blobStorageService = blobStorageService;
            _entityService = entityService;
            _siteConfig = siteConfig.Value;
            this._contentTypeProvider = new FileExtensionContentTypeProvider();
            _optimizer = new ImageOptimizer();
        }


        protected async Task PostCreateSteps(MediaBlobEntity obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<MediaBlobEntity>() { Data = obj });
        }
        protected async Task PostUpdateSteps(MediaBlobEntity obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<MediaBlobEntity>() { Data = obj });
        }
        protected async Task PostDeleteSteps(MediaBlobEntity obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<MediaBlobEntity>() { Data = obj });
        }

        public async Task<List<MediaBlobUploadResult>> UploadImagesFromFiles(Guid userId, List<IFormFile> files)
        {
            if (files == null) return new List<MediaBlobUploadResult>();

            // upload file 
            var newFiles = files.Select(x => new MediaBlobUploadRequest(x)).ToList();
            // bulk upload with preset ids
            var fileResults = await this.UploadImages(userId, newFiles);

            // bulk create entities
            var mediaBlobEntities = newFiles.Select(x =>
            {
                return new MediaBlobEntity()
                {
                    Id = x.Id,
                    FilePath = this.GetFileUrl(x.Id.ToString()),
                    Name = x.File.FileName,
                    UserId = userId,
                    Type = MediaBlobFileType.Image
                };
            }).ToList();
            // TODO return IDs of all created entities rather than count
            var blobEntityGuids = await this._entityService.AddRange(mediaBlobEntities);
            //Run pipelines
            var tasks = new List<Task>();
            for (var i = 0; i < mediaBlobEntities.Count; i++)
            {
                tasks.Add(PostCreateSteps(mediaBlobEntities[i]));
            }
            await Task.WhenAll(tasks);
            return fileResults.ToList();

        }

        public async Task<List<MediaBlobUploadResult>> UploadVideosFromFiles(Guid userId, List<IFormFile> files)
        {
            if (files == null) return new List<MediaBlobUploadResult>();

            // upload file 
            var newFiles = files.Select(x => new MediaBlobUploadRequest(x)).ToList();
            // bulk upload with preset ids
            var fileResults = await this.UploadVideos(userId, newFiles);

            // bulk create entities
            var mediaBlobEntities = newFiles.Select(x =>
            {
                return new MediaBlobEntity()
                {
                    Id = x.Id,
                    FilePath = this.GetFileUrl(x.Id.ToString()),
                    Name = x.File.FileName,
                    UserId = userId,
                    Type = MediaBlobFileType.Video
                };
            }).ToList();
            // TODO return IDs of all created entities rather than count
            var blobEntityGuids = await this._entityService.AddRange(mediaBlobEntities);
            //Run pipelines
            var tasks = new List<Task>();
            for (var i = 0; i < mediaBlobEntities.Count; i++)
            {
                tasks.Add(PostCreateSteps(mediaBlobEntities[i]));
            }
            await Task.WhenAll(tasks);
            return fileResults.ToList();
        }


        private MediaBlobUploadResult CreateUploadResult(BlobFile file, bool success)
        {
            return new MediaBlobUploadResult()
            {
                Success = success,
                Id = file.id,
                FilePath = file.fileName,
            };
        }
        public async Task<List<MediaBlobUploadResult>> UploadImages(Guid userId, IEnumerable<MediaBlobUploadRequest> files)
        {
            var blobFiles = files.Select(x => CreateImageBlob(userId, x.Id, x.File)).ToList();
            var uploadResults = await _blobStorageService.UploadManyAsync(userId, blobFiles, BlobFileTypeEnum.Image);
            var results = uploadResults.Select((x, i) => CreateUploadResult(blobFiles.ElementAt(i), x.IsCompleted)).ToList();
            return results;
        }
        public async Task<List<MediaBlobUploadResult>> UploadVideos(Guid userId, IEnumerable<MediaBlobUploadRequest> files)
        {
            var blobFiles = files.Select(x => CreateVideoBlob(userId, x.Id, x.File)).ToList();
            var uploadResults = await _blobStorageService.UploadManyAsync(userId, blobFiles, BlobFileTypeEnum.Video);
            var results = uploadResults.Select((x, i) => CreateUploadResult(blobFiles.ElementAt(i), x.IsCompleted)).ToList();
            return results;
        }
        public async Task<List<MediaBlobUploadResult>> UploadFiles(Guid userId, IEnumerable<MediaBlobUploadRequest> files)
        {
            var blobFiles = files.Select(x => CreateFileBlob(userId, x.Id, x.File)).ToList();
            var uploadResults = await _blobStorageService.UploadManyAsync(userId, blobFiles, BlobFileTypeEnum.File);
            var results = uploadResults.Select((x, i) => CreateUploadResult(blobFiles.ElementAt(i), x.IsCompleted)).ToList();
            return results;
        }
        private BlobFile CreateVideoBlob(Guid userId, Guid blobId, IFormFile file)
        {
            var path = _blobStorageService.GetImagePath(userId.ToString(), file.FileName);
            return CreateBlob(userId, blobId, file, path);
        }
        private BlobFile CreateFileBlob(Guid userId, Guid blobId, IFormFile file)
        {
            var path = _blobStorageService.GetImagePath(userId.ToString(), file.FileName);
            return CreateBlob(userId, blobId, file, path);
        }
        private BlobFile CreateImageBlob(Guid userId, Guid blobId, IFormFile file)
        {
            var path = _blobStorageService.GetImagePath(userId.ToString(), file.FileName);
            return CreateBlob(userId, blobId, file, path);
        }
        private BlobFile CreateBlob(Guid userId, Guid blobId, IFormFile file, string blobPath)
        {
            byte[] data = CompressImage(file);
            return new BlobFile()
            {
                id = blobId,
                fileName = file.FileName,
                fileMimeType = GetFileType(file.FileName),
                fileData = data,
                blobPath = blobPath
            };
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public string GetParameterPath(string filePath, int h, int w)
        {
            return $"{filePath}w={w}h={h}";
        }

        public string ResizeImage(string filePath, int h, int w, bool ignoreAspectRatio)
        {
            /// Read from file
            using var image = new MagickImage(filePath);

            var size = new MagickGeometry(h, w);
            // This will resize the image to a fixed size without maintaining the aspect ratio.
            // Normally an image will be resized to fit inside the specified size.
            size.IgnoreAspectRatio = ignoreAspectRatio;

            image.Resize(size);

            var resizedPath = GetParameterPath(filePath, h, w);
            image.Write(resizedPath);

            return resizedPath;
        } 
        public byte[] CompressImage(IFormFile file)
        {
            byte[] data;
            
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                ms.Position = 0;
                _optimizer.LosslessCompress(ms);
                data = ms.ToArray();
            }
            return data;
        }













        /// <summary>
        /// OLD
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>

        private string GetFileType(string fileName)
        {
            string contentType;
            if (!_contentTypeProvider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
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
            byte[] data = CompressImage(file);
            /*
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                data = ms.ToArray();
            }*/
            return await _blobStorageService.UploadOther(userId, file.FileName, data, GetFileType(file.FileName));
        }
        public string GetPrivateLocation(string fileName)
        {
            return _publicFiles.FirstOrDefault(x => x.Value == fileName).Key;
        }
        public string GetFileUrl(string id)
        {
            return $"{_siteConfig.Domain}/{Constants.Paths.MediaFile}/{id}";
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
