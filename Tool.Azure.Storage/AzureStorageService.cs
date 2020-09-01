using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tool.Azure.Storage
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly StorageOptions _option;
        private CloudStorageAccount _cloudStorageAccount;
        private readonly ILogger<AzureStorageService> _logger;
        private string _containerName;
        public string ContainerName
        {
            get
            {
                return _containerName;
            }
            set
            {
                _containerName = value.ToLower();
            }
        }
        public AzureStorageService(IOptions<StorageOptions> option, ILogger<AzureStorageService> logger)
        {
            this._option = option.Value;
            this._logger = logger;
            this.ContainerName = option.Value.ContainerName;
        }
        public async Task<CloudStorageAccount> CreateStorageAccountAsync()
        {
            return await Task.FromResult(this.CreateStorageAccount());
        }
        public CloudStorageAccount CreateStorageAccount()
        {
            if (this._cloudStorageAccount == null)
            {
                CloudStorageAccount.TryParse(this._option.ConnectionString, out this._cloudStorageAccount);
            }
            return this._cloudStorageAccount ?? throw new Exception("CloudStorageAccount is null");
        }

        public async Task<CloudBlobContainer> CreateCloudBlobContainerAsync()
        {
            CloudStorageAccount storageAccount = await this.CreateStorageAccountAsync();
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(this.ContainerName);
            container.CreateIfNotExists(_option.BlobContainerPublicAccessType);
            return container;
        }

        public CloudBlobContainer CreateCloudBlobContainer()
        {
            CloudStorageAccount storageAccount = this.CreateStorageAccount();
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(this.ContainerName);
            container.CreateIfNotExists(_option.BlobContainerPublicAccessType);
            return container;
        }

        public async Task<bool> RemoveBlobAsync(string path)
        {
            try
            {
                var blob = await this.GetCloudBlobAsync(path);
                return await blob.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task<string> UploadFilesFromStreamAsync(Stream stream, string filename)
        {
            try
            {
                CloudBlobContainer container = await this.CreateCloudBlobContainerAsync();
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);
                stream.Position = 0;
                await blockBlob.UploadFromStreamAsync(stream);
                return this._option.StorageEndpoint + "/" + this.ContainerName + "/" + filename;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return string.Empty;
            }
        }

        public string UploadFilesFromStream(Stream stream, string filename)
        {
            try
            {
                CloudBlobContainer container = this.CreateCloudBlobContainer();
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);
                stream.Position = 0;
                blockBlob.UploadFromStream(stream);
                return this._option.StorageEndpoint + "/" + this.ContainerName + "/" + filename;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return string.Empty;
            }
        }

        public async Task<CloudBlob> GetCloudBlobAsync(string path)
        {
            var container = await this.CreateCloudBlobContainerAsync();
            string blobName = this.GetFullBlobName(path);
            var blob = container.GetBlobReference(blobName);
            return blob;
        }

        public CloudBlob GetCloudBlob(string path)
        {
            var container = this.CreateCloudBlobContainer();
            string blobName = this.GetFullBlobName(path);
            return container.GetBlobReference(blobName);
        }

        public async Task<string> GetSecureURlAsync(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return string.Empty;

                var blob = await this.GetCloudBlobAsync(path);

                var sas = blob.GetSharedAccessSignature(this._option.SharedAccessBlobPolicy);
                var secureURl = blob.Uri.AbsoluteUri + sas;
                return secureURl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return string.Empty;
            }
        }

        public string GetSecureURl(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return string.Empty;

                var blob = this.GetCloudBlob(path);
                var sas = blob.GetSharedAccessSignature(this._option.SharedAccessBlobPolicy);
                var secureURl = blob.Uri.AbsoluteUri + sas;
                return secureURl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return string.Empty;
            }
        }

        public string[] GetSecureArrURl(string path, params char[] splitChar)
        {
            string[] arr = path.Split(splitChar);
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = this.GetSecureURl(arr[i]);
            }
            return arr;
        }
        private string GetFullBlobName(string path)
        {
            Uri uri = new Uri(path);
            string blobName = string.Empty;
            for (int i = 0; i < uri.Segments.Length; i++)
            {
                if (i > 1)
                    blobName += uri.Segments[i];
            }
            return blobName;
        }

    }
}
