using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingSystemAPI.Models{
public class Checkout
{
    [Key]
    public int id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [RegularExpression(@"^[A-Z][a-zA-Z]{2,}\s[A-Z][a-zA-Z\s]*$", ErrorMessage = "Invalid Name. Ex: Firstname Lastname")]
    public string name { get; set; }


    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9._%+-]*@[a-zA-Z0-9]+([.-][a-zA-Z0-9]+)*\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address")]
    public string email { get; set; }

   
    [RegularExpression(@"^\+91[6-9]\d{9}$", ErrorMessage = "Invalid mobile number format. Ex: +918989898989")]
    [Required(ErrorMessage = "Mobile number is required")]
    public string mobileNumber { get; set; }


    [Required(ErrorMessage = "Address is required")]
    public string address { get; set; }

    [Required(ErrorMessage = "Pin code is required")]
    [RegularExpression(@"^[1-9]\d{5}$", ErrorMessage = "Invalid pincode format. Ex: 607001")]
    public string pincode { get; set; }

    public int userId { get; set; }

}

}
