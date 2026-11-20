using BookStore.Business.Services.IServices;
using BookStore.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // ClaimsIdentity is in this namespace

namespace BookStoreWebApp.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly IShoppingCartService _shoppingCartService;
        public CartCountViewComponent(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Remove the cart count from session if the user is not authenticated
                HttpContext.Session.Remove(SD.SessionCart);
                return View(0);
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                // Remove the cart count from session if the claim is not found
                HttpContext.Session.Remove(SD.SessionCart);
                // Return 0 if the user is not authenticated or the claim is not found
                return View(0);
            }

            // Get the count of items in the user's shopping cart
            var sessionCount = HttpContext.Session.GetInt32(SD.SessionCart);
            // If the session count is not set, fetch it from the database and set it in the session
            if (sessionCount.HasValue)
            {
                // Return the count from session if it exists
                return View(sessionCount.Value);
            }

            // Fetch the count from the database and set it in the session
            var cartCount = await _shoppingCartService.GetCartCountAsync(claim.Value);
            // Set the cart count in session
            HttpContext.Session.SetInt32(SD.SessionCart, cartCount);
            // Return the count to the view
            return View(cartCount);
        }
    }
}
