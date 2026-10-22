using BookStore.Business.Services.IServices;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePOST(Product product)
        {
            if (!String.IsNullOrEmpty(product.Name) && !await _productService.IsProductNameUniqueAsync(product.Name))
            {
                ModelState.AddModelError("", "That product name already exists");
            }
            if (ModelState.IsValid)
            {
                await _productService.CreateProductAsync(product);
                TempData["success"] = "Product was created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Update")]
        public async Task<IActionResult> UpdatePOST(Product product)
        {
            if (!String.IsNullOrEmpty(product.Name) &&
                !await _productService.IsProductNameUniqueAsync(product.Name, product.Id))
            {
                ModelState.AddModelError("", "Product \"{product.Name}\" already exists");
            }
            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(product);
                TempData["success"] = $"Product \"{product.Name}\" was updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        // Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int id)
        {
            await _productService.DeleteProductAsync(id);
            TempData["success"] = "Product was deleted successfully";
            return RedirectToAction("Index");

        }
    }
}
