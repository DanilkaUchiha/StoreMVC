using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreMVC.Data;
using StoreMVC.Models;
using StoreMVC.Utility;

namespace StoreMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area(nameof(Admin))]
    public class ProductTypesController : Controller
    {
        // 2. Создаём приватную переменную для трансфера данных из/в базу данных
        private readonly ApplicationDbContext _db;

        // 1. Создаём конструктор класса по умолчанию
        public ProductTypesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: ProductTypes/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Задача - получить и вернуть в представление все типы продуктов в листе
            //var productsFromDb = _db.ProductTypes.ToList();

            return View(await _db.ProductTypes.ToListAsync());
        }


        // GET : ProductTypes/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            ProductType product = await _db.ProductTypes.FindAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: ProductTypes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductType product)
        {
            if (id != product.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(product);

            _db.Update(product);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Product {product.Name} was successfully edited!";

            return RedirectToAction(nameof(Index));
        }

        // GET : ProductTypes/Details
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            ProductType product = await _db.ProductTypes.FindAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET : ProductTypes/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            ProductType product = await _db.ProductTypes.FindAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: ProductTypes/Create
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            ProductType product = await _db.ProductTypes.FindAsync(id);

            if (product == null)
            {
                TempData["SM"] = $"Failed to delete {product.Id} product!";
                return RedirectToAction(nameof(Index));
            }

            _db.ProductTypes.Remove(product);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Succeed to delete {product.Id} product!";
            return RedirectToAction(nameof(Index));
        }

        // GET: ProductTypes/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();

        }

        // POST: ProductTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductType productType)
        {
            // 1. Проверить модель на валидность
            if (ModelState.IsValid)
            {
                // 1.1 Если модель валидная, то добавляем значения в сущности Entity
                _db.Add(productType);

                // 1.2 Сохраняем изменения в базе данных асинхронно
                await _db.SaveChangesAsync();

                // 1.3 Добавляем сообщение о успешном сохранении значений в базе
                TempData["SM"] = $"Product type: {productType.Name} added successful!";

                // 1.4 Переадресовываем на страницу Index
                return RedirectToAction(nameof(Index));
            }

            // 2. Если модель не валидная, вернуть текущее представление вместе с моделью для исправления ошибок
            return View(productType);
        }
    }
}
