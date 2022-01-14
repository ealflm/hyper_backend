using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Common
{
    public class FilesViewModel
    {
        public string[] DeleteFiles { get; set; }
        public IFormFile[] UploadFiles {get; set;}
    }
}
