using System.Collections.Generic;
using FoodOrderingSystemAPI.Models;

namespace FoodOrderingSystemAPI.Interfaces
{
    public interface ICartRepository
    {
        IEnumerable<Cart> GetCartItems();
        IEnumerable<Cart> GetCartItemsByUserId(int userId);
        void AddToCart(Cart cartItem);
        void IncreaseQuantity(int productId);
        void DecreaseQuantity(int productId);
        void RemoveCartItem(int itemId);
        void MarkCartItemsAsCheckedOut(int userId);
    }
}
