using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InsureesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Insurees
        public async Task<IActionResult> Index()
        {
            return View(await _context.Insurees.ToListAsync());
        }

        // GET: Insurees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuree = await _context.Insurees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuree == null)
            {
                return NotFound();
            }

            return View(insuree);
        }

        // GET: Insurees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Insurees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                insuree.Quote = CalculateQuote(insuree); // Calculate the quote
                _context.Add(insuree);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(insuree);
        }

   

        // POST: Insurees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (id != insuree.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insuree);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsureeExists(insuree.Id))
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
            return View(insuree);
        }

        // GET: Insurees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuree = await _context.Insurees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuree == null)
            {
                return NotFound();
            }

            return View(insuree);
        }

        // POST: Insurees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insuree = await _context.Insurees.FindAsync(id);
            if (insuree != null)
            {
                _context.Insurees.Remove(insuree);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsureeExists(int id)
        {
            return _context.Insurees.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Admin()
        {
            var insurees = await _context.Insurees.ToListAsync();
            return View(insurees);
        }

        public decimal CalculateQuote(Insuree insuree)
        {
            decimal baseQuote = 50m; // Base price

            // Age-based calculation
            var age = DateTime.Now.Year - insuree.DateOfBirth.Year;
            if (insuree.DateOfBirth.Date > DateTime.Now.AddYears(-age)) age--; // Adjust if birth date hasn't passed this year

            if (age <= 18)
            {
                baseQuote += 100;
            }
            else if (age >= 19 && age <= 25)
            {
                baseQuote += 50;
            }
            else
            {
                baseQuote += 25;
            }

            // Car Year-based calculation
            if (insuree.CarYear < 2000)
            {
                baseQuote += 25;
            }
            else if (insuree.CarYear > 2015)
            {
                baseQuote += 25;
            }

            // Car Make and Model-based calculation
            if (insuree.CarMake.ToLower() == "porsche")
            {
                baseQuote += 25;
                if (insuree.CarModel.ToLower() == "911 carrera")
                {
                    baseQuote += 25; // Additional $25 for Porsche 911 Carrera
                }
            }

            // Speeding Tickets-based calculation
            baseQuote += insuree.SpeedingTickets * 10;

            // DUI-based calculation
            if (insuree.DUI)
            {
                baseQuote *= 1.25m; // Add 25% for DUI
            }

            // Coverage Type-based calculation
            if (insuree.CoverageType) // Full coverage
            {
                baseQuote *= 1.5m; // Add 50% for full coverage
            }

            return baseQuote;
        }

    }
}
