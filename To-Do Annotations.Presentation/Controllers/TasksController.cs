using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using To_Do_Annonations.Domain.Entities;
using AppContext = To_Do_Annotations.Persistence.Database.AppContext;

namespace To_Do_Annotations.Presentation.Controllers
{
    public class TasksController : Controller
    {
        private readonly AppContext _context;

        public TasksController(AppContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Access");
            }

            var myTasks = await _context.Tasks
                                        .Where(t => t.UserId == userId)
                                        .ToListAsync();

            return View(myTasks);
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Access");

            if (id == null) return NotFound();

            var toDoTask = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == id);
            if (toDoTask == null) return NotFound();

            return View(toDoTask);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Access");
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Status,CreatedAt")] ToDoTask toDoTask)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Access");

            if (ModelState.IsValid)
            {
                toDoTask.UserId = userId.Value;

                _context.Add(toDoTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(toDoTask);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Access");

            if (id == null) return NotFound();

            var toDoTask = await _context.Tasks.FindAsync(id);
            if (toDoTask == null) return NotFound();

            return View(toDoTask);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,CreatedAt,UserId")] ToDoTask toDoTask)
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Access");

            if (id != toDoTask.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toDoTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoTaskExists(toDoTask.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(toDoTask);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Access");

            if (id == null) return NotFound();

            var toDoTask = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == id);
            if (toDoTask == null) return NotFound();

            return View(toDoTask);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Access");

            var toDoTask = await _context.Tasks.FindAsync(id);
            if (toDoTask != null)
            {
                _context.Tasks.Remove(toDoTask);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoTaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}