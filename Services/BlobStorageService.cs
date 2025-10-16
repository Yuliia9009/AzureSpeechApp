
using Azure.Storage.Blobs;

namespace SpeechApp.API.Services
{
    public class BlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration config)
        {
            _connectionString = config["AzureStorage:ConnectionString"];
            _containerName = config["AzureStorage:ContainerName"];
        }

        public async Task<string> UploadFileAsync(Stream stream, string fileName)
        {
            var containerClient = new BlobContainerClient(_connectionString, _containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(stream, overwrite: true);
            return blobClient.Uri.ToString();
        }
    }
}
