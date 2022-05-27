using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TourismSmartTransportation.Business.ViewModel.Admin.Package
{
    // [ModelBinder(BinderType = typeof(MetadataValueModelBinder))]
    public class CreatePackageModel
    {
        // public Guid Id { get; set; }
        public Guid? TierId { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public string Name { get; set; }
        public decimal Limit { get; set; }
        public decimal Value { get; set; }
        public int? Status { get; set; }
    }
}
