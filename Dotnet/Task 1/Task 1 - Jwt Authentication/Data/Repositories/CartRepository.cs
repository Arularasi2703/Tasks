using System.Collections.Generic;
using System.Linq;
using AccountAPI.Data;
using FoodOrderingSystemAPI.Interfaces;
using FoodOrderingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace OrderCartServiceAPI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly FoodAppDbContext _foodAppDbContext;

        public CartRepository(FoodAppDbContext foodAppDbContext)
        {
            _foodAppDbContext = foodAppDbContext;
        }

        public IEnumerable<Cart> GetCartItems()
        {
            return _foodAppDbContext.Carts.ToList();
        }
        public IEnumerable<Cart> GetCartItemsByUserId(int userId)
        {
            return _foodAppDbContext.Carts
            .Where(c => c.userId == userId && !c.IsCheckedOut)
            .ToList();
        }


        public void AddToCart(Cart cartItem)
        {
            // Check if the cart item already exists in the database
            var existingCartItem = _foodAppDbContext.Carts.FirstOrDefault(cartItems => cartItems.id == cartItem.id);
            if (existingCartItem != null)
            {
                // Increase the quantity and update the total amount
                existingCartItem.quantity += cartItem.quantity;
                existingCartItem.totalAmount += cartItem.totalAmount;
            }
            else
            {
                // Add the new cart item to the database
                _foodAppDbContext.Carts.Add(cartItem);
            }

            _foodAppDbContext.SaveChanges();
        }

        public void IncreaseQuantity(int itemId)
        {
            var cartItem = _foodAppDbContext.Carts.FirstOrDefault(cartItem => cartItem.id == itemId);
            if (cartItem != null)
            {
                cartItem.quantity++;
                cartItem.totalAmount = cartItem.price * cartItem.quantity;
                _foodAppDbContext.SaveChanges();
            }
        }

        public void DecreaseQuantity(int itemId)
        {
            var cartItem = _foodAppDbContext.Carts.FirstOrDefault(cartItem => cartItem.id == itemId);
            if (cartItem != null)
            {
                if (cartItem.quantity > 1)
                {
                    cartItem.quantity--;
                    cartItem.totalAmount = cartItem.price * cartItem.quantity;
                }
                else
                {
                    _foodAppDbContext.Carts.Remove(cartItem);
                }

                _foodAppDbContext.SaveChanges();
            }
        }

        public void RemoveCartItem(int itemId)
        {
            var cartItem = _foodAppDbContext.Carts.FirstOrDefault(cartItem => cartItem.id == itemId);
            if (cartItem != null)
            {
                _foodAppDbContext.Carts.Remove(cartItem);
                _foodAppDbContext.SaveChanges();
            }
        }

        public void MarkCartItemsAsCheckedOut(int userId)
        {
            var cartItems = _foodAppDbContext.Carts.Where(c => c.userId == userId && !c.IsCheckedOut);
            foreach (var cartItem in cartItems)
            {
                cartItem.IsCheckedOut = true;
                _foodAppDbContext.Entry(cartItem).Property(c => c.IsCheckedOut).IsModified = true;
            }

            _foodAppDbContext.SaveChanges();
        }
    }
}
