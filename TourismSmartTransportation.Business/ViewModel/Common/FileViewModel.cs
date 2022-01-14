﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Common
{
    public class FileViewModel
    {
        public string DeleteFile { get; set; }
        public IFormFile UploadFile { get; set; }
    }
}
