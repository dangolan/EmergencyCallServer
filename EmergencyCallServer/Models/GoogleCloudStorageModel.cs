using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EmergencyCallServer.Models
{
    public class GoogleCloudStorageModel
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public GoogleCloudStorageModel(string bucketName)
        {
            _bucketName = bucketName;
            _storageClient = StorageClient.Create();
        }

        public async Task<string> UploadPhotoAsync(IFormFile photo, string fileName)
        {
            // Check if the file is a PNG
            if (photo.ContentType != "image/png")
            {
                throw new ArgumentException("Only PNG files are allowed.");
            }

            var contentType = photo.ContentType;

            try
            {
                using (var stream = photo.OpenReadStream())
                {
                    // Append ".png" to the filename
                    var objectName = $"{fileName}.png";

                    // Upload the file to the specified bucket
                    await _storageClient.UploadObjectAsync(_bucketName, objectName, contentType, stream);
                }

            // Return the URL to access the uploaded file
                return $"https://storage.cloud.google.com/{_bucketName}/{fileName}.png";
            }
            catch (Google.GoogleApiException ex)
            {
                // Handle Google API specific exceptions
                throw new Exception("An error occurred while uploading the file to Google Cloud Storage.", ex);
            }
            catch (Exception ex)
            {
                // Handle other possible exceptions
                throw new Exception("An unexpected error occurred.", ex);
            }
        }


        public async Task<bool> DeletePhotoAsync(string fileName)
        {
            try
            {
                string objName = $"{fileName}.png"; 
                await _storageClient.DeleteObjectAsync(_bucketName, fileName);
                return true;
            }
            catch (Google.GoogleApiException e) when (e.Error.Code == 404)
            {
                // Object not found
                return false;
            }
        }
    }
}
