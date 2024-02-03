using AccountAPI.Data;
using FoodOrderingSystemAPI.Interfaces;
using FoodOrderingSystemAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace FoodOrderingSystemAPI.Repositories{
  public class OrderRepository:IOrderRepository{
    private readonly FoodAppDbContext _foodAppDbContext;
     private readonly IConfiguration _configuration;
    public OrderRepository(FoodAppDbContext foodAppDbContext,IConfiguration configuration)
      {
        _foodAppDbContext = foodAppDbContext;
        _configuration = configuration;
      }
    public async Task AddInvoiceAsync(InvoiceModel invoiceModel)
    {
      await _foodAppDbContext.invoiceModel.AddAsync(invoiceModel);
      await _foodAppDbContext.SaveChangesAsync();
    }
    public async Task AddCheckoutDetailAsync(Checkout checkout)
    {
      await _foodAppDbContext.CheckoutDetails.AddAsync(checkout);
      await _foodAppDbContext.SaveChangesAsync();
    }

    public async Task AddOrderAsync(Order order)
    {
      await _foodAppDbContext.orders.AddAsync(order);
      await _foodAppDbContext.SaveChangesAsync();
    }

    // public async Task UpdateTransactionIdAndOrderIdAsync(int userId, string transactionId, string orderId)
    // {
    //     var invoice = await _db.invoiceModel.FirstOrDefaultAsync(i => i.FKUserID == userId);
    //     if (invoice != null)
    //     {
    //         invoice.TransactionId = transactionId;
    //         invoice.OrderId = orderId;
    //         _db.Entry(invoice).State = EntityState.Modified;
    //         await _db.SaveChangesAsync();
    //     }
    // }
    public async Task UpdateTransactionIdAsync(int userId, string transactionId)
    {
        var lastUserOrder = await _foodAppDbContext.orders
        .Join(
            _foodAppDbContext.invoiceModel,
            order => order.invoiceId,
            invoice => invoice.id,
            (order, invoice) => new { Order = order, Invoice = invoice }
        )
        .Where(joinResult => joinResult.Invoice.userId == userId)
        .OrderByDescending(joinResult => joinResult.Order.id)
        .Select(joinResult => new { joinResult.Order, joinResult.Invoice })
        .FirstOrDefaultAsync();

        if (lastUserOrder != null)
        {
            lastUserOrder.Invoice.transactionId = transactionId;
            _foodAppDbContext.Entry(lastUserOrder.Invoice).State = EntityState.Modified;
            await _foodAppDbContext.SaveChangesAsync();
        }
    }


    public async Task<List<InvoiceDetails>> GetOrdersByUserIdAsync(int userId)
    {
        var query = from order in _foodAppDbContext.orders
            join invoice in _foodAppDbContext.invoiceModel on order.invoiceId equals invoice.id
            join foodItem in _foodAppDbContext.FoodItems on order.foodItemId equals foodItem.id
            where invoice.userId == userId
            select new
            {
                orderId = order.id,
                orderDate = invoice.dateOfInvoice,
                orderStatus = order.orderStatus,
                foodItemName = foodItem.name,
                foodItemDescription = foodItem.description,
                foodItemImageUrl = foodItem.imageUrl,
                foodItemPrice = order.unitPrice,
                foodItemQty = order.quantity,
                totalAmount = order.orderBill,
                invoiceId = invoice.id,
                transactionId = invoice.transactionId
            };

        var orderDetails = await query.ToListAsync();

        var invoiceDetails = orderDetails
            .GroupBy(orderDetail => orderDetail.invoiceId)
            .Select(group => new InvoiceDetails
            {
                invoiceId = group.Key,
                orderDate = group.FirstOrDefault()?.orderDate.HasValue == true ? group.FirstOrDefault().orderDate.Value.Date : default,
                orderStatus = group.FirstOrDefault()?.orderStatus,
                orderDetails = group.Select(orderDetail => new OrderDetails
                {
                    orderId = orderDetail.orderId,
                    foodItemName = orderDetail.foodItemName,
                    foodItemDescription = orderDetail.foodItemDescription,
                    foodItemImageUrl = orderDetail.foodItemImageUrl,
                    foodItemPrice = orderDetail.foodItemPrice,
                    foodItemQty = orderDetail.foodItemQty,
                    totalAmount = orderDetail.totalAmount,
                    invoiceId = orderDetail.invoiceId,
                    transactionId = orderDetail.transactionId
                }).ToList()
            }).ToList();

        return invoiceDetails;
    }

    public async Task<List<OrderDetails>> GetLastOrderDetailsByUserIdAsync(int userId)
    {
        var lastInvoiceId = await _foodAppDbContext.invoiceModel
            .Where(invoice => invoice.userId == userId)
            .OrderByDescending(invoice => invoice.dateOfInvoice)
            .Select(invoice => invoice.id)
            .FirstOrDefaultAsync();

        var result = await (from order in _foodAppDbContext.orders
            join invoice in _foodAppDbContext.invoiceModel on order.invoiceId equals invoice.id
            join foodItem in _foodAppDbContext.FoodItems on order.foodItemId equals foodItem.id
            where order.invoiceId == lastInvoiceId
            select new OrderDetails
            {
                orderId = order.id,
                orderDate = invoice.dateOfInvoice.HasValue ? invoice.dateOfInvoice.Value.Date : default,
                orderStatus = order.orderStatus,
                foodItemName = foodItem.name,
                foodItemDescription = foodItem.description,
                foodItemImageUrl = foodItem.imageUrl,
                foodItemPrice = order.unitPrice,
                foodItemQty = order.quantity,
                totalAmount = order.orderBill,
                invoiceId = invoice.id,
                transactionId = invoice.transactionId 
            }).ToListAsync();

        return result;
    }
  }
}