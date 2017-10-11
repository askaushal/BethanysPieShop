using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShop.Models
{
    public interface IShoppingCartRepository
    {
        string ShoppingCartId { get; set; }
        List<ShoppingCartItem> ShoppingCartItems { get; set; }
        void AddToCart(Pie pie, int amount);
        int RemoveFromCart(Pie pie);
        List<ShoppingCartItem> GetShoppingCartItems();
        void ClearCart();
        decimal GetShoppingTotal();
    }
}
