using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Models;

namespace Project_Sem3.Areas.Admin.Controllers
{
    public class InsurancePlanController : AdminBaseController
    {
        private readonly MyDbContext _context;

        public InsurancePlanController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Admin/InsurancePlan
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.InsurancePlans.Include(i => i.Creator).Include(i => i.Deleter).Include(i => i.Updater);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Admin/InsurancePlan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // if (id == null)
            // {
            //     return NotFound();
            // }
            //
            // var insurancePlan = await _context.InsurancePlans
            //     .Include(i => i.Creator)
            //     .Include(i => i.Deleter)
            //     .Include(i => i.Updater)
            //     .FirstOrDefaultAsync(m => m.Id == id);
            // if (insurancePlan == null)
            // {
            //     return NotFound();
            // }
            //
            return View();
        }

        // GET: Admin/InsurancePlan/Create
        public IActionResult Create()
        {
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            return View();
        }

        // POST: Admin/InsurancePlan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Type,Status,CoverageAmount,CreatedAt,UpdatedAt,DeleteAt,CreatedBy,UpdatedBy,DeleteBy")] InsurancePlan insurancePlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(insurancePlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insurancePlan.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insurancePlan.DeleteBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insurancePlan.UpdatedBy);
            return View(insurancePlan);
        }

        // GET: Admin/InsurancePlan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurancePlan = await _context.InsurancePlans.FindAsync(id);
            if (insurancePlan == null)
            {
                return NotFound();
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insurancePlan.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insurancePlan.DeleteBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insurancePlan.UpdatedBy);
            return View(insurancePlan);
        }

        // POST: Admin/InsurancePlan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Type,Status,CoverageAmount,CreatedAt,UpdatedAt,DeleteAt,CreatedBy,UpdatedBy,DeleteBy")] InsurancePlan insurancePlan)
        {
            if (id != insurancePlan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insurancePlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsurancePlanExists(insurancePlan.Id))
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
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insurancePlan.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insurancePlan.DeleteBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insurancePlan.UpdatedBy);
            return View(insurancePlan);
        }

        // GET: Admin/InsurancePlan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurancePlan = await _context.InsurancePlans
                .Include(i => i.Creator)
                .Include(i => i.Deleter)
                .Include(i => i.Updater)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insurancePlan == null)
            {
                return NotFound();
            }

            return View(insurancePlan);
        }

        // POST: Admin/InsurancePlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insurancePlan = await _context.InsurancePlans.FindAsync(id);
            if (insurancePlan != null)
            {
                _context.InsurancePlans.Remove(insurancePlan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsurancePlanExists(int id)
        {
            return _context.InsurancePlans.Any(e => e.Id == id);
        }
    }
}
