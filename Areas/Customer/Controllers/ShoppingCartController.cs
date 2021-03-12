using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreMVC.Data;
using StoreMVC.Extensions;
using StoreMVC.Models;
using StoreMVC.Models.ViewModels;
using StoreMVC.Utility;

namespace StoreMVC.Areas.Customer.Controllers
{
    [Area(nameof(Customer))]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;


        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCartVM = new()
            {
                Products = new()
            };

        }

        // Get ShoppingCart/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var productIdList = HttpContext.Session.Get<List<int>>(SessionKey.ShoppingCart);
            ShoppingCartVM.Products.Clear();

            if (productIdList is not null && productIdList.Count != 0)
            {
                foreach (var productId in productIdList)
                {
                    Product product = await _db.Products
                                              .Include(x => x.SpecialTag)
                                              .Include(x => x.ProductType)
                                              .FirstOrDefaultAsync(y => y.Id == productId);

                    ShoppingCartVM.Products.Add(product);
                }

                //productIdList.ForEach(async x =>
                //{
                //    Product product = await _db.Products
                //                               .Include(x => x.ProductType)
                //                               .Include(x => x.SpecialTag)
                //                               .FirstOrDefaultAsync(y => y.Id == x);

                //    ShoppingCartVM.Products.Add(product);
                //});
            }

            return View(ShoppingCartVM);
        }

        //Post ShoppingCart/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> PostIndex()
        {
            var productIdList = HttpContext.Session.Get<List<int>>(SessionKey.ShoppingCart);

            ShoppingCartVM.Appointment.AppointmentDay = ShoppingCartVM.Appointment
                                                                      .AppointmentDay
                                                                      .AddHours(ShoppingCartVM.Appointment.AppointmentTime.Hour)
                                                                      .AddMinutes(ShoppingCartVM.Appointment.AppointmentTime.Minute);

            Appointment appointment = ShoppingCartVM.Appointment;

            await _db.Appointments.AddAsync(appointment);
            await _db.SaveChangesAsync();

            foreach (var product in productIdList)
            {
                ProductsForAppointment productForApointment = new()
                {
                    AppointmentId = appointment.Id,
                    ProductId = product
                };

                await _db.ProductsForAppointments.AddAsync(productForApointment);
            }

            await _db.SaveChangesAsync();

            ShoppingCartVM.Products.Clear();

            return RedirectToAction(nameof(AppointmentConfirmation), new { Id = appointment.Id});
        }



        //Get ShoppingCart/AppointmentConfirmation/Id
        [HttpGet]
        public async Task<IActionResult> AppointmentConfirmation(int id)
        {
            // Получаем информацию о заказе конкретного клиента по ID из базы данных
            ShoppingCartVM.Appointment = await _db.Appointments.FindAsync(id);

            // Получаем и записываем в массив все товары конкретного заказчика
            var productListObj = await _db.ProductsForAppointments.Where(x => x.AppointmentId == id).ToListAsync();

            // Пробегаем циклом по массиву продуктов и добавляем их во ViewModel со всеми связанными таблицами
            foreach(var product in productListObj)
            {
                ShoppingCartVM.Products.Add(await _db.Products
                                                     .Include(x => x.ProductType)
                                                     .Include(x => x.SpecialTag)
                                                     .FirstOrDefaultAsync(x => x.Id == product.Id));
            }
            // Возвращаем представление вместе с VM
            return View(ShoppingCartVM);
        }
    }
}
