using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Azure.Storage
{
    public class StorageOptions
    {
        public string ConnectionString { get; set; }

        /// <summary>
        /// ContainerName：default
        /// </summary>
        public string ContainerName { get; set; } = "default";
        /// <summary>
        /// default：BlobContainerPublicAccessType.Off
        /// </summary>
        public BlobContainerPublicAccessType BlobContainerPublicAccessType { get; set; } = BlobContainerPublicAccessType.Off;

        public SharedAccessBlobPermissions SharedAccessBlobPermissions { get; set; } = SharedAccessBlobPermissions.Read;
        public int SharedAccessExpirySeconds { get; set; } = 1800;

    }
}
