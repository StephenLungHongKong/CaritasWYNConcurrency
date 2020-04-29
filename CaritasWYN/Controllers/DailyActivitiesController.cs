using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CaritasWYN.Data;
using CaritasWYN.Models;

namespace CaritasWYN.Controllers
{
    public class DailyActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DailyActivities
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DailyActivities.Include(d => d.Act_Type).Include(d => d.Staff);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DailyActivities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyActivity = await _context.DailyActivities
                .Include(d => d.Act_Type)
                .Include(d => d.Staff)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DailyActivityId == id);
            if (dailyActivity == null)
            {
                return NotFound();
            }

            return View(dailyActivity);
        }

        // GET: DailyActivities/Create
        public IActionResult Create()
        {
            ViewData["Act_typeId"] = new SelectList(_context.Act_Types, "Act_typeId", "ActivityName");
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "FirstName");
            return View();
        }

        // POST: DailyActivities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DailyActivityId,DateStart,DateEnd,Act_typeId,StaffId,RowVersion")] DailyActivity dailyActivity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dailyActivity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Act_typeId"] = new SelectList(_context.Act_Types, "Act_typeId", "ActivityName", dailyActivity.Act_typeId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "FirstName", dailyActivity.StaffId);
            return View(dailyActivity);
        }

        // GET: DailyActivities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyActivity = await _context.DailyActivities
                .Include(d => d.Act_Type)
                .Include(d => d.Staff)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DailyActivityId == id);

            if (dailyActivity == null)
            {
                return NotFound();
            }
            ViewData["Act_typeId"] = new SelectList(_context.Act_Types, "Act_typeId", "ActivityName", dailyActivity.Act_typeId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "FirstName", dailyActivity.StaffId);
            return View(dailyActivity);
        }

        // POST: DailyActivities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            
            var dailyActivityToUpdate= await _context.DailyActivities
                .Include(d => d.Act_Type)
                .Include(d => d.Staff)
                .FirstOrDefaultAsync(m => m.DailyActivityId == id);


            if (dailyActivityToUpdate == null)
            {
                DailyActivity deleteDailyActivity = new DailyActivity();
                await TryUpdateModelAsync(deleteDailyActivity);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The Activity was deleted by another user.");
                ViewData["Act_typeId"] = new SelectList(_context.Act_Types, "Act_typeId", "ActivityName", deleteDailyActivity.Act_typeId);
                ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "FirstName", deleteDailyActivity.StaffId);
                return View(deleteDailyActivity);
            }

            _context.Entry(dailyActivityToUpdate).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<DailyActivity>(
                dailyActivityToUpdate,
                "",
                s => s.DateStart, s => s.DateEnd, s => s.Act_typeId, s => s.StaffId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (DailyActivity)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The Daily Activity was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (DailyActivity)databaseEntry.ToObject();

                        if (databaseValues.DateStart != clientValues.DateStart)
                        {
                            ModelState.AddModelError("DateStart", $"Current value: {databaseValues.DateStart:d}");
                        }
                        if (databaseValues.DateEnd != clientValues.DateEnd)
                        {
                            ModelState.AddModelError("DateEnd", $"Current value: {databaseValues.DateEnd:t}");
                        }
                        if (databaseValues.Act_typeId != clientValues.Act_typeId)
                        {
                            Act_type databaseAct_type = await _context.Act_Types.FirstOrDefaultAsync(i => i.Act_typeId == databaseValues.Act_typeId);
                            ModelState.AddModelError("Act_typeId", $"Current value: {databaseAct_type.ActivityName}");
                        }
                        if (databaseValues.StaffId != clientValues.StaffId)
                        {
                            Staff databaseStaff = await _context.Staffs.FirstOrDefaultAsync(i => i.StaffId == databaseValues.StaffId);
                            ModelState.AddModelError("StaffId", $"Current value: {databaseStaff.FirstName}");
                        }

                        ModelState.AddModelError(string.Empty, "Record is modified by another user. "
                                + " Records is not updated yet. "
                                + " To cancel update close this windows or click back to list "
                                + " To update press save again" );
                        dailyActivityToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }

                }
            }
                ViewData["Act_typeId"] = new SelectList(_context.Act_Types, "Act_typeId", "ActivityName", dailyActivityToUpdate.Act_typeId);
                ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "FirstName", dailyActivityToUpdate.StaffId);
                return View(dailyActivityToUpdate);
        }

        // GET: DailyActivities/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyActivity = await _context.DailyActivities
                .Include(d => d.Act_Type)
                .Include(d => d.Staff)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DailyActivityId == id);
            if (dailyActivity == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete"
                    + " was modified by another user after you got the original vlaues."
                    + " The delete operation was cancelled"
                    + " You may try again!";
            }
            return View(dailyActivity);
        }

        // POST: DailyActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DailyActivity dailyActivity)
        {
            try
            {
                if(await _context.DailyActivities.AnyAsync(m => m.DailyActivityId == dailyActivity.DailyActivityId))
                {
                    _context.DailyActivities.Remove(dailyActivity);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = dailyActivity.DailyActivityId });
            }            
        }

        private bool DailyActivityExists(int id)
        {
            return _context.DailyActivities.Any(e => e.DailyActivityId == id);
        }
    }
}
