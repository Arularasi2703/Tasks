using System.ComponentModel.DataAnnotations;

namespace FoodOrderingSystemAPI.Models{
  public class FoodItemRequest
    {
        public string name { get; set; }
        
        public int price { get; set; }
        
        public string description { get; set; }
        
        public IFormFile image { get; set; }
        
        public string category { get; set; }

        public bool isVegan { get; set; }

        public float calories { get; set; }
    }
}