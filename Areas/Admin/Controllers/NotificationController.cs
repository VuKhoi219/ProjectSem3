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
    public class NotificationController : AdminBaseController
    {
        private readonly MyDbContext _context;

        public NotificationController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Notification
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Notifications.Include(n => n.Creator).Include(n => n.Deleter).Include(n => n.Updater).Include(n => n.User);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Admin/Notification/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.Creator)
                .Include(n => n.Deleter)
                .Include(n => n.Updater)
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: Admin/Notification/Create
        public IActionResult Create()
        {
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard");
            return View();
        }

        // POST: Admin/Notification/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Message,IsRead,CreatedAt,UpdatedAt,DeleteAt,CreatedBy,UpdatedBy,DeleteBy")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", notification.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", notification.DeleteBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", notification.UpdatedBy);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", notification.UserId);
            return View(notification);
        }

        // GET: Admin/Notification/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", notification.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", notification.DeleteBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", notification.UpdatedBy);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", notification.UserId);
            return View(notification);
        }

        // POST: Admin/Notification/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Message,IsRead,CreatedAt,UpdatedAt,DeleteAt,CreatedBy,UpdatedBy,DeleteBy")] Notification notification)
        {
            if (id != notification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.Id))
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
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", notification.CreatedBy);
            ViewData["DeleteBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", notification.DeleteBy);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", notification.UpdatedBy);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "CitizenIdentificationCard", notification.UserId);
            return View(notification);
        }

        // GET: Admin/Notification/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.Creator)
                .Include(n => n.Deleter)
                .Include(n => n.Updater)
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Admin/Notification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
