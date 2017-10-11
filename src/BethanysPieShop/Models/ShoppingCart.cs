using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShop.Models
{
    public class ShoppingCart: IShoppingCartRepository
    {
        private readonly AppDbContext _appDbContext;

        public ShoppingCart(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public string ShoppingCartId { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        public static ShoppingCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            var context = services.GetService<AppDbContext>();

            var cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(Pie pie, int amount)
        {

            //check if shoppingcartitem exists in DB for the input Pie
            var shoppingCartItem = _appDbContext.ShoppingCartItems.SingleOrDefault(s => s.Pie.PieId == pie.PieId
                                    && s.ShoppingCartId == ShoppingCartId);
            //create a shoppingcartitem
            if (shoppingCartItem == null)
            {
                shoppingCartItem = new Models.ShoppingCartItem()
                {
                    ShoppingCartId = ShoppingCartId,
                    Pie = pie,
                    Amount = 1
                };

                //add it to the cart if shopping cart item doesnt exist
                _appDbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                //increment pie quantity by 1 if shoppingcartitem exists
                shoppingCartItem.Amount++;
            }

            //save changes to DB
            _appDbContext.SaveChanges();
        }

        public int RemoveFromCart(Pie pie)
        {
            //fetch ahoppingcartitem from db
            var shoppingCartItem = _appDbContext.ShoppingCartItems.SingleOrDefault(s => s.Pie.PieId == pie.PieId
                            && ShoppingCartId == ShoppingCartId);

            var localAmount = 0;

            //if shoppingcartitem found
            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount > 1)
                {
                    shoppingCartItem.Amount--;
                    localAmount = shoppingCartItem.Amount;
                }
                else
                {
                    _appDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }

            _appDbContext.SaveChanges();

            return localAmount;
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ?? 
                (ShoppingCartItems = _appDbContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                .Include(s => s.Pie).ToList());
        }

        public void ClearCart()
        {
            var cartItems = _appDbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == ShoppingCartId);

            _appDbContext.ShoppingCartItems.RemoveRange(cartItems);

            _appDbContext.SaveChanges();
        }

        public decimal GetShoppingTotal()
        {
            //OLD way
            //var cartItems = _appDbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == ShoppingCartId);

            //var total = 0;
            //foreach (ShoppingCartItem cartItem in cartItems)
            //{
            //    total = total + cartItem.Amount;
            //}

            //BETTER WAY!!
            var total = _appDbContext.ShoppingCartItems
                .Where(s => s.ShoppingCartId == ShoppingCartId).Select(c => c.Pie.PieId * c.Amount).Sum();

            return total;
        }
    }
}
