using BookStore.Business.Services.IServices;
using BookStore.Models;

namespace BookStore.Business.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDBContext _context;
        public ShoppingCartService(ApplicationDBContext context)
        {
            _context = context;
        }


        async Task ClearCartAsync(string userId)
        {
            var cartItems = _context.ShoppingCarts.Include(c => c.Product).Where(c => c.ApplicationUserId == userId).ToListAsync();

            if (cartItems.Any())
            {
                _context.ShoppingCarts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ShoppingCart> GetCartByIdAsync(int cartId)
        {
            return await _context.ShoppingCarts.Include(c => c.Product).FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<int> GetCartCountAsync(string userId)
        {
            return await _context.ShoppingCarts.Where(c => c.ApplicationUserId == userId).SumAsync(c => c.Count);
        }

        public async Task<IEnumerable<ShoppingCart>> GetShoppingCartItemsAsync(string userId)
        {
            return await _context.ShoppingCarts.Include(c => c.Product).Where(u => u.ApplicationUserId == userId).ToListAsync();
        }

        public async Task<ShoppingCart> AddToCartAsync(ShoppingCart cart)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateCartAsync(ShoppingCart cart)
        {
            throw new NotImplementedException();
        }
    }
}
