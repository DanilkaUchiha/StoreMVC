using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreMVC.Data;
using StoreMVC.Extensions;
using StoreMVC.Utility;

namespace StoreMVC.Areas.Customer.Controllers
{
    [Area(nameof(Customer))]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Products.ToListAsync());
        }

        //Get Home/Details
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return NotFound();

            var product = await _db.Products.Include(x => x.ProductType).Include(x => x.SpecialTag).FirstOrDefaultAsync(x => x.Id == id);
            if (product is null)
                return NotFound();

            return View(product);
        }

        //Post Home/Details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id)
        {
            // Создаём лист типа int и записываем в него данные из сессии, Проверяем, если массив null, то создаём новый экземпляр лист
            List<int> listOfShoppingCart = HttpContext.Session.Get<List<int>>(SessionKey.ShoppingCart) ?? new();
            listOfShoppingCart.Add(id);
            HttpContext.Session.Set(SessionKey.ShoppingCart, listOfShoppingCart);
            TempData["SM"] = "Product was successfully added to cart!";

            return RedirectToAction(nameof(Index));
        }

        //Post Home/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var listOfShoppingCart = HttpContext.Session.Get<List<int>>(SessionKey.ShoppingCart);
            if (listOfShoppingCart.Count > 0 && listOfShoppingCart.Contains(id))
            {
                listOfShoppingCart.Remove(id);
            }

            HttpContext.Session.Set(SessionKey.ShoppingCart, listOfShoppingCart);
            TempData["SM"] = "Product was successfully deleted from cart!";
            
            return RedirectToAction(nameof(Index));
        }
    }
}
