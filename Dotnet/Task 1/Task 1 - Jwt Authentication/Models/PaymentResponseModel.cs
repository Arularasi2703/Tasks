using System.ComponentModel.DataAnnotations;

namespace FoodOrderingSystemAPI.Models{

    public class PaymentResponseModel
    {
        public string message { get; set; }
        public List<OrderDetails> orderDetails { get; set; }
    }
}
