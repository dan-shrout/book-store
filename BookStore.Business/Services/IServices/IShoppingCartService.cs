using BookStore.Models;

namespace BookStore.Business.Services.IServices
{
    public interface IShoppingCartService
    {
        Task<ShoppingCart?> GetCartByIdAsync(int cartId);
        Task<IEnumerable<ShoppingCart>> GetUserShoppingCartItemsAsync(string userId);
        Task<int> GetCartCountAsync(string userId);
        Task<ShoppingCart> AddToCartAsync(ShoppingCart cart);
        Task UpdateCartAsync(ShoppingCart cart);
        Task ClearCartAsync(string userId);
    }
}
