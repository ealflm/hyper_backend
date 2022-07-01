using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.SearchModel.Admin.CategoryManagement
{
    public class CreateCategoryModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
