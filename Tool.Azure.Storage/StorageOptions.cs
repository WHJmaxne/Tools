using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Azure.Storage
{
    public class StorageOptions
    {
        public string StorageAccount { get; set; }
        public string StorageEndpoint { get; set; }
        public string ContainerName { get; set; }
        public float BlobExpireSeconds { get; set; }
    }
}
