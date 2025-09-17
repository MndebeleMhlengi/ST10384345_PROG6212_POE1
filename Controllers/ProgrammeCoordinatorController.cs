using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CMCS.Data;
using CMCS.Models;
using System.Security.Claims;

namespace CMCS.Controllers
{
    [Authorize(Roles = "PROGRAMME_COORDINATOR")]
    public class ProgrammeCoordinatorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgrammeCoordinatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var pendingClaims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.PENDING)
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .OrderBy(c => c.SubmissionDate)
                .ToListAsync();

            ViewBag.TotalPending = pendingClaims.Count;
            ViewBag.TotalApproved = await _context.Claims.CountAsync(c => c.Status == ClaimStatus.APPROVED_PC);

            return View(pendingClaims);
        }

        public async Task<IActionResult> ReviewClaim(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .Include(c => c.Approvals)
                .FirstOrDefaultAsync(c => c.ClaimId == id);

            if (claim == null)
                return NotFound();

            return View(claim);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int claimId, string comments)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var claim = await _context.Claims.FindAsync(claimId);

            if (claim != null)
            {
                claim.Status = ClaimStatus.APPROVED_PC;
                claim.LastModifiedDate = DateTime.Now;

                var approval = new ClaimApproval
                {
                    ClaimId = claimId,
                    ApproverId = userId,
                    Level = ApprovalLevel.PROGRAMME_COORDINATOR,
                    Status = ApprovalStatus.APPROVED,
                    Comments = comments,
                    ApprovalDate = DateTime.Now
                };

                _context.ClaimApprovals.Add(approval);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Claim approved successfully!";
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int claimId, string reason)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var claim = await _context.Claims.FindAsync(claimId);

            if (claim != null)
            {
                claim.Status = ClaimStatus.REJECTED;
                claim.LastModifiedDate = DateTime.Now;

                var approval = new ClaimApproval
                {
                    ClaimId = claimId,
                    ApproverId = userId,
                    Level = ApprovalLevel.PROGRAMME_COORDINATOR,
                    Status = ApprovalStatus.REJECTED,
                    RejectionReason = reason,
                    ApprovalDate = DateTime.Now
                };

                _context.ClaimApprovals.Add(approval);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Claim rejected.";
            }

            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> ApprovedClaims()
        {
            var approvedClaims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.APPROVED_PC)
                .Include(c => c.Lecturer)
                .OrderByDescending(c => c.LastModifiedDate)
                .ToListAsync();

            return View(approvedClaims);
        }
    }
}