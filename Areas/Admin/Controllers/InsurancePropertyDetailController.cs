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
    public class InsurancePropertyDetailController : AdminBaseController
    {
        private readonly MyDbContext _context;

        public InsurancePropertyDetailController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Admin/InsurancePropertyDetail/Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.InsurancePropertyDetails.ToListAsync());
        }

        // GET: Admin/InsurancePropertyDetail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurancePropertyDetail = await _context.InsurancePropertyDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insurancePropertyDetail == null)
            {
                return NotFound();
            }

            return View(insurancePropertyDetail);
        }

        // GET: Admin/InsurancePropertyDetail/Create
        public IActionResult Create()
        {
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name");
            return View();
        }

        // POST: Admin/InsurancePropertyDetail/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PropertyType,Location,Duration,RiskFactor,Region,PlanId,AnnualPaymentAmount,Premium,Deductible")] InsurancePropertyDetail insurancePropertyDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(insurancePropertyDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name", insurancePropertyDetail.PlanId);
            return View(insurancePropertyDetail);
        }

        // GET: Admin/InsurancePropertyDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurancePropertyDetail = await _context.InsurancePropertyDetails.FindAsync(id);
            if (insurancePropertyDetail == null)
            {
                return NotFound();
            }
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name", insurancePropertyDetail.PlanId);
            return View(insurancePropertyDetail);
        }

        // POST: Admin/InsurancePropertyDetail/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PropertyType,Location,Duration,RiskFactor,Region,PlanId,AnnualPaymentAmount,Premium,Deductible")] InsurancePropertyDetail insurancePropertyDetail)
        {
            if (id != insurancePropertyDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insurancePropertyDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsurancePropertyDetailExists(insurancePropertyDetail.Id))
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
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name", insurancePropertyDetail.PlanId);
            return View(insurancePropertyDetail);
        }

        // GET: Admin/InsurancePropertyDetail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurancePropertyDetail = await _context.InsurancePropertyDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insurancePropertyDetail == null)
            {
                return NotFound();
            }

            return View(insurancePropertyDetail);
        }

        // POST: Admin/InsurancePropertyDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insurancePropertyDetail = await _context.InsurancePropertyDetails.FindAsync(id);
            if (insurancePropertyDetail != null)
            {
                _context.InsurancePropertyDetails.Remove(insurancePropertyDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsurancePropertyDetailExists(int id)
        {
            return _context.InsurancePropertyDetails.Any(e => e.Id == id);
        }
    }
}
