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
    public class PaymentController : AdminBaseController
    {
        private readonly MyDbContext _context;

        public PaymentController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Payment
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Payments.Include(p => p.Contract).Include(p => p.Creator).Include(p => p.Deleter).Include(p => p.Updater).Include(p => p.User);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Admin/Payment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Contract)
                .Include(p => p.Creator)
                .Include(p => p.Deleter)
                .Include(p => p.Updater)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Admin/Payment/Create
        public IActionResult Create()
        {
            ViewData["ContractId"] = new SelectList(_context.InsuranceContracts, "Id", "Id");
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            return View();
        }

        // POST: Admin/Payment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ContractId,Amount,PaymentDate,Status,ImageUrl,CreatedAt,UpdatedAt,DeleteAt,CreatedBy,UpdatedBy,DeleteBy")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContractId"] = new SelectList(_context.InsuranceContracts, "Id", "Id", payment.ContractId);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", payment.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", payment.DeleteBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", payment.UpdatedBy);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", payment.UserId);
            return View(payment);
        }

        // GET: Admin/Payment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["ContractId"] = new SelectList(_context.InsuranceContracts, "Id", "Id", payment.ContractId);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", payment.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", payment.DeleteBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", payment.UpdatedBy);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", payment.UserId);
            return View(payment);
        }

        // POST: Admin/Payment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ContractId,Amount,PaymentDate,Status,ImageUrl,CreatedAt,UpdatedAt,DeleteAt,CreatedBy,UpdatedBy,DeleteBy")] Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.Id))
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
            ViewData["ContractId"] = new SelectList(_context.InsuranceContracts, "Id", "Id", payment.ContractId);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", payment.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", payment.DeleteBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", payment.UpdatedBy);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", payment.UserId);
            return View(payment);
        }

        // GET: Admin/Payment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Contract)
                .Include(p => p.Creator)
                .Include(p => p.Deleter)
                .Include(p => p.Updater)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Admin/Payment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
