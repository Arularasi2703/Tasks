using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FoodOrderingSystemAPI.Models{
  public class Login{

    [StringLength(50)]
    [Required(ErrorMessage = "Email Required")]
    public string email { get; set; }

    [Required(ErrorMessage = "Password Required")]
    [DataType(DataType.Password)]
    public string password { get; set; }
    [BindNever]
    public bool rememberMe { get; set; }
    public bool isAdmin{get;set;}
  }

}