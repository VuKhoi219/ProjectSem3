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
    public class InsuranceContractController : AdminBaseController
    {
        private readonly MyDbContext _context;

        public InsuranceContractController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Admin/InsuranceContract
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.InsuranceContracts.Include(i => i.Creator).Include(i => i.Deleter).Include(i => i.Plan).Include(i => i.Updater).Include(i => i.User);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Admin/InsuranceContract/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceContract = await _context.InsuranceContracts
                .Include(i => i.Creator)
                .Include(i => i.Deleter)
                .Include(i => i.Plan)
                .Include(i => i.Updater)
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceContract == null)
            {
                return NotFound();
            }

            return View(insuranceContract);
        }

        // GET: Admin/InsuranceContract/Create
        public IActionResult Create()
        {
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Id");
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            return View();
        }

        // POST: Admin/InsuranceContract/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,PlanId,DetailId,StartDate,EndDate,Status,CreatedAt,UpdatedAt,DeleteAt,CreatedBy,UpdatedBy,DeleteBy")] InsuranceContract insuranceContract)
        {
            if (ModelState.IsValid)
            {
                _context.Add(insuranceContract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceContract.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceContract.DeleteBy);
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Id", insuranceContract.PlanId);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceContract.UpdatedBy);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceContract.UserId);
            return View(insuranceContract);
        }

        // GET: Admin/InsuranceContract/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceContract = await _context.InsuranceContracts.FindAsync(id);
            if (insuranceContract == null)
            {
                return NotFound();
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceContract.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceContract.DeleteBy);
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Id", insuranceContract.PlanId);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceContract.UpdatedBy);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceContract.UserId);
            return View(insuranceContract);
        }

        // POST: Admin/InsuranceContract/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,PlanId,DetailId,StartDate,EndDate,Status,CreatedAt,UpdatedAt,DeleteAt,CreatedBy,UpdatedBy,DeleteBy")] InsuranceContract insuranceContract)
        {
            if (id != insuranceContract.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insuranceContract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceContractExists(insuranceContract.Id))
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
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceContract.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceContract.DeleteBy);
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Id", insuranceContract.PlanId);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceContract.UpdatedBy);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", insuranceContract.UserId);
            return View(insuranceContract);
        }

        // GET: Admin/InsuranceContract/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceContract = await _context.InsuranceContracts
                .Include(i => i.Creator)
                .Include(i => i.Deleter)
                .Include(i => i.Plan)
                .Include(i => i.Updater)
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceContract == null)
            {
                return NotFound();
            }

            return View(insuranceContract);
        }

        // POST: Admin/InsuranceContract/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insuranceContract = await _context.InsuranceContracts.FindAsync(id);
            if (insuranceContract != null)
            {
                _context.InsuranceContracts.Remove(insuranceContract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsuranceContractExists(int id)
        {
            return _context.InsuranceContracts.Any(e => e.Id == id);
        }
    }
}
