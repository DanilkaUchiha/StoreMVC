using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreMVC.Data;
using StoreMVC.Models;
using StoreMVC.Models.ViewModels;
using StoreMVC.Utility;

namespace StoreMVC.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductViewModel ProductVm { get; set; }

        public ProductsController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;

            ProductVm = new()
            {
                Product = new(),
                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList()
            };
        }

        public async Task<IActionResult> Index()
        {
            // Заполняем модель представления данными, в том числе и из сопутствующих таблиц
            //Почему тут юзаем акти
            var products = _db.Products
                              .Include(x => x.ProductType)
                              .Include(x => x.SpecialTag);
            return View(await products.ToListAsync());
        }

        //Get Products/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(ProductVm);
        }

        //Post Products/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostCreate()
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                ProductVm.ProductTypes = _db.ProductTypes.ToList();
                ProductVm.SpecialTags = _db.SpecialTags.ToList();
                return View(ProductVm);
            }

            // Если модель валидна, добавляем информацию в сущности Entity
            await _db.Products.AddAsync(ProductVm.Product);

            // Сохраняем изменения в базе данных
            await _db.SaveChangesAsync();

            // Логика сохранения картинок
            // Создаём абсолютный путь к корню сайта
            var webRootPath = _hostingEnvironment.WebRootPath;

            // Получаем файлы из формы
            var files = HttpContext.Request.Form.Files;

            // Получаем товар из базы данных
            var productFromDb = await _db.Products.FindAsync(ProductVm.Product.Id);

            // Проверяем, получены ли из формы файлы
            if (files.Count != 0)
            {
                // Комбинируем путь к папке сохранения
                var uploadPath = Path.Combine(webRootPath, SD.ImageFolder);

                // Если получены, то получаем расширение файла
                var extension = Path.GetExtension(files[0].FileName);

                // Сохраняем картинку
                using (var fileStream = new FileStream(Path.Combine(uploadPath, ProductVm.Product.Id + extension), FileMode.Create))
                {
                    await files[0].CopyToAsync(fileStream);
                }

                // Записываем путь к картинке в модель
                productFromDb.Image = $"\\{SD.ImageFolder}\\{ProductVm.Product.Id}{extension}";
            }
            else
            {
                // Если картинки нет, то формируем путь к изображению по умолчанию
                var uploadPath = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProjectImage);

                // Копируем картинку по умолчанию в папку с продуктом
                System.IO.File.Copy(uploadPath, webRootPath + @"\" + SD.ImageFolder + @"\" + ProductVm.Product.Id + ".png");

                // Записываем путь к картинке в модель
                productFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductVm.Product.Id + ".png";
            }

            // Сохраняем изменения в базе
            await _db.SaveChangesAsync();

            // Добавляем сообщение о успехе
            TempData["SM"] = $"Product {ProductVm.Product.Name} was successfully added!";

            // Переадресовываем на страницу Index
            return RedirectToAction(nameof(Index));
        }

        //Get Products/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            ProductVm.Product = await _db.Products.FindAsync(id);
            if (ProductVm.Product == null)
                return NotFound();

            return View(ProductVm);
        }

        //Get Products/Details
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            ProductVm.Product = await _db.Products.FindAsync(id);
            if (ProductVm.Product == null)
                return NotFound();

            return View(ProductVm);
        }

        //Post Product/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (!ModelState.IsValid)
                return View(ProductVm);

            var webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var product = await _db.Products.FindAsync(id);

            if (files != null && files.Count != 0)
            {
                var uploadPath = Path.Combine(webRootPath, SD.ImageFolder);
                var newExtension = Path.GetExtension(files[0].FileName);
                var oldExtension = Path.GetExtension(product.Image);
                var oldFile = Path.Combine(uploadPath, product.Id + oldExtension);
                var newImageName = Path.Combine(uploadPath, product.Id + newExtension);

                if (oldExtension != null && System.IO.File.Exists(oldFile))
                    System.IO.File.Delete(oldFile);

                using (var fileStream = new FileStream(newImageName, FileMode.Create))
                {
                    await files[0].CopyToAsync(fileStream);
                }

                ProductVm.Product.Image = $"\\{SD.ImageFolder}\\{product.Id}{newExtension}";
            }
            //default image
            ProductVm.Product.CopyTo(product);

            await _db.SaveChangesAsync();

            TempData["SM"] = $"Product {product.Name} was successfully edited!";
            
            return RedirectToAction(nameof(Index));
        }

        //Get Product/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
                return NotFound();

            ProductVm.Product = await _db.Products.FindAsync(id);
            if (ProductVm.Product is null)
                return NotFound();

            return View(ProductVm);
        }

        //Post Product/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product is null)
                return NotFound();

            if ( product.Image is not null)
            {
                var webRootPath = _hostingEnvironment.WebRootPath;
                var imagePath = webRootPath + product.Image;

                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Product {product.Name} was successfully deleted!";
            
            return RedirectToAction(nameof(Index));
        }
    }
}
