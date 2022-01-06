using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Common;

namespace TourismSmartTransportation.Business.Interfaces
{
    public interface IUploadFileService
    {
        Task<string> Upload(UploadFileSearchModel model);
    }
}
