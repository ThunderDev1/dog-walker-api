using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Api.Settings;

namespace Api.Services
{
    public interface IFileService
    {
        Task<Uri> UploadImage(string containerName, byte[] file, string fileName);
        void DeleteFile(string containerName, string fileName);
        string GetSasUri(string containerName, string uri, SharedAccessBlobPermissions permission = SharedAccessBlobPermissions.Read);
        string GetSasToken(string containerName, SharedAccessBlobPermissions permission = SharedAccessBlobPermissions.Read);
    }

    public class FileService : IFileService
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly StorageSettings _storageSettings;

        public FileService(IOptions<StorageSettings> storageOptions)
        {
            _storageSettings = storageOptions.Value;
            _storageAccount = CloudStorageAccount.Parse(_storageSettings.ConnectionString);
        }

        public async Task<Uri> UploadImage(string containerName, byte[] file, string fileName)
        {
            if (file.Length > _storageSettings.MaxUploadSizeMb * 1024 * 1024)
            {
                return null;
            }

            if (!_storageSettings.AuthorizedImageFormats.Contains(Path.GetExtension(fileName)))
            {
                return null;
            }

            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            // /!\ lowercase name
            CloudBlobContainer container = blobClient.GetContainerReference(containerName.ToLower());

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            await blockBlob.DeleteIfExistsAsync();

            // Create or overwrite the blob with contents from a local file.

            await blockBlob.UploadFromByteArrayAsync(file, 0, file.Length);

            return blockBlob.Uri;
        }

        public void DeleteFile(string containerName, string fileName)
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            // /!\ lowercase name
            CloudBlobContainer container = blobClient.GetContainerReference(containerName.ToLower());

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            blockBlob.DeleteIfExistsAsync();
        }

        public string GetSasUri(string containerName, string uri, SharedAccessBlobPermissions permission = SharedAccessBlobPermissions.Read)
        {
            if (string.IsNullOrEmpty(uri))
                return null;

            string sasToken = GetSasToken(containerName);

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", uri, sasToken);
        }

        public string GetSasToken(string containerName, SharedAccessBlobPermissions permission = SharedAccessBlobPermissions.Read)
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName.ToLower());

            var sasToken = container.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = permission,
                SharedAccessStartTime = DateTime.Now.AddHours(-2),
                SharedAccessExpiryTime = DateTime.Now.AddHours(2),
            });
            return sasToken;
        }

        public enum ImageUploadStatus
        {
            MaxFileSizeExceeded,
            InvalidFileFormat,
            Success
        }
    }
}