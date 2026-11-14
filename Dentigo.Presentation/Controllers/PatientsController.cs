using Dentigo.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dentigo.Domain.Entities;
using Dentigo.Infrastructure.DbContexts;
using Dentigo.Presentation.Models.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dentigo.Presentation.Controllers
{
    public class PatientsController : Controller
    {
        private readonly DentigoDbContext _context;

        public PatientsController(DentigoDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            IQueryable<Patient> patients = _context.Patients;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                patients = patients.Where(p =>
                    p.FullName.Contains(searchString) ||
                    (p.PhoneNumber != null &&
                     p.PhoneNumber.Contains(searchString)));
            }

            var result = await patients
                .OrderBy(p => p.FullName)
            .Select(p => new Dentigo.Presentation.Models.Patient.PatientViewModel
            {
                    Id = p.Id,
                    FullName = p.FullName,
                    PhoneNumber = p.PhoneNumber,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            ViewBag.SearchString = searchString;

            return View("~/Views/Patient/Index.cshtml", result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Patient/Create.cshtml",
                new CreatePatientViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePatientViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Patient/Create.cshtml", model);

            var patient = new Patient
            {
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.Now
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var patient = await _context.Patients
                .Where(p => p.Id == id)
                .Select(p => new PatientViewModel
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    PhoneNumber = p.PhoneNumber,
                    CreatedAt = p.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (patient == null)
                return NotFound();

            return View("~/Views/Patient/Details.cshtml", patient);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
                return NotFound();

            var model = new EditPatientViewModel
            {
                Id = patient.Id,
                FullName = patient.FullName,
                PhoneNumber = patient.PhoneNumber
            };

            return View("~/Views/Patient/Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditPatientViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("~/Views/Patient/Edit.cshtml", model);

            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
                return NotFound();

            patient.FullName = model.FullName;
            patient.PhoneNumber = model.PhoneNumber;
            patient.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DentalChart(int id)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
                return NotFound();

            var teeth = await _context.Teeth
                .Where(t => t.PatientId == id)
                .Select(t => new ToothViewModel
                {
                    Id = t.Id,
                    ToothNumber = t.ToothNumber,
                    Notes = t.Notes
                })
                .ToListAsync();

            var model = new DentalChartViewModel
            {
                PatientId = patient.Id,
                PatientName = patient.FullName,
                Teeth = teeth
            };

            return View("~/Views/Patient/DentalChart.cshtml", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTooth(AddToothViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(DentalChart), new { id = model.PatientId });

            var patientExists = await _context.Patients
                .AnyAsync(p => p.Id == model.PatientId);

            if (!patientExists)
                return NotFound();

            var toothExists = await _context.Teeth
                .AnyAsync(t => t.PatientId == model.PatientId &&
                               t.ToothNumber == model.ToothNumber);

            if (toothExists)
            {
                TempData["Error"] = "This tooth is already registered.";
                return RedirectToAction(nameof(DentalChart), new { id = model.PatientId });
            }

            var tooth = new Tooth
            {
                PatientId = model.PatientId,
                ToothNumber = model.ToothNumber,
                Notes = model.Notes
            };

            _context.Teeth.Add(tooth);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tooth added successfully.";

            return RedirectToAction(nameof(DentalChart), new { id = model.PatientId });
        }

        [HttpGet]
        public async Task<IActionResult> AddClinicalFinding(int patientId)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null)
                return NotFound();

            ViewBag.PatientName = patient.FullName;

            ViewBag.Teeth = new SelectList(
                await _context.Teeth
                    .Where(t => t.PatientId == patientId)
                    .ToListAsync(),
                "Id",
                "ToothNumber"
            );

            return View("~/Views/Patient/AddClinicalFinding.cshtml",
                new CreateClinicalFindingViewModel
                {
                    PatientId = patientId
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClinicalFinding(
            CreateClinicalFindingViewModel model)
        {
            var patientExists = await _context.Patients
                .AnyAsync(p => p.Id == model.PatientId);

            if (!patientExists)
                return NotFound();

            var toothExists = await _context.Teeth
                .AnyAsync(t => t.Id == model.ToothId &&
                               t.PatientId == model.PatientId);

            if (!toothExists)
                ModelState.AddModelError("ToothId", "Invalid tooth selected.");

            if (!ModelState.IsValid)
            {
                ViewBag.PatientName = await _context.Patients
                    .Where(p => p.Id == model.PatientId)
                    .Select(p => p.FullName)
                    .FirstOrDefaultAsync();

                ViewBag.Teeth = new SelectList(
                    await _context.Teeth
                        .Where(t => t.PatientId == model.PatientId)
                        .ToListAsync(),
                    "Id",
                    "ToothNumber",
                    model.ToothId
                );

                return View("~/Views/Patient/AddClinicalFinding.cshtml", model);
            }

            var doctorId = await _context.Users
                .Where(u => u.Role == UserRole.Doctor && u.IsActive)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (doctorId == 0)
                return BadRequest("No active doctor found.");

            var finding = new ClinicalFinding
            {
                PatientId = model.PatientId,
                ToothId = model.ToothId,
                Description = model.Description,
                CreatedByUserId = doctorId,
                CreatedAt = DateTime.Now
            };

            _context.ClinicalFindings.Add(finding);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Clinical finding added successfully.";

            return RedirectToAction(nameof(DentalChart),
                new { id = model.PatientId });
        }

    }

}
