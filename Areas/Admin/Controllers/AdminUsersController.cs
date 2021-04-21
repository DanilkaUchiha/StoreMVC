using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreMVC.Data;
using StoreMVC.Models;
using Microsoft.AspNetCore.Authorization;
using StoreMVC.Utility;

namespace StoreMVC.Areas.Customer.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area(nameof(Admin))]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminUsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.ApplicationUsers.ToListAsync());
        }

        //GET AdminUsers/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            //guid может генерить id с пробелами
            var validId = id is null ? string.Empty : id.Trim();
            if (validId.Length is 0)
                return NotFound();

            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == validId);
            if(user is null)
                return NotFound();

            return View(user);
        }

        //POST AdminUsers/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser editedUser)
        {
            if (id != editedUser.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(editedUser);

            var dbUser = await _db.ApplicationUsers.FirstOrDefaultAsync(user => user.Id == editedUser.Id);
            if(dbUser is null)
                return RedirectToAction(nameof(Index));

            editedUser.CopyTo(dbUser);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Admin user {dbUser.Name} was successfully edited!";
            return RedirectToAction(nameof(Index));
        }

        //GET AdminUsers/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var validId = id is null ? string.Empty : id.Trim();
            if (validId.Length is 0)
                return NotFound();

            var user = await _db.ApplicationUsers.FindAsync(validId);
            if (user is null)
                return NotFound();

            return View(user);
        }

        //POST AdminUsers/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> PostDelete(string id)
        {
            var user = await _db.ApplicationUsers.FindAsync(id);
            if (user is null)
                return RedirectToAction(nameof(Index));

            user.LockoutEnd = DateTime.Now.AddYears(1000);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"User {user.Name} was suspended!";
            return RedirectToAction(nameof(Index));
        }

    }
}
