using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CMCS.Data;
using CMCS.Models;
using CMCS.Models.ViewModels;
using System.Security.Claims;

namespace CMCS.Controllers
{
    [Authorize(Roles = "LECTURER")]
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LecturerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FindAsync(userId);

            var claims = await _context.Claims
                .Where(c => c.LecturerId == userId)
                .OrderByDescending(c => c.SubmissionDate)
                .Take(10)
                .ToListAsync();

            ViewBag.User = user;
            ViewBag.PendingClaims = claims.Count(c => c.Status == ClaimStatus.PENDING);
            ViewBag.ApprovedClaims = claims.Count(c => c.Status == ClaimStatus.APPROVED_FINAL);

            return View(claims);
        }

        // GET: Submit Claim
        public IActionResult SubmitClaim()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _context.Users.Find(userId);

            var model = new ClaimSubmissionViewModel
            {
                HourlyRate = user?.HourlyRate ?? 0
            };

            return View(model);
        }

        // POST: Submit Claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(ClaimSubmissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var claim = new CMCS.Models.Claim
                {
                    LecturerId = userId,
                    MonthWorked = model.MonthWorked,
                    YearWorked = model.YearWorked,
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    ModuleTaught = model.ModuleTaught,
                    AdditionalNotes = model.AdditionalNotes,
                    Status = ClaimStatus.PENDING,
                    SubmissionDate = DateTime.Now,
                    ClaimReference = GenerateClaimReference()
                };

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(model);
        }

        public async Task<IActionResult> ViewClaims()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var claims = await _context.Claims
                .Where(c => c.LecturerId == userId)
                .Include(c => c.Documents)
                .Include(c => c.Approvals)
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            return View(claims);
        }

        public IActionResult UploadDocuments(int claimId)
        {
            ViewBag.ClaimId = claimId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocuments(int claimId, IFormFile file, string description)
        {
            if (file != null && file.Length > 0)
            {
                // Validate file type and size
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["ErrorMessage"] = "Only PDF, DOCX, and XLSX files are allowed.";
                    return RedirectToAction("UploadDocuments", new { claimId });
                }

                if (file.Length > 10 * 1024 * 1024) // 10MB limit
                {
                    TempData["ErrorMessage"] = "File size cannot exceed 10MB.";
                    return RedirectToAction("UploadDocuments", new { claimId });
                }

                // Save file (in production, use cloud storage)
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Save document record
                var document = new Document
                {
                    ClaimId = claimId,
                    FileName = file.FileName,
                    FilePath = fileName,
                    FileType = extension,
                    FileSize = file.Length,
                    Description = description,
                    ContentType = file.ContentType,
                    UploadDate = DateTime.Now
                };

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Document uploaded successfully!";
            }

            return RedirectToAction("ViewClaims");
        }

        private string GenerateClaimReference()
        {
            return $"CLM-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }
    }
}
