using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoreMVC.Data;
using StoreMVC.Models;

namespace StoreMVC.Areas.Admin.Controllers
{
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
        public IActionResult Index()
        {
            // Задача - получить и вернуть в представление все типы продуктов в листе
            //var productsFromDb = _db.ProductTypes.ToList();

            return View(_db.ProductTypes.ToList());
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
