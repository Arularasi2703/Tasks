using FoodOrderingSystemAPI.Models;
namespace FoodOrderingSystemAPI.Interfaces{
  public interface IOrderRepository
  {
    Task AddInvoiceAsync(InvoiceModel invoice);
    Task AddOrderAsync(Order order);
    // Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);

    Task AddCheckoutDetailAsync(Checkout checkout);
    Task UpdateTransactionIdAsync(int userId, string transactionId);
    Task<List<InvoiceDetails>> GetOrdersByUserIdAsync(int userId);
    Task<List<OrderDetails>> GetLastOrderDetailsByUserIdAsync(int userId);
  }
}