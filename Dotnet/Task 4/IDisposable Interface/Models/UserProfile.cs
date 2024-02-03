using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingSystemAPI.Models{
  public class UserProfile
  {
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int profileId { get; set; }
      [Required]
      public string name { get; set; }

      [Required]
      [EmailAddress]
      public string email { get; set; }

      public string? mobileNumber { get; set; }
      public byte[]? profilePicture { get; set; }
      public string? address { get; set; }

      public int? pincode { get; set; }

      public int userId { get; set; }
      [ForeignKey("userId")]
      public virtual SignupLogin user { get; set; }

  }

}