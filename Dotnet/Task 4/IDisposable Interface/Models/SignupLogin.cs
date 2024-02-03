using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingSystemAPI.Models
{
    public class SignupLogin
    {
        [Key]
        public int userId { get; set; }
        [StringLength(50)]
        [RegularExpression(@"^[A-Z][a-zA-Z]{2,}\s[A-Z][a-zA-Z\s]*$", ErrorMessage = "Invalid Name. Ex: Firstname Lastname")]
        [Required(ErrorMessage = "Name Required")]
        public string name { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Email Required")]
        [RegularExpression(@"^[a-zA-Z0-9]+[a-zA-Z0-9._%+-]*@[a-zA-Z0-9]+([.-][a-zA-Z0-9]+)*\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address")]
        
        public string email { get; set; }

        [Required(ErrorMessage = "Password Required")]
        // [PasswordValidation(ErrorMessage = "Password must be at least 6 characters long. Please choose a stronger password. Try a mix of letters, numbers, and symbols.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{6,}$",ErrorMessage = "Use 6 or more characters with a mix of letters, numbers & symbols")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage ="Confirm Password Required!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{6,}$",ErrorMessage = "Use 6 or more characters with a mix of letters, numbers & symbols")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Password and Confirm Password should be same")]
        public string confirmPassword { get; set; }
        public bool isAdmin { get; set; }

        public bool rememberMe { get; set; } 
    }
}
