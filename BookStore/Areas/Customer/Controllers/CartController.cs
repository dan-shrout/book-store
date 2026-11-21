using BookStore.Business.Services.IServices;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace BookStoreWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IApplicationUserService _applicationUserService;
        public CartController(IOrderService orderService, IShoppingCartService shoppingCartService, IApplicationUserService applicationUserService)
        {
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _applicationUserService = applicationUserService;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            // if everything is valid, get the shopping cart items for the user and pass them to the view
            var cartItems = await _shoppingCartService.GetUserShoppingCartItemsAsync(userId);
            var user = await _applicationUserService.GetUserByIdAsync(userId);

            // Create a new instance of the ShoppingCartVM class and populate it with the shopping cart items and a new OrderHeader object
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = cartItems,
                OrderHeader = new()
            };
            // Populate the OrderHeader object with the user's information
            shoppingCartVM.OrderHeader.ApplicationUser = user;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;
            shoppingCartVM.OrderHeader.Name = user.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
            shoppingCartVM.OrderHeader.City = user.City;
            shoppingCartVM.OrderHeader.State = user.State;
            shoppingCartVM.OrderHeader.PostalCode = user.PostalCode;

            // Calculate the total order amount by iterating through the shopping cart items and summing up the total price for each item (price * count)
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(shoppingCartVM);
        }

        // This method handles Stripe payments
        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPOST(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItems = await _shoppingCartService.GetUserCartItemsAsync(userId);
            shoppingCartVM.ShoppingCartList = cartItems;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.UtcNow;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }


            shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            shoppingCartVM.OrderHeader.OrderDetails = shoppingCartVM.ShoppingCartList.Select(cart => new OrderDetails
            {
                ProductId = cart.ProductId,
                Price = cart.Price,
                Count = cart.Count,
            }).ToList();
            //create order 

            await _orderService.CreateOrderAsync(shoppingCartVM.OrderHeader);

            try
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                var sessionUrl = await _orderService.CreateStripeCheckoutSessionAsync(shoppingCartVM.OrderHeader, shoppingCartVM.ShoppingCartList, domain);

                await _shoppingCartService.ClearCartAsync(userId);
                HttpContext.Session.SetInt32(SD.SessionCart, 0);
                Response.Headers.Append("Location", sessionUrl);
                return new StatusCodeResult(303);

            }
            catch (StripeException ex)
            {
                TempData["error"] = "Payment processing failed. Please try again.";
                return RedirectToAction(nameof(Index));
            }

        }

        // Action method to increase the quantity of a specific item in the shopping cart
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            return View(id);
        }
        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart != null)
            {
                if (cart.Count == 1000)
                {
                    // Do nothing
                }
                else
                {
                    cart.Count++;
                    await _shoppingCartService.UpdateCartAsync(cart);
                    await UpdateCartSessionAsync();

                }
            }

            return RedirectToAction(nameof(Index));
        }
        // Action method to decrease the quantity of a specific item in the shopping cart
        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart != null)
            {
                cart.Count--;
                await _shoppingCartService.UpdateCartAsync(cart);
                await UpdateCartSessionAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        // Action method to remove a specific item from the shopping cart
        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart != null)
            {
                cart.Count = 0;
                await _shoppingCartService.UpdateCartAsync(cart);
                await UpdateCartSessionAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UpdateCart(int cartId, int count)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                return NotFound();
            }

            if (count <= 1)
            {
                cart.Count = 0;
            }
            else
            {
                if (count >= 1000)
                {
                    cart.Count = 1000;
                }
                else
                {
                    cart.Count = count;
                }
            }
            await _shoppingCartService.UpdateCartAsync(cart);
            await UpdateCartSessionAsync(); // Update the session with the new cart count
            // Return a JSON response indicating success
            return Ok(new { success = true });
        }
        // Helper method
        private async Task UpdateCartSessionAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                var count = await _shoppingCartService.GetCartCountAsync(userId);
                HttpContext.Session.SetInt32(SD.SessionCart, count); // Update the session with the new cart count
            }

        }
    }
}
