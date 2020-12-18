using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using trifenix.connect.interfaces.upload;

namespace trifenix.agro.storage.operations
{
    public class UploadImage : IUploadImage
    {
        private readonly string _connectionString;
        public UploadImage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> UploadImageBase64(string base64){
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(_connectionString, out storageAccount)) {
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("contenedor" + Guid.NewGuid().ToString());
                await cloudBlobContainer.CreateIfNotExistsAsync();
                BlobContainerPermissions permissions = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };
                await cloudBlobContainer.SetPermissionsAsync(permissions);

                
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(Guid.NewGuid().ToString() + ".jpg");
                
                var bytes = Convert.FromBase64String(base64);
                using (var stream = new MemoryStream(bytes))
                {
                    await cloudBlockBlob.UploadFromStreamAsync(stream);
                }
                var blobUrl = cloudBlockBlob.Uri.AbsoluteUri;
                

                return blobUrl;
            }
            else
                throw new Exception("Error! Falla en TryParse");
        }

    }
}