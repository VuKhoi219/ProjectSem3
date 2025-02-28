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
    public class LoanPaymentController : AdminBaseController
    {
        private readonly MyDbContext _context;

        public LoanPaymentController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LoanPayment
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.LoanPayments.Include(l => l.BorrowCapital).Include(l => l.Creator);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Admin/LoanPayment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanPayment = await _context.LoanPayments
                .Include(l => l.BorrowCapital)
                .Include(l => l.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanPayment == null)
            {
                return NotFound();
            }

            return View(loanPayment);
        }

        // GET: Admin/LoanPayment/Create
        public IActionResult Create()
        {
            ViewData["BorrowId"] = new SelectList(_context.BorrowCapitals, "Id", "LoanPurpose");
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            return View();
        }

        // POST: Admin/LoanPayment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BorrowId,PaymentAmount,PaymentDate,PaymentImage,OverdueDays,PenaltyInterest,Status,CreatedAt,CreatedBy")] LoanPayment loanPayment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loanPayment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BorrowId"] = new SelectList(_context.BorrowCapitals, "Id", "LoanPurpose", loanPayment.BorrowId);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", loanPayment.CreatedBy);
            return View(loanPayment);
        }

        // GET: Admin/LoanPayment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanPayment = await _context.LoanPayments.FindAsync(id);
            if (loanPayment == null)
            {
                return NotFound();
            }
            ViewData["BorrowId"] = new SelectList(_context.BorrowCapitals, "Id", "LoanPurpose", loanPayment.BorrowId);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", loanPayment.CreatedBy);
            return View(loanPayment);
        }

        // POST: Admin/LoanPayment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BorrowId,PaymentAmount,PaymentDate,PaymentImage,OverdueDays,PenaltyInterest,Status,CreatedAt,CreatedBy")] LoanPayment loanPayment)
        {
            if (id != loanPayment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loanPayment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanPaymentExists(loanPayment.Id))
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
            ViewData["BorrowId"] = new SelectList(_context.BorrowCapitals, "Id", "LoanPurpose", loanPayment.BorrowId);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", loanPayment.CreatedBy);
            return View(loanPayment);
        }

        // GET: Admin/LoanPayment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanPayment = await _context.LoanPayments
                .Include(l => l.BorrowCapital)
                .Include(l => l.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanPayment == null)
            {
                return NotFound();
            }

            return View(loanPayment);
        }

        // POST: Admin/LoanPayment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loanPayment = await _context.LoanPayments.FindAsync(id);
            if (loanPayment != null)
            {
                _context.LoanPayments.Remove(loanPayment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanPaymentExists(int id)
        {
            return _context.LoanPayments.Any(e => e.Id == id);
        }
    }
}
