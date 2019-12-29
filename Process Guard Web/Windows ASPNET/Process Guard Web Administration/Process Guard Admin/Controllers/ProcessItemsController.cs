using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Process_Guard_Admin.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Process_Guard_Admin.Controllers
{
    public class ProcessItemsController : Controller
    {

        private readonly processguarddbContext _context;

        public ProcessItemsController(processguarddbContext context)
        {
            _context = context;
        }

        // GET: /ProcessItems/
        public IActionResult Index(string searchString)
        {
            var items = _context.ProcessList.ToList();

            if (!String.IsNullOrWhiteSpace(searchString))
                items.Where(x => x.Exe.Contains(searchString));

            return View(items);
        }

        [HttpPost]
        // POST: /ProcessItems/searchString
        public IActionResult Index(string searchString, bool used)
        {
            var items = _context.ProcessList.AsEnumerable();

            if (!String.IsNullOrWhiteSpace(searchString))
                items = items.Where(x => x.Exe.Contains(searchString));

            return View(items);
        }

        // GET: ProcessItems/Edit/&Exe=""?xfilename=""
        public IActionResult Edit(string Exe, string xfilename)
        {
            if (String.IsNullOrWhiteSpace(Exe))
                return NotFound();

            EditProcess editProcess = new EditProcess();
            editProcess.process = _context.ProcessList.SingleOrDefault<ProcessList>(p => p.Exe == Exe && p.Filename == xfilename);
            editProcess.colors = _context.Colors.ToList();           

            if (editProcess == null)
                return NotFound();

            return View(editProcess);
        }

        // POST: ProcessItems/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string Exe, string Filename, [Bind("Exe,Filename,Description,Color")] ProcessList process)
        {
            if (Exe != process.Exe && Filename != process.Filename)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(process);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if(_context.ProcessList.FirstOrDefault(p => p.Filename == Filename && p.Exe == Exe) != null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(process);
        }

        // GET: ProcessItems/Create
        public IActionResult Create()
        {
            EditProcess newProcess = new EditProcess();
            newProcess.colors = _context.Colors.ToList();

            if (newProcess.colors == null)
                return NotFound();

            return View(newProcess);
        }

        // POST: ProcessItems/Create
        [HttpPost]
        public IActionResult Create([Bind("Exe, Filename, Description, Color")] ProcessList process)
        {
            if (process == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var item = _context.ProcessList.FirstOrDefault(p => p.Exe == process.Exe && p.Filename == process.Filename);
                if (item != null)
                {
                    return BadRequest();
                }
                else
                {
                    _context.ProcessList.Add(process);
                    _context.SaveChanges();
                }
            }
            else
            {
                return BadRequest();
            }

            return RedirectToAction("Index");
        }

        // GET: ProcessItems/Details/&Exe=""?Filename=""
        public IActionResult Details(string Exe, string Filename)
        {
            var process = _context.ProcessList.FirstOrDefault(p => p.Exe == Exe && p.Filename == Filename);

            if (process == null)
                return NotFound();

            return View(process);
        }

        // POST: ProcessItems/Delete/&Exe=""?Filename=""
        public IActionResult Delete(string Exe, string Filename)
        {
            var process = _context.ProcessList.FirstOrDefault(p => p.Exe == Exe && p.Filename == Filename);

            if (process == null)
                return NotFound();

            _context.Remove(process);
            _context.SaveChanges();

            return RedirectToAction("Index");

        }

    }
}
