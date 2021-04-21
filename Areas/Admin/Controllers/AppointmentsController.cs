using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using StoreMVC.Data;
using StoreMVC.Models;
using StoreMVC.Models.ViewModels;
using StoreMVC.Utility;
using System.Security.Claims;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace StoreMVC.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = SD.SuperAdminEndUser + "," + SD.AdminEndUser)] // задаем несколько ролей
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public AppointmentViewModel AppointmentVM { get; set; } = new();

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchKey)
        {
            // 1 Получаем объект юзера, который сделал запрос к методу
            var currentUser = this.User;

            // 2. Получаем объект идентификации юзера
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;

            // 3. Получаем доступ ко всему объекту юзера
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            //_db.Appointments.Include(x => x.SalesPerson); // doesnt work
            if (searchKey is null)
            {
                AppointmentVM.Appointments = await _db.Appointments.Include(x=> x.SalesPerson).ToListAsync();
            }
            else
            {
                AppointmentVM.Appointments = await _db.Appointments.Include(x => x.SalesPerson).Where(x => x.CustomerName.Contains(searchKey) ||
                                                                               x.CustomerPhoneNumber.Contains(searchKey)   ||
                                                                               x.CustomerEmail.Contains(searchKey))
                                                                   .ToListAsync();
            }

            if (!User.IsInRole(SD.SuperAdminEndUser))
                AppointmentVM.Appointments = AppointmentVM.Appointments.Where(x => x.SalesPersonId == claim.Value).ToList();

            return View(AppointmentVM);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return NotFound();

            var products = (from p in _db.Products
                join a in _db.ProductsForAppointments
                    on p.Id equals a.ProductId
                where a.AppointmentId == id
                select p).Include("ProductType") as IEnumerable<Product>;

            AppointmentDetailsViewModel detailsVM = new()
            {
                Appointment = await _db.Appointments.Include(x => x.SalesPerson).FirstOrDefaultAsync(x => x.Id == id),
                SalesPeople = await _db.ApplicationUsers.ToListAsync(),
                Products = products.ToList()
            };
            detailsVM.Appointment.AppointmentTime = detailsVM.Appointment.AppointmentTime
                .AddHours(detailsVM.Appointment.AppointmentDay.Hour)
                .AddMinutes(detailsVM.Appointment.AppointmentDay.Minute);

            return View(detailsVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
                return NotFound();

            var products = (from p in _db.Products
                                            join a in _db.ProductsForAppointments
                                            on p.Id equals a.ProductId
                                            where a.AppointmentId == id
                                            select p).Include("ProductType") as IEnumerable<Product>;

            AppointmentDetailsViewModel detailsVM = new ()
            {
                Appointment = await _db.Appointments.Include(x => x.SalesPerson).FirstOrDefaultAsync(x => x.Id == id),
                SalesPeople = await _db.ApplicationUsers.ToListAsync(),
                Products = products.ToList()
            };
            detailsVM.Appointment.AppointmentTime = detailsVM.Appointment.AppointmentTime
                .AddHours(detailsVM.Appointment.AppointmentDay.Hour)
                .AddMinutes(detailsVM.Appointment.AppointmentDay.Minute);

            return View(detailsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentDetailsViewModel currentAppointment)
        {
            if (!ModelState.IsValid)
                return View(currentAppointment);

            if (!id.Equals(currentAppointment.Appointment.Id))
                return NotFound();


            var appointmentFromDb = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == id);
            currentAppointment.Appointment.CopyTo(appointmentFromDb);
            if (User.IsInRole(SD.SuperAdminEndUser))
                appointmentFromDb.SalesPersonId = currentAppointment.Appointment.SalesPersonId;
            
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
                return NotFound();

            var products = (from p in _db.Products
                join a in _db.ProductsForAppointments
                    on p.Id equals a.ProductId
                where a.AppointmentId == id
                select p).Include("ProductType") as IEnumerable<Product>;

            AppointmentDetailsViewModel detailsVM = new()
            {
                Appointment = await _db.Appointments.Include(x => x.SalesPerson).FirstOrDefaultAsync(x => x.Id == id),
                SalesPeople = await _db.ApplicationUsers.ToListAsync(),
                Products = products.ToList()
            };
            detailsVM.Appointment.AppointmentTime = detailsVM.Appointment.AppointmentTime
                .AddHours(detailsVM.Appointment.AppointmentDay.Hour)
                .AddMinutes(detailsVM.Appointment.AppointmentDay.Minute);

            return View(detailsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, AppointmentDetailsViewModel curAppointment)
        {
            if (!id.Equals(curAppointment.Appointment.Id))
                return NotFound();

            var appointmentFromDb = await _db.Appointments.FindAsync(id);
            if (appointmentFromDb is null)
                return RedirectToAction(nameof(Index));

            _db.Appointments.Remove(appointmentFromDb);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Appointment was successfully removed!";
            return RedirectToAction(nameof(Index));
        }
    }
}
