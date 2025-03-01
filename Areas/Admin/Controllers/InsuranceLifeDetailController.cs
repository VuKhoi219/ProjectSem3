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
    public class InsuranceLifeDetailController : AdminBaseController
    {
        private readonly MyDbContext _context;

        public InsuranceLifeDetailController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Admin/InsuranceLifeDetail/Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.InsuranceLifeDetails.ToListAsync());
        }

        // GET: Admin/InsuranceLifeDetail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceLifeDetail = await _context.InsuranceLifeDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceLifeDetail == null)
            {
                return NotFound();
            }

            return View(insuranceLifeDetail);
        }

        // GET: Admin/InsuranceLifeDetail/Create
        public IActionResult Create()
        {
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name");
            return View();
        }

        // POST: Admin/InsuranceLifeDetail/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TermYears,AgeGroup,Beneficiaries,Duration,RiskFactor,Region,PlanId,AnnualPaymentAmount,Premium,Deductible")] InsuranceLifeDetail insuranceLifeDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(insuranceLifeDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name", insuranceLifeDetail.PlanId);
            return View(insuranceLifeDetail);
        }

        // GET: Admin/InsuranceLifeDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceLifeDetail = await _context.InsuranceLifeDetails.FindAsync(id);
            if (insuranceLifeDetail == null)
            {
                return NotFound();
            }
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name", insuranceLifeDetail.PlanId);
            return View(insuranceLifeDetail);
        }

        // POST: Admin/InsuranceLifeDetail/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TermYears,AgeGroup,Beneficiaries,Duration,RiskFactor,Region,PlanId,AnnualPaymentAmount,Premium,Deductible")] InsuranceLifeDetail insuranceLifeDetail)
        {
            if (id != insuranceLifeDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insuranceLifeDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceLifeDetailExists(insuranceLifeDetail.Id))
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
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name", insuranceLifeDetail.PlanId);
            return View(insuranceLifeDetail);
        }

        // GET: Admin/InsuranceLifeDetail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceLifeDetail = await _context.InsuranceLifeDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceLifeDetail == null)
            {
                return NotFound();
            }

            return View(insuranceLifeDetail);
        }

        // POST: Admin/InsuranceLifeDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insuranceLifeDetail = await _context.InsuranceLifeDetails.FindAsync(id);
            if (insuranceLifeDetail != null)
            {
                _context.InsuranceLifeDetails.Remove(insuranceLifeDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsuranceLifeDetailExists(int id)
        {
            return _context.InsuranceLifeDetails.Any(e => e.Id == id);
        }
    }
}
