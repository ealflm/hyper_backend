using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Admin.Authorization
{
    public class AuthorizationResultViewModel
    {
        public AuthorizationResultViewModel(object data, string message)
        {
            Data = data;
            Message = message;
        }

        public object Data { get; set; }
        public string Message { get; set; }
    }
}
