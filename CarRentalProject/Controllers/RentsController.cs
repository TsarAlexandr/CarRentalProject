﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarRentalProject.Data;
using CarRentalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Routing;

namespace CarRentalProject.Controllers
{
    public class RentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RentsController(ApplicationDbContext context, UserManager<ApplicationUser>  usr)
        {
            _context = context;
            _userManager = usr;
        }
        [Authorize(Roles="admin")]
        // GET: Rents
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rents.ToListAsync());
        }

        // GET: Rents/Details/5
        [Authorize]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rents = await _context.Rents
                .SingleOrDefaultAsync(m => m.ID == id);
            if (rents == null)
            {
                return NotFound();
            }

            return View(rents);
        }

        // GET: Rents/Create
        [Authorize]

        public IActionResult Create(int? id)
        {
           return View();
        }

        // POST: Rents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, DateTime startDate, DateTime endDate)
        {

            var otherRents = _context.Rents.Where(x => x.CarID == id);
            foreach (var rent in otherRents)
            {
                if (((startDate >= rent.StartDate) && (startDate <= rent.EndDate)) ||
                    ((endDate >= rent.StartDate) && (endDate <= rent.EndDate)) ||
                    ((startDate <= rent.StartDate) && (endDate >= rent.EndDate)))
                {
                    ModelState.AddModelError("", "Unfortunatly, this date are taken. Please select another one");
                    TempData["message"] = "Unfortunatly, this date are taken. Please select another one";
                    break;
                }

            }

            if (endDate < startDate)
            {
                ModelState.AddModelError("", "Invalid data input");
                TempData["message"] = "Invalid data input";
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var rents = new Rents();
                rents.CarID = id;
                rents.StartDate = startDate;
                rents.EndDate = endDate;
                rents.UserID = user?.Id;
                _context.Add(rents);
                await _context.SaveChangesAsync();
                TempData["message"] = "The car was rented succesfull!";
                return RedirectToAction("Index", "Autos");
            }
            return RedirectToAction("Details", "Autos", new {id = id });
            
        }


        // GET: Rents/Edit/5
        [Authorize]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rents = await _context.Rents.SingleOrDefaultAsync(m => m.ID == id);
            if (rents == null)
            {
                return NotFound();
            }
            return View(rents);
        }

        // POST: Rents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public async Task<IActionResult> Edit(int id, [Bind("ID,UserID,CarID,StartDate,EndDate")] Rents rents)
        {
            if (id != rents.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rents);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentsExists(rents.ID))
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
            return View(rents);
        }

        // GET: Rents/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rents = await _context.Rents
                .SingleOrDefaultAsync(m => m.ID == id);
            if (rents == null)
            {
                return NotFound();
            }

            return View(rents);
        }

        // POST: Rents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rents = await _context.Rents.SingleOrDefaultAsync(m => m.ID == id);
            _context.Rents.Remove(rents);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        private bool RentsExists(int id)
        {
            return _context.Rents.Any(e => e.ID == id);
        }
    }
}
