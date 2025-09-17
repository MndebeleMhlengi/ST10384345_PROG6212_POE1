using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CMCS.Data;
using CMCS.Models;

namespace CMCS.Controllers
{
    [Authorize(Roles = "HR")]
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HRController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var finalApprovedClaims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.APPROVED_FINAL)
                .Include(c => c.Lecturer)
                .OrderByDescending(c => c.LastModifiedDate)
                .ToListAsync();

            ViewBag.TotalForPayment = finalApprovedClaims.Count;
            ViewBag.TotalAmount = finalApprovedClaims.Sum(c => c.TotalAmount);
            ViewBag.PaidClaims = await _context.Claims.CountAsync(c => c.Status == ClaimStatus.PAID);

            return View(finalApprovedClaims);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsPaid(int claimId)
        {
            var claim = await _context.Claims.FindAsync(claimId);

            if (claim != null)
            {
                claim.Status = ClaimStatus.PAID;
                claim.LastModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Claim marked as paid!";
            }

            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> ManageLecturers()
        {
            var lecturers = await _context.Users
                .Where(u => u.Role == UserRole.LECTURER)
                .ToListAsync();

            return View(lecturers);
        }

        public async Task<IActionResult> PaymentReports()
        {
            var paidClaims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.PAID)
                .Include(c => c.Lecturer)
                .OrderByDescending(c => c.LastModifiedDate)
                .ToListAsync();

            return View(paidClaims);
        }
    }
}