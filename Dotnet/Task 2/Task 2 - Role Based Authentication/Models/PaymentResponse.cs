using System.ComponentModel.DataAnnotations;

namespace FoodOrderingSystemAPI.Models{
    public class PaymentResponse
    {
      [Required]  
        public string razorpayPaymentId { get; set; }
        [Required]  
        public int userId { get; set; }
    }

}