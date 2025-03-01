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

        // GET: Admin/InsuranceVehicleDetail/Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.InsuranceVehicleDetails.ToListAsync());
        }

        // GET: Admin/InsuranceVehicleDetail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceVehicleDetail = await _context.InsuranceVehicleDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceVehicleDetail == null)
            {
                return NotFound();
            }

            return View(insuranceVehicleDetail);
        }

        // GET: Admin/InsuranceVehicleDetail/Create
        public IActionResult Create()
        {
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name");
            return View();
        }

        // // POST: Admin/InsuranceVehicleDetail/Create
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("Id,VehicleType,VehicleModel,VehicleYear,Duration,RiskFactor,Region,PlanId,AnnualPaymentAmount,Premium,Deductible")] InsuranceVehicleDetail insuranceVehicleDetail)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         _context.Add(insuranceVehicleDetail);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name", insuranceVehicleDetail.PlanId);
        //     return View(insuranceVehicleDetail);
        // }

        // GET: Admin/InsuranceVehicleDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceVehicleDetail = await _context.InsuranceVehicleDetails.FindAsync(id);
            if (insuranceVehicleDetail == null)
            {
                return NotFound();
            }
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name", insuranceVehicleDetail.PlanId);
            return View(insuranceVehicleDetail);
        }

        // POST: Admin/InsuranceVehicleDetail/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VehicleType,VehicleModel,VehicleYear,Duration,RiskFactor,Region,PlanId,AnnualPaymentAmount,Premium,Deductible")] InsuranceVehicleDetail insuranceVehicleDetail)
        {
            if (id != insuranceVehicleDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insuranceVehicleDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceVehicleDetailExists(insuranceVehicleDetail.Id))
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
            ViewData["PlanId"] = new SelectList(_context.InsurancePlans, "Id", "Name", insuranceVehicleDetail.PlanId);
            return View(insuranceVehicleDetail);
        }

        // GET: Admin/InsuranceVehicleDetail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceVehicleDetail = await _context.InsuranceVehicleDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceVehicleDetail == null)
            {
                return NotFound();
            }

            return View(insuranceVehicleDetail);
        }

        // POST: Admin/InsuranceVehicleDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insuranceVehicleDetail = await _context.InsuranceVehicleDetails.FindAsync(id);
            if (insuranceVehicleDetail != null)
            {
                _context.InsuranceVehicleDetails.Remove(insuranceVehicleDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsuranceVehicleDetailExists(int id)
        {
            return _context.InsuranceVehicleDetails.Any(e => e.Id == id);
        }
    }
}
