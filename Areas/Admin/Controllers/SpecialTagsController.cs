using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreMVC.Data;
using StoreMVC.Models;

namespace StoreMVC.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SpecialTagsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Get SpecialTags/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _db.SpecialTags.ToListAsync());
        }

        // Get SpecialTags/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Post SpecialTags/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTag tag)
        {
            if (!ModelState.IsValid) 
                return View(tag);
            
            await _db.SpecialTags.AddAsync(tag);
            await _db.SaveChangesAsync();
            TempData["SM"] = $"Tag {tag.Name} was added successfully!";

            return RedirectToAction(nameof(Index));
        }

        //Get SpecialTags/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var tag = await _db.SpecialTags.FindAsync(id);
            if (tag == null)
                return NotFound();

            return View(tag);
        }

        //Post SpecialTags/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTag tag)
        {
            if (id != tag.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(tag);

            _db.Update(tag);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Tag {tag.Name} was successfully edited!";

            return RedirectToAction(nameof(Index));
        }

        //Get SpecialTags/Details
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var tag = await _db.SpecialTags.FindAsync(id);
            if (tag == null)
                return NotFound();

            return View(tag);
        }

        //Get SpecialTags/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var tag = await _db.SpecialTags.FindAsync(id);
            if (tag == null)
                return NotFound();

            return View(tag);
        }

        //Post SpecialTags/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTag(int? id)
        {
            SpecialTag tag = await _db.SpecialTags.FindAsync(id);
            if (tag == null)
            {
                TempData["SM"] = $"Failed to delete tag {tag.Name} !";
                return RedirectToAction(nameof(Index));
            }

            _db.SpecialTags.Remove(tag);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Tag {tag.Name} was successfully deleted!";
            
            return RedirectToAction(nameof(Index));
        }
    }
}
