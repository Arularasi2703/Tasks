using System.ComponentModel.DataAnnotations;

namespace FoodOrderingSystemAPI.Models{
  public class FoodItem{
     [Key]
    public int id { get; set; }
    
    [Required]
    public string name { get; set; }

    [Required]    
    public int price { get; set; }
    
    [Required]
    public string description { get; set; }
    
    [Required]
    public byte[] imageUrl { get; set; }
    
    [Required]
    public string category { get; set; }

    [Required]
    public bool isVegan { get; set; }

    [Required]
    public float calories { get; set; }
    public bool IsInGallery { get; set; }
  }
}