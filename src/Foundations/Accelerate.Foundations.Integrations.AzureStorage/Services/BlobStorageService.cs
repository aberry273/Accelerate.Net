using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Integrations.AzureStorage.Models;
using Azure;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static System.Reflection.Metadata.BlobBuilder;

namespace Accelerate.Foundations.Integrations.AzureStorage.Services
{ 
    public class BlobStorageService : IBlobStorageService
    { 
        string appUserContainerImagesSubfolderName = "images";
        string appUserContainerVideosSubfolderName = "videos";
        string appUserContainerOtherSubfolderName = "other";
        string accessKey = string.Empty;
        private readonly AzureStorageConfiguration _config;
        private readonly ILogger<BlobStorageService> _logger;
        private BlobServiceClient _client;
        private BlobContainerClient _container;
        BlobUploadOptions _options = new BlobUploadOptions
        {
            TransferOptions = new StorageTransferOptions
            {
                // Set the maximum number of workers that 
                // may be used in a parallel transfer.
                MaximumConcurrency = 8,

                // Set the maximum length of a transfer to 50MB.
                MaximumTransferSize = 50 * 1024 * 1024
            }
        };
        public BlobStorageService(
          ILogger<BlobStorageService> logger, IOptions<AzureStorageConfiguration> options)
        {
            _logger = logger;
            _config = options.Value;
            this.accessKey = _config.AccessKey;
        }
        private async Task CreateAppContainer()
        {
            try
            {
                if (this._client == null)
                {
                    this._client = new BlobServiceClient(_config.ConnectionString);
                }
                var container = _client.GetBlobContainerClient(Constants.Settings.AppContainerName);
                if(container == null)
                {
                    container = await _client.CreateBlobContainerAsync(Constants.Settings.AppContainerName);
                }
                this._container = container;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public async Task<BlobContainerClient> GetContainer()
        {
            if (_container == null) await CreateAppContainer();
            return _container;
        } 

        public async Task<string?> UploadOther(Guid fileId, string userId, string strFileName, byte[] fileData, string fileMimeType)
        {
            try
            {
                return await this.UploadAsync(fileId, GetOtherPath(userId, strFileName), fileData, fileMimeType);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        // Generates its own guid
        public async Task<string?> UploadOther(string userId, string strFileName, byte[] fileData, string fileMimeType)
        {
            try
            {
                return await this.UploadAsync(GetOtherPath(userId, strFileName), fileData, fileMimeType);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public async Task<string?> UploadVideo(Guid fileId, string userId, string strFileName, byte[] fileData, string fileMimeType = "mp4")
        {
            try
            {
                return await this.UploadAsync(fileId, GetVideoPath(userId, strFileName), fileData, fileMimeType);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        // Generates its own guid
        public async Task<string?> UploadVideo(string userId, string strFileName, byte[] fileData, string fileMimeType = "mp4")
        {
            try
            {
                return await this.UploadAsync(GetVideoPath(userId, strFileName), fileData, fileMimeType);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public async Task<string?> UploadImage(Guid fileId, string userId, string strFileName, byte[] fileData, string fileMimeType = "png")
        {
            try
            {
                return await this.UploadAsync(fileId, GetImagePath(userId, strFileName), fileData, fileMimeType);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        // Generates its own guid
        public async Task<string?> UploadImage(string userId, string strFileName, byte[] fileData, string fileMimeType = "png")
        {
            try
            {
                return await this.UploadAsync(GetImagePath(userId, strFileName), fileData, fileMimeType);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public string GetOtherPath(string userId, string strFileName)
        {
            return $"accounts/{userId}/{appUserContainerOtherSubfolderName}/{GenerateFileName(strFileName)}";
        }
        public string GetImagePath(string userId, string strFileName)
        {
            return $"accounts/{userId}/{appUserContainerImagesSubfolderName}/{GenerateFileName(strFileName)}";
        }
        public string GetVideoPath(string userId, string strFileName)
        {
            return $"accounts/{userId}/{appUserContainerVideosSubfolderName}/{GenerateFileName(strFileName)}";
        }
        private string GenerateFileName(string fileName)
        {
            string strFileName = string.Empty;
            string[] strName = fileName.Split('.');
            strFileName = DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + "." + strName[strName.Length - 1];
            return strFileName;
        }
        public Dictionary<string, string> GetBlobMetaTags(string id, string mimeType)
        {
            return new Dictionary<string, string>
            {
                { "id",id },
                { "mimeType", mimeType },
                { "Date", DateTime.Now.ToUniversalTime().ToString() }
            };
        }

        public async Task<BlobClient> GetBlobByTag(TaggedBlobItem taggedBlob)
        {
            if (taggedBlob == null) return null;
            if (_container == null) await CreateAppContainer();
            return _container.GetBlobClient(taggedBlob.BlobName);
        }
        public async Task<TaggedBlobItem> FindBlobByTagId(string id)
        {
            if (_container == null) await CreateAppContainer();
            //string query = @"""Date"" >= '2020-04-20' AND ""Date"" <= '2020-04-30'";
            string query = @"""id"" = '"+id+"'";
            var blobs = await FindBlobByQuery(query);
            return blobs.FirstOrDefault();
        }
        public async Task<List<TaggedBlobItem>> FindBlobByQuery(string query)
        {
            //string query = @"""Date"" >= '2020-04-20' AND ""Date"" <= '2020-04-30'"; 
            List<TaggedBlobItem> blobs = new List<TaggedBlobItem>();
            await foreach (TaggedBlobItem taggedBlobItem in _container.FindBlobsByTagsAsync(query))
            {
                blobs.Add(taggedBlobItem);
            }
            return blobs;
        }
          
        public async Task<List<string>> UploadManyAsync(Guid userId, List<BlobFile> files)
        {
            try
            {
                if (_container == null) await CreateAppContainer();
                
                var tasks = new Queue<Task<Response<BlobContentInfo>>>();
                foreach (BlobFile file in files)
                {
                    Stream stream = new MemoryStream(file.fileData);
                    var blobUploadOptions = new BlobUploadOptions()
                    {
                        Metadata = new Dictionary<string, string> { { "id", file.id.ToString() } },
                        Tags = GetBlobMetaTags(file.id.ToString(), file.fileMimeType)
                    };
                    
                    BlockBlobClient blob = new BlockBlobClient(_config.ConnectionString, _container.Name, GetOtherPath(userId.ToString(), file.fileName));

                    tasks.Enqueue(blob.UploadAsync(stream, blobUploadOptions)); 
                }

                // Run all the tasks asynchronously.
                await Task.WhenAll(tasks);

                return files.Select(x => x.id.ToString()).ToList();
            }

            catch (RequestFailedException ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError($"Azure request failed: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError($"Error parsing files in the directory: {ex.Message}");
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError($"Exception: {ex.Message}");
            }
            return new List<string>();
        }
        public async Task<string?> UploadAsync(string fullFilePath, byte[] fileData, string fileMimeType)
        {
            try
            { 
                if(_container == null) await CreateAppContainer();
                BlockBlobClient blob = new BlockBlobClient(_config.ConnectionString, _container.Name, fullFilePath);
                //...
                var id = Guid.NewGuid().ToString();
                Stream stream = new MemoryStream(fileData);
                var blobUploadOptions = new BlobUploadOptions()
                {
                    Metadata = new Dictionary<string, string> { { "id", id } },
                    Tags = GetBlobMetaTags(id, fileMimeType)
                };
                var result = await blob.UploadAsync(stream, blobUploadOptions);
                return id; 
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public async Task<string?> UploadAsync(Guid fileId, string fullFilePath, byte[] fileData, string fileMimeType)
        {
            try
            {
                if (_container == null) await CreateAppContainer();
                BlockBlobClient blob = new BlockBlobClient(_config.ConnectionString, _container.Name, fullFilePath);
                //...
                var id = fileId.ToString();
                Stream stream = new MemoryStream(fileData);
                var blobUploadOptions = new BlobUploadOptions()
                {
                    Metadata = new Dictionary<string, string> { { "id", id } },
                    Tags = GetBlobMetaTags(id, fileMimeType)
                };
                var result = await blob.UploadAsync(stream, blobUploadOptions);
                return id;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task<Azure.Response> DeleteOther(string userId, string strFileName)
        {
            return await this.DeleteAsync(GetOtherPath(userId, strFileName));
        }
        public async Task<Azure.Response> DeleteImage(string userId, string strFileName)
        {
            return await this.DeleteAsync(GetImagePath(userId, strFileName));
        }
        public async Task<Azure.Response> DeleteVideo(string userId, string strFileName)
        {
            return await this.DeleteAsync(GetVideoPath(userId, strFileName));
        }

        public async Task<Azure.Response> DeleteAsync(string fileUrl)
        {
            if (_container == null)
            {
                await CreateAppContainer();
            }

            return await _container.DeleteBlobAsync(fileUrl);
        }
    }
}
