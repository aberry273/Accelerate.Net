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
    }
}
