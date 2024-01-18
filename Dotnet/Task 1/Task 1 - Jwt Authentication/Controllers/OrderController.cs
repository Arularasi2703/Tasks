// Title        : Food Ordering System ( FoodOrderingSystem API )
// Description  : Manages user authentication, account creation, login, profile management, cart operations,
//                and order handling for a food ordering system
// Author       : Arularasi J
// Created at   : 21/07/2023
// Updated at   : 20/12/2023
// Reviewed by  : 
// Reviewed at  : 
using FoodOrderingSystemAPI.Interfaces;
using FoodOrderingSystemAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FoodOrderingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderRepository orderRepository,IConfiguration configuration,ILogger<OrderController> logger,ICartRepository cartRepository)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            _logger = logger;
            _cartRepository = cartRepository;
        }

        [Authorize(Roles = "User")]
        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest checkoutrequest)
        {
            try
            {
                var checkout = checkoutrequest.checkout;
                var cart = checkoutrequest.cart;
                int userIdFromCart = cart.FirstOrDefault(item => item.userId != 0)?.userId ?? 0;
                var foodItemIds = cart.Select(item => item.foodItemId).Distinct().ToList();
                

                var totalAmount = cart.Sum(item => item.totalAmount);
                var invoice = new InvoiceModel
                {
                    dateOfInvoice = DateTime.UtcNow,
                    Total_Bill = totalAmount,
                    userId = userIdFromCart
                };
                await _orderRepository.AddInvoiceAsync(invoice);
                string orderStatus = _configuration["OrderSettings:OrderStatus"];

                foreach (var item in cart)
                {
                    var orderdetails = new Order
                    {
                        quantity = item.quantity,
                        unitPrice = (int)item.price,
                        orderBill = item.totalAmount,
                        orderDate = DateTime.Now,
                        foodItemId = item.foodItemId, 
                        invoiceId = invoice.id,
                        orderStatus = orderStatus
                    };
                    await _orderRepository.AddOrderAsync(orderdetails);
                }

                var checkoutDetails = new Checkout
                {
                    name = checkout.name,
                    email = checkout.email,
                    mobileNumber = checkout.mobileNumber,
                    address = checkout.address,
                    pincode = checkout.pincode,
                    userId = userIdFromCart
                };
                await _orderRepository.AddCheckoutDetailAsync(checkoutDetails);

                var userId = checkoutrequest.cart.FirstOrDefault()?.userId ?? 0;
                _cartRepository.MarkCartItemsAsCheckedOut(userId);
                Random _random = new Random();
                int minValue = _configuration.GetValue<int>("AppSettings:TransactionId:MinValue");
                int maxValue = _configuration.GetValue<int>("AppSettings:TransactionId:MaxValue");
                int TransactionId = _random.Next(minValue, maxValue);

                return Ok(TransactionId);
            }
            catch (Exception exception)
            {
                string errorMessage = _configuration["Messages:FailureMessages:CheckoutError"];
                _logger.LogError(exception, errorMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
            }
        }


        [HttpPost("payment")]
        public async Task<IActionResult> ProcessPaymentAsync([FromBody] PaymentResponse paymentresponse)
        {
            try
            {
                // Retrieve the payment details from the paymentResponse object and process the payment
                var paymentId = paymentresponse.razorpayPaymentId;

                // Process the payment and handle success/failure scenarios

                // Assuming you have the user ID available, update the transaction ID and order ID
                int userId = paymentresponse.userId;
                await _orderRepository.UpdateTransactionIdAsync(userId, paymentId);

                var orderDetails = await _orderRepository.GetLastOrderDetailsByUserIdAsync(userId);

                // Return the order summary along with the payment status message
                string paymentMessage =  _configuration["Messages:SuccessMessages:PaymentSuccessful"];

                var responseModel = new PaymentResponseModel
                {
                    message = paymentMessage,
                    orderDetails = orderDetails
                };

                return Ok(responseModel);
            }
            catch (Exception exception)
            {
                // Handle any exceptions that occur during payment processing
                string errorMessage = _configuration["Messages:FailureMessages:PaymentError"];
                _logger.LogError(exception, errorMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
            }
        }

        // [Authorize(Roles = "User")]
        [HttpGet("OrderHistory")]
        public async Task<IActionResult> GetOrderHistory(int userId)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
                if (orders.Count > 0)
                {
                    return Ok(orders);
                }
                else
                {
                    string noOrdersMessage = _configuration["Messages:FailureMessages:NoOrdersPlaced"];
                    _logger.LogInformation(noOrdersMessage);
                    return Ok(noOrdersMessage);
                    // return Ok("You haven't placed any orders yet.");
                }
            }
            catch (Exception exception)
            {
                string errorMessage = _configuration["Messages:FailureMessages:RetrieveOrderHistoryError"];
                _logger.LogError(exception, errorMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
            }
        }

        // [HttpPost]  
        //  public async Task<IActionResult> AddFoodItemAsync([FromForm] FoodItemRequest foodItemRequest)
        // {
        //     try
        //     {
        //         if (!ModelState.IsValid)
        //         {
        //             return BadRequest(ModelState);
        //         }
        //         byte[] imageBytes = null;

        //         using (var memorystream = new MemoryStream())
        //         {
        //             foodItemRequest.image.OpenReadStream().CopyTo(memorystream);
        //             imageBytes = memorystream.ToArray();
        //         }

        //         var foodItem = new FoodItem
        //         {
        //             name = foodItemRequest.name,
        //             price = foodItemRequest.price,
        //             description = foodItemRequest.description,
        //             imageUrl = imageBytes,
        //             category = foodItemRequest.category,
        //             isVegan = foodItemRequest.isVegan,
        //             calories = foodItemRequest.calories
        //         };

        //         await _foodMenuService.AddFoodItemAsync(foodItem);
        //         return Ok(foodItem);
        //     }
        //     catch (Exception exception)
        //     {
        //         // _logger.LogError(ex, "Error occurred while adding a new food item.");
        //         // return StatusCode(500, "An error occurred while adding a new food item.");
        //         _logger.LogError(exception, _configuration["Messages:Error:AddFoodItem"]);
        //         return StatusCode(500, _configuration["Messages:Error:AddFoodItem"]);
        //     }
        // }

    }
}
