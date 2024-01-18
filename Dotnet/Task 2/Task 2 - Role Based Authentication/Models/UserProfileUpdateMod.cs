using System.ComponentModel.DataAnnotations;

namespace FoodOrderingSystemAPI.Models
{
    public class UpdateUserProfileModel
    {
        public int profileId { get; set; }
        public int userId{get;set;}

        [Required]
        public string name { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        public string mobileNumber { get; set; }
        public IFormFile profilePicture { get; set; }
        public string address { get; set; }
        public int? pincode { get; set; }
    }
}
  