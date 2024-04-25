using Accelerate.Foundations.Integrations.AzureStorage.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.AzureStorage.Services
{
    public interface IBlobStorageService
    {
        Task<List<string>> UploadManyAsync(Guid userId, List<BlobFile> files);
        Task<string?> UploadAsync(Guid fileId, string fullFilePath, byte[] fileData, string fileMimeType);
        Task<string?> UploadOther(Guid fileId, string userId, string strFileName, byte[] fileData, string fileMimeType);
        Task<string?> UploadVideo(Guid fileId, string userId, string strFileName, byte[] fileData, string fileMimeType = "mp4");
        Task<string?> UploadImage(Guid fileId, string userId, string strFileName, byte[] fileData, string fileMimeType = "png");
        Task<string?> UploadOther(string userId, string strFileName, byte[] fileData, string fileMimeType);
        Task<string?> UploadVideo(string userId, string strFileName, byte[] fileData, string fileMimeType = "mp4");
        Task<string?> UploadImage(string userId, string strFileName, byte[] fileData, string fileMimeType = "png");
        Task<string?> UploadAsync(string fullFilePath, byte[] fileData, string fileMimeType);
        Task<Azure.Response> DeleteOther(string userId, string strFileName);
        Task<Azure.Response> DeleteImage(string userId, string strFileName);
        Task<Azure.Response> DeleteVideo(string userId, string strFileName);
        Task<Azure.Response> DeleteAsync(string fileUrl);
        Task<TaggedBlobItem> FindBlobByTagId(string id);
        Task<List<TaggedBlobItem>> FindBlobByQuery(string query);
        Task<BlobClient> GetBlobByTag(TaggedBlobItem taggedBlob);
        Task<BlobContainerClient> GetContainer();
        string GetOtherPath(string userId, string strFileName);
        string GetImagePath(string userId, string strFileName);
        string GetVideoPath(string userId, string strFileName);
    }
}
