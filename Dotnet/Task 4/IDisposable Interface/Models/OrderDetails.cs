namespace FoodOrderingSystemAPI.Models{
    public class OrderDetails
{
    public int orderId { get; set; }
    public DateTime orderDate { get; set; }
    public string? orderStatus {get;set;}
    public string foodItemName { get; set; }
    public string foodItemDescription{get;set;}
    public byte[] foodItemImageUrl { get; set; }
    public int foodItemPrice { get; set; }
    public int foodItemQty { get; set; }
    public string transactionId{get;set;}
    public float totalAmount { get; set; }
    public int invoiceId { get; set; }

    public static implicit operator OrderDetails(List<OrderDetails> v)
    {
      throw new NotImplementedException();
    }
  }
}
