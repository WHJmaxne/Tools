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
        /// <summary>
        /// Permissions : read
        /// SharedAccessStartTime：DateTime.UtcNow
        /// SharedAccessExpiryTime: 1800s
        /// </summary>
        public SharedAccessBlobPolicy SharedAccessBlobPolicy { get; set; } = new SharedAccessBlobPolicy
        {
            Permissions = SharedAccessBlobPermissions.Read,
            SharedAccessStartTime = DateTime.UtcNow,
            SharedAccessExpiryTime = DateTime.UtcNow.AddSeconds(1800)
        };
    }
}
