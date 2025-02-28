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
    public class InsuranceVehicleDetailController : AdminBaseController
    {
        private readonly MyDbContext _context;

        public InsuranceVehicleDetailController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Admin/InsuranceVehicleDetail
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.InsuranceDetails.Include(i => i.Creator).Include(i => i.Deleter).Include(i => i.Updater);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Admin/InsuranceVehicleDetail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceDetails = await _context.InsuranceDetails
                .Include(i => i.Creator)
                .Include(i => i.Deleter)
                .Include(i => i.Updater)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceDetails == null)
            {
                return NotFound();
            }

            return View(insuranceDetails);
        }

        // GET: Admin/InsuranceVehicleDetail/Create
        public IActionResult Create()
        {
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            return View();
        }

        // POST: Admin/InsuranceVehicleDetail/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PlanId,AnnualPaymentAmount,Premium,Deductible,CreatedAt,UpdatedAt,DeleteAt,CreatedBy,UpdatedBy,DeleteBy")] InsuranceDetails insuranceDetails)
        {
            if (ModelState.IsValid)
            {
                _context.Add(insuranceDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceDetails.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceDetails.DeleteBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceDetails.UpdatedBy);
            return View(insuranceDetails);
        }

        // GET: Admin/InsuranceVehicleDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceDetails = await _context.InsuranceDetails.FindAsync(id);
            if (insuranceDetails == null)
            {
                return NotFound();
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceDetails.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceDetails.DeleteBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceDetails.UpdatedBy);
            return View(insuranceDetails);
        }

        // POST: Admin/InsuranceVehicleDetail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlanId,AnnualPaymentAmount,Premium,Deductible,CreatedAt,UpdatedAt,DeleteAt,CreatedBy,UpdatedBy,DeleteBy")] InsuranceDetails insuranceDetails)
        {
            if (id != insuranceDetails.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insuranceDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceDetailsExists(insuranceDetails.Id))
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
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceDetails.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceDetails.DeleteBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceDetails.UpdatedBy);
            return View(insuranceDetails);
        }

        // GET: Admin/InsuranceVehicleDetail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceDetails = await _context.InsuranceDetails
                .Include(i => i.Creator)
                .Include(i => i.Deleter)
                .Include(i => i.Updater)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceDetails == null)
            {
                return NotFound();
            }

            return View(insuranceDetails);
        }

        // POST: Admin/InsuranceVehicleDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insuranceDetails = await _context.InsuranceDetails.FindAsync(id);
            if (insuranceDetails != null)
            {
                _context.InsuranceDetails.Remove(insuranceDetails);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsuranceDetailsExists(int id)
        {
            return _context.InsuranceDetails.Any(e => e.Id == id);
        }
    }
}
