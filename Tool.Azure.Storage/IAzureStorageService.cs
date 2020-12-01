using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tool.Azure.Storage
{
    public interface IAzureStorageService
    {
        Task<string> UploadFilesFromStreamAsync(Stream stream, string fileName);
        string UploadFilesFromStream(Stream stream, string fileName);
        Task<bool> RemoveBlobAsync(string path);
        Task<CloudStorageAccount> CreateStorageAccountAsync();
        Task<CloudBlobContainer> CreateCloudBlobContainerAsync();
        Task<string> GetSecureURlAsync(string path);
        Task<CloudBlob> GetCloudBlobAsync(string path);

        Task<string> LargeFileTransferAsync(string sourcePath, string fileName, Action<TransferStatus> ProgressCallback = null);
        string LargeFileTransfer(string sourcePath, string fileName, Action<TransferStatus> ProgressCallback = null);
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
