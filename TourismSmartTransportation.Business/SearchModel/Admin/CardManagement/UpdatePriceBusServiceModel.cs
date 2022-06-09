using System;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.CardManagement
{
    public class UpdateCardModel
    {
        public Guid? CustomerId { get; set; }
        public string Uid { get; set; }
        public int? Status { get; set; }
    }
}