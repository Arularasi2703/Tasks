// Title        : Food Ordering System ( FoodOrderingSystem API )
// Description  : Manages user authentication, account creation, login, profile management, cart operations,
//                and order handling for a food ordering system
// Author       : Arularasi J
// Created at   : 21/07/2023
// Updated at   : 20/12/2023
// Reviewed by  : 
// Reviewed at  : 
using System.Security.Claims;
using System.Collections.Generic;
using FoodOrderingSystemAPI.Interfaces;
using FoodOrderingSystemAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrderCartServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartRepository cartRepository,IConfiguration configuration,ILogger<CartController> logger)
        {
            _cartRepository = cartRepository;
            _configuration = configuration;
            _logger = logger;
        }

        [Authorize(Roles = "User")]
        [HttpGet("{userId}")]
        public ActionResult<IEnumerable<Cart>> GetCartItemsByUserId(int userId)
        {
            var cartItems = _cartRepository.GetCartItemsByUserId(userId);
            return Ok(cartItems);
        }


        // [Authorize(Roles="User")]
        // [HttpPost]
        // public IActionResult AddToCart([FromBody] Cart cartItem)
        // {
        //     if (cartItem == null)
        //     {
        //         string invalidCartItemError = _configuration["FailureMessages:InvalidCartItemError"];
        //         return BadRequest(invalidCartItemError);
        //         // return BadRequest("Invalid cart item data");
        //     }
        //     _cartRepository.AddToCart(cartItem);
        //     return Ok();
        // }

        [Authorize(Roles="User")]
        [HttpPost]
        public IActionResult AddToCart([FromBody] Cart cartItem)
        {
            try
            {
                if (cartItem == null)
                {
                    string invalidCartItemError = _configuration["FailureMessages:InvalidCartItemError"];
                    return BadRequest(invalidCartItemError);
                }

                _cartRepository.AddToCart(cartItem);
                return Ok();
            }
            catch (Exception exception)
            {
                string errorMessage = _configuration["LogMessages:FailureMessages:AddToCartError"];
                _logger.LogError(exception, errorMessage);
                return StatusCode(500, errorMessage);
            }
        }

        [Authorize(Roles="User")]
        [HttpPost("{itemId}/increase")]
        public IActionResult IncreaseQuantity(int itemId)
        {
            try
            {
                _cartRepository.IncreaseQuantity(itemId);
                return Ok();
            }
            catch (Exception exception)
            {
                string errorMessage = _configuration["Messages:FailureMessages:IncreaseCartQuantityError"];
                _logger.LogError(exception, errorMessage);
                return StatusCode(500, errorMessage);
            }
        }

        [Authorize(Roles="User")]
        [HttpPost("{itemId}/decrease")]
        public IActionResult DecreaseQuantity(int itemId)
        {
            try
            {
                _cartRepository.DecreaseQuantity(itemId);
                return Ok();
            }
            catch (Exception exception)
            {
                string errorMessage = _configuration["Messages:FailureMessages:DecreaseQuantityError"];
                _logger.LogError(exception, errorMessage);
                return StatusCode(500, errorMessage);
            }
        }

        [HttpDelete("{itemId}")]
        public IActionResult RemoveCartItem(int itemId)
        {
            try
            {
                _cartRepository.RemoveCartItem(itemId);
                return Ok();
            }
            catch (Exception exception)
            {
                string errorMessage = _configuration["Messages:FailureMessages:RemoveCartItemError"];
                _logger.LogError(exception, errorMessage);
                return StatusCode(500, errorMessage);
            }
        }
    }
}
