using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.BasePriceOfBusService
{
    public class UpdateBasePriceOfBusService
    {
        public decimal? MaxDistance { get; set; }

        [ValidValueMinMax("MaxDistance", ErrorMessage = "MinDistance cannot be greater than MaxDistance")]
        public decimal? MinDistance { get; set; }

        public decimal? Price { get; set; }

        public int? Status { get; set; }
    }
}