using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalToDoList.Data;
using FinalToDoList.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using FinalToDoList.ViewModels;

namespace FinalToDoList.Controllers
{
    public class MyTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _env;

        private readonly string[] permittedExtensions = new string[]
        {
            ".jpg",".jpeg",".png",".bmp",".gif",".ico"
        };

        public MyTasksController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: MyTasks
        public async Task<IActionResult> Index(int? cid, int? pid, int page = 1)
        {
            const int pageSize = 3;
            IQueryable<MyTask> products = _context.MyTasks.Include(p => p.Category).Include(p => p.Status);
            if (cid != null && cid != 0)
                products = products.Where(P => P.CategoryId == cid);
            if (pid != null && pid != 0)
                products = products.Where(P => P.StatusId == pid);
            List<Category> categories = _context.Categories.ToList();
            List<Status> producers = _context.Statuses.ToList();
            categories.Insert(0, new Category() { Id = 0, Name = "Всі категорії" });
            producers.Insert(0, new Status() { Id = 0, Name = "Всі статуси" });
            int count = await products.CountAsync();
            var items = await products.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            FilterViewModel viewModel = new FilterViewModel()
            {
                MyTasks = items,
                Categories = new SelectList(categories, "Id", "Name"),
                Statuses = new SelectList(producers, "Id", "Name"),
                PageViewModel = pageViewModel
            };
            return View(viewModel);
        }

        // GET: MyTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MyTasks == null)
            {
                return NotFound();
            }

            var myTask = await _context.MyTasks
                .Include(m => m.Category)
                .Include(m => m.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myTask == null)
            {
                return NotFound();
            }

            return View(myTask);
        }

        // GET: MyTasks/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name");
            return View();
        }

        // POST: MyTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,DeadLine,Hashtag,FileName,Done,CategoryId,StatusId")] MyTask myTask,
            IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string name = image.FileName;
                    string ext = Path.GetExtension(name);
                    if (permittedExtensions.Contains(ext))
                    {
                        string path = $"/Images/{name}";
                        //string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "UploadedFiles"));
                        string serverPath = _env.WebRootPath + path;

                        using (FileStream fs = new FileStream(serverPath, FileMode.Create, FileAccess.Write))
                        {
                            await image.CopyToAsync(fs);
                        }
                        myTask.FileName = path;
                        _context.MyTasks.Add(myTask);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction(nameof(Error));
                    }
                }
                else
                {
                    return RedirectToAction(nameof(UploadError));
                }
            }
            return View(myTask);
        }

        // GET: MyTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MyTasks == null)
            {
                return NotFound();
            }

            var myTask = await _context.MyTasks.FindAsync(id);
            if (myTask == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", myTask.CategoryId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", myTask.StatusId);
            return View(myTask);
        }

        // POST: MyTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,DeadLine,Hashtag,FileName,Done,CategoryId,StatusId")] MyTask myTask)
        {
            if (id != myTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(myTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MyTaskExists(myTask.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", myTask.CategoryId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", myTask.StatusId);
            return View(myTask);
        }

        // GET: MyTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MyTasks == null)
            {
                return NotFound();
            }

            var myTask = await _context.MyTasks
                .Include(m => m.Category)
                .Include(m => m.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myTask == null)
            {
                return NotFound();
            }

            return View(myTask);
        }

        // POST: MyTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,
            IFormFile image)
        {
            if (_context.MyTasks == null)
            {
                return Problem("Entity set 'ApplicationDbContext.MyTasks'  is null.");
            }
            var myTask = await _context.MyTasks.FindAsync(id);
            if (myTask != null)
            {
                _context.MyTasks.Remove(myTask);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MyTaskExists(int id)
        {
            return (_context.MyTasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult Error()
        {
            ViewData["message"] = "Розширення файлу не відповідає графічному формату";
            return View();
        }
        public IActionResult UploadError()
        {
            ViewData["message"] = "Помилка завантаження файлу";
            return View();
        }
    }
}
