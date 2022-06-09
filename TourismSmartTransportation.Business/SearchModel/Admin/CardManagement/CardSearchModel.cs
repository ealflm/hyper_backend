using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.CardManagement
{
    public class CardSearchModel
    {
        public Guid? CustomerId { get; set; }
        public string Uid { get; set; }
        public int? Status { get; set; }
    }
}