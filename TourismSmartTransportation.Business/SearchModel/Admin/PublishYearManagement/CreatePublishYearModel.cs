using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PublishYearManagement
{
    public class CreatePublishYearModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
