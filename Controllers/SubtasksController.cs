using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalToDoList.Data;
using FinalToDoList.Models;

namespace FinalToDoList.Controllers
{
    public class SubtasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubtasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Subtasks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Subtasks.Include(s => s.MyTask);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Subtasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Subtasks == null)
            {
                return NotFound();
            }

            var subtask = await _context.Subtasks
                .Include(s => s.MyTask)
                .FirstOrDefaultAsync(m => m.SubtaskId == id);
            if (subtask == null)
            {
                return NotFound();
            }

            return View(subtask);
        }

        // GET: Subtasks/Create
        public IActionResult Create()
        {
            ViewData["MyTaskId"] = new SelectList(_context.MyTasks, "Id", "Description");
            return View();
        }

        // POST: Subtasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubtaskId,Name,Description,DeadLine,Done,MyTaskId")] Subtask subtask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subtask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MyTaskId"] = new SelectList(_context.MyTasks, "Id", "Description", subtask.MyTaskId);
            return View(subtask);
        }

        // GET: Subtasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Subtasks == null)
            {
                return NotFound();
            }

            var subtask = await _context.Subtasks.FindAsync(id);
            if (subtask == null)
            {
                return NotFound();
            }
            ViewData["MyTaskId"] = new SelectList(_context.MyTasks, "Id", "Description", subtask.MyTaskId);
            return View(subtask);
        }

        // POST: Subtasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubtaskId,Name,Description,DeadLine,Done,MyTaskId")] Subtask subtask)
        {
            if (id != subtask.SubtaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subtask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubtaskExists(subtask.SubtaskId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MyTaskId"] = new SelectList(_context.MyTasks, "Id", "Description", subtask.MyTaskId);
            return View(subtask);
        }

        // GET: Subtasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Subtasks == null)
            {
                return NotFound();
            }

            var subtask = await _context.Subtasks
                .Include(s => s.MyTask)
                .FirstOrDefaultAsync(m => m.SubtaskId == id);
            if (subtask == null)
            {
                return NotFound();
            }

            return View(subtask);
        }

        // POST: Subtasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Subtasks == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Subtasks'  is null.");
            }
            var subtask = await _context.Subtasks.FindAsync(id);
            if (subtask != null)
            {
                _context.Subtasks.Remove(subtask);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubtaskExists(int id)
        {
          return (_context.Subtasks?.Any(e => e.SubtaskId == id)).GetValueOrDefault();
        }
    }
}
