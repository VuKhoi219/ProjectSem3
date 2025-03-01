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
    public class InsuranceHealthDetailController : AdminBaseController
    {
        private readonly MyDbContext _context;

        public InsuranceHealthDetailController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Admin/InsuranceHealthDetail/Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.InsuranceHealthDetails.ToListAsync());
        }

        // GET: Admin/InsuranceHealthDetail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceHealthDetail = await _context.InsuranceHealthDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceHealthDetail == null)
            {
                return NotFound();
            }

            return View(insuranceHealthDetail);
        }

        // GET: Admin/InsuranceHealthDetail/Create
        public IActionResult Create()
        {
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name");
            return View();
        }

        // POST: Admin/InsuranceHealthDetail/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AgeGroup,HospitalNetwork,PreExistingConditions,Duration,RiskFactor,Region,PlanId,AnnualPaymentAmount,Premium,Deductible")] InsuranceHealthDetail insuranceHealthDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(insuranceHealthDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name", insuranceHealthDetail.PlanId);
            return View(insuranceHealthDetail);
        }

        // GET: Admin/InsuranceHealthDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceHealthDetail = await _context.InsuranceHealthDetails.FindAsync(id);
            if (insuranceHealthDetail == null)
            {
                return NotFound();
            }
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name", insuranceHealthDetail.PlanId);
            return View(insuranceHealthDetail);
        }

        // POST: Admin/InsuranceHealthDetail/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AgeGroup,HospitalNetwork,PreExistingConditions,Duration,RiskFactor,Region,PlanId,AnnualPaymentAmount,Premium,Deductible")] InsuranceHealthDetail insuranceHealthDetail)
        {
            if (id != insuranceHealthDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insuranceHealthDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceHealthDetailExists(insuranceHealthDetail.Id))
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
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name", insuranceHealthDetail.PlanId);
            return View(insuranceHealthDetail);
        }

        // GET: Admin/InsuranceHealthDetail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceHealthDetail = await _context.InsuranceHealthDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceHealthDetail == null)
            {
                return NotFound();
            }

            return View(insuranceHealthDetail);
        }

        // POST: Admin/InsuranceHealthDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insuranceHealthDetail = await _context.InsuranceHealthDetails.FindAsync(id);
            if (insuranceHealthDetail != null)
            {
                _context.InsuranceHealthDetails.Remove(insuranceHealthDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsuranceHealthDetailExists(int id)
        {
            return _context.InsuranceHealthDetails.Any(e => e.Id == id);
        }
    }
}
