using System.ComponentModel.DataAnnotations;

namespace FoodOrderingSystemAPI.Models
{
    public class Cart
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }

        public string imageUrl { get; set; }

        public float price { get; set; }

        public int quantity { get; set; }

        public float totalAmount { get; set; }
        public int foodItemId{get;set;}
        public int userId {get;set;}
        public bool IsCheckedOut { get; set; }


    }
}
