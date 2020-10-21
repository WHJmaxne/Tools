using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tool.Azure.Storage
{
    public interface IAzureStorageService
    {
        Task<string> UploadFilesFromStreamAsync(Stream stream, string filename);
        Task<string> LargeFileTransferAsync(string sourcePath);
        Task<bool> RemoveBlobAsync(string path);
        Task<CloudStorageAccount> CreateStorageAccountAsync();
        Task<CloudBlobContainer> CreateCloudBlobContainerAsync();
        Task<string> GetSecureURlAsync(string path);
        Task<CloudBlob> GetCloudBlobAsync(string path);

        string UploadFilesFromStream(Stream stream, string filename);
        CloudStorageAccount CreateStorageAccount();
        CloudBlobContainer CreateCloudBlobContainer();
        CloudBlob GetCloudBlob(string path);
        string GetSecureURl(string path);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="splitChar">default split char '@'</param>
        /// <returns></returns>
        string[] GetSecureArrURl(string path, params char[] splitChar);
    }
}
