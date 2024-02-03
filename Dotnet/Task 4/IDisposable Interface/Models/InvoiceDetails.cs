using System.ComponentModel.DataAnnotations;

namespace FoodOrderingSystemAPI.Models{
  public class InvoiceDetails
{
    public int invoiceId { get; set; }
    public DateTime? orderDate { get; set; }
    public string orderStatus { get; set; }
    public List<OrderDetails> orderDetails { get; set; }
}
}