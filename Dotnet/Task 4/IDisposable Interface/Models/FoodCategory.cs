using System.ComponentModel.DataAnnotations;

namespace FoodOrderingSystemAPI.Models{
  public class FoodCategory{
     public string name { get; set; }
    
    public List<FoodItem> items { get; set; }
  }
} 