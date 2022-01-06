using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Business.SearchModel.Common;

namespace TourismSmartTransportation.Business.Implements
{
    public class UploadFileService : IUploadFileService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public UploadFileService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> Upload(UploadFileSearchModel model)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("upload-file");
            if (blobContainer == null)
            {
                return null;
            }
            string fileName = Guid.NewGuid().ToString() + "." + model.ImageFile.ContentType.Substring(6);
            var blobClient = blobContainer.GetBlobClient(fileName);

            await blobClient.UploadAsync(model.ImageFile.OpenReadStream());
            return fileName;
        }
    }
}
