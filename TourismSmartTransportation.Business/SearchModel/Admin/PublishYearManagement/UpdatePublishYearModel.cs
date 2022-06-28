using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PublishYearManagement
{
    public class UpdatePublishYearModel
    {
        [NotAllowedEmptyStringValidator]
        public string Name { get; set; }

        [NotAllowedEmptyStringValidator]
        public string Description { get; set; }

        public int? Status { get; set; }
    }
}
