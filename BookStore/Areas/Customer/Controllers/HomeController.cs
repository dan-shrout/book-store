using BookStore.Business.Services.IServices;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStoreWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        public HomeController(IProductService productService, IShoppingCartService shoppingCartService)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync(includeCategory: true);
            return View(products);
        }

        public async Task<IActionResult> Details(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId, includeCategory: true);
            if (product == null)
            {
                return NotFound();
            }
            ShoppingCart cart = new ShoppingCart()
            {
                ProductId = productId,
                Product = product,
                Count = 1
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            // This method is called when the user submits the form to add a product to the shopping cart.
            // It retrieves the user's ID from the claims and associates it with the shopping cart item before adding it to the cart.
            var claimsIdentity = (ClaimsIdentity)User.Identity; // Cast the User.Identity to ClaimsIdentity to access the claims
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Retrieve the user's ID from the claims

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            shoppingCart.ApplicationUserId = userId;
            await _shoppingCartService.AddToCartAsync(shoppingCart);

            // This adds the count of items in the shopping cart to the session, so it can be displayed in the UI (e.g., in the navigation bar).
            var count = await _shoppingCartService.GetCartCountAsync(userId);
            HttpContext.Session.SetInt32(SD.SessionCart, count); // Update the session with the new cart count

            TempData["success"] = "Item added to cart successfully!";

            return RedirectToAction("Index");
        }

    }
}
