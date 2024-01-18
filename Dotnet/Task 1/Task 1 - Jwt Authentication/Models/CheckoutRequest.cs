using System.ComponentModel.DataAnnotations;

namespace FoodOrderingSystemAPI.Models{
  public class CheckoutRequest
{
    public Checkout checkout { get; set; }
    public List<Cart> cart { get; set; }
}

}