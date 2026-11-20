using Dentigo.Application.Interfaces.Repositories;
using Dentigo.Application.Models;
using Dentigo.Domain.Entities;
using Dentigo.Presentation.Models.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dentigo.Presentation.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IToothRepository _toothRepository;
        private readonly IClinicalFindingRepository _clinicalFindingRepository;
        private readonly IUserRepository _userRepository;

        public PatientsController(
            IPatientRepository patientRepository,
            IToothRepository toothRepository,
            IClinicalFindingRepository clinicalFindingRepository,
            IUserRepository userRepository)
        {
            _patientRepository = patientRepository;
            _toothRepository = toothRepository;
            _clinicalFindingRepository = clinicalFindingRepository;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index(
            string? searchString,
            DateTime? createdFrom,
            DateTime? createdTo,
            int pageNumber = 1,
            CancellationToken cancellationToken = default)
        {
            var parameters = new PatientFilterParameters
            {
                SearchString = searchString,
                CreatedFrom = createdFrom,
                CreatedTo = createdTo,
                PageNumber = pageNumber < 1 ? 1 : pageNumber,
                PageSize = 10
            };

            var pagedResult = await _patientRepository.GetPagedAsync(parameters, cancellationToken);

            var result = pagedResult.Items
                .Select(p => new PatientViewModel
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    PhoneNumber = p.PhoneNumber,
                    CreatedAt = p.CreatedAt
                })
                .ToList();

            ViewBag.SearchString = searchString;
            ViewBag.CreatedFrom = createdFrom?.ToString("yyyy-MM-dd");
            ViewBag.CreatedTo = createdTo?.ToString("yyyy-MM-dd");

            ViewBag.CurrentPage = pagedResult.PageNumber;
            ViewBag.TotalPages = pagedResult.TotalPages;
            ViewBag.TotalCount = pagedResult.TotalCount;
            ViewBag.HasPreviousPage = pagedResult.HasPreviousPage;
            ViewBag.HasNextPage = pagedResult.HasNextPage;

            return View("~/Views/Patient/Index.cshtml", result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Patient/Create.cshtml", new CreatePatientViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePatientViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Patient/Create.cshtml", model);

            var patient = new Patient
            {
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.Now
            };

            await _patientRepository.AddAsync(patient, cancellationToken);
            await _patientRepository.SaveChangesAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
        {
            var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);

            if (patient == null)
                return NotFound();

            var model = new PatientViewModel
            {
                Id = patient.Id,
                FullName = patient.FullName,
                PhoneNumber = patient.PhoneNumber,
                CreatedAt = patient.CreatedAt
            };

            return View("~/Views/Patient/Details.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
        {
            var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);

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
        public async Task<IActionResult> Edit(int id, EditPatientViewModel model, CancellationToken cancellationToken = default)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("~/Views/Patient/Edit.cshtml", model);

            var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);

            if (patient == null)
                return NotFound();

            patient.FullName = model.FullName;
            patient.PhoneNumber = model.PhoneNumber;
            patient.UpdatedAt = DateTime.Now;

            _patientRepository.Update(patient);
            await _patientRepository.SaveChangesAsync(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DentalChart(int id, CancellationToken cancellationToken = default)
        {
            var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);

            if (patient == null)
                return NotFound();

            var teeth = await _toothRepository.GetByPatientIdAsync(id, cancellationToken);

            var model = new DentalChartViewModel
            {
                PatientId = patient.Id,
                PatientName = patient.FullName,
                Teeth = teeth.Select(t => new ToothViewModel
                {
                    Id = t.Id,
                    ToothNumber = t.ToothNumber,
                    Notes = t.Notes
                }).ToList(),
                NewTooth = new AddToothViewModel { PatientId = patient.Id }
            };

            return View("~/Views/Patient/DentalChart.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTooth([Bind(Prefix = "NewTooth")] AddToothViewModel model, CancellationToken cancellationToken = default)
        {
            if (model.PatientId <= 0)
            {
                TempData["Error"] = "Invalid patient ID.";
                return RedirectToAction(nameof(Index));
            }

            var patient = await _patientRepository.GetByIdAsync(model.PatientId, cancellationToken);

            if (patient == null)
            {
                TempData["Error"] = "Patient not found.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var toothExists = await _toothRepository.ExistsAsync(model.PatientId, model.ToothNumber, cancellationToken);

                if (toothExists)
                {
                    ModelState.AddModelError("NewTooth.ToothNumber", "This tooth is already registered.");
                }
            }

            if (!ModelState.IsValid)
            {
                var teeth = await _toothRepository.GetByPatientIdAsync(model.PatientId, cancellationToken);

                var viewModel = new DentalChartViewModel
                {
                    PatientId = patient.Id,
                    PatientName = patient.FullName,
                    Teeth = teeth.Select(t => new ToothViewModel
                    {
                        Id = t.Id,
                        ToothNumber = t.ToothNumber,
                        Notes = t.Notes
                    }).ToList(),
                    NewTooth = model
                };

                return View("~/Views/Patient/DentalChart.cshtml", viewModel);
            }

            var tooth = new Tooth
            {
                PatientId = model.PatientId,
                ToothNumber = model.ToothNumber,
                Notes = model.Notes
            };

            await _toothRepository.AddAsync(tooth, cancellationToken);
            await _toothRepository.SaveChangesAsync(cancellationToken);

            TempData["Success"] = "Tooth added successfully.";

            return RedirectToAction(nameof(DentalChart), new { id = model.PatientId });
        }

        [HttpGet]
        public async Task<IActionResult> AddClinicalFinding(int patientId, CancellationToken cancellationToken = default)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);

            if (patient == null)
                return NotFound();

            var teeth = await _toothRepository.GetByPatientIdAsync(patientId, cancellationToken);

            ViewBag.PatientName = patient.FullName;
            ViewBag.Teeth = new SelectList(teeth, "Id", "ToothNumber");

            return View("~/Views/Patient/AddClinicalFinding.cshtml",
                new CreateClinicalFindingViewModel
                {
                    PatientId = patientId
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClinicalFinding(CreateClinicalFindingViewModel model, CancellationToken cancellationToken = default)
        {
            if (model.PatientId <= 0)
            {
                TempData["Error"] = "Invalid patient ID.";
                return RedirectToAction(nameof(Index));
            }

            var patient = await _patientRepository.GetByIdAsync(model.PatientId, cancellationToken);

            if (patient == null)
            {
                TempData["Error"] = "Patient not found.";
                return RedirectToAction(nameof(Index));
            }

            if (model.ToothId.HasValue)
            {
                var toothExists = await _toothRepository.ExistsByIdAndPatientIdAsync(model.ToothId.Value, model.PatientId, cancellationToken);

                if (!toothExists)
                    ModelState.AddModelError(nameof(model.ToothId), "Invalid tooth selected.");
            }

            if (!ModelState.IsValid)
            {
                var teeth = await _toothRepository.GetByPatientIdAsync(model.PatientId, cancellationToken);

                ViewBag.PatientName = patient.FullName;
                ViewBag.Teeth = new SelectList(teeth, "Id", "ToothNumber", model.ToothId);

                return View("~/Views/Patient/AddClinicalFinding.cshtml", model);
            }

            // NOTE: Temporary default assignment. Authentication and logged-in user identification
            // are not configured yet, so we assign the first active Doctor as the default recorder.
            var doctorId = await _userRepository.GetActiveDoctorIdAsync(cancellationToken);

            if (!doctorId.HasValue || doctorId.Value == 0)
            {
                ModelState.AddModelError(string.Empty, "No active doctor found in the system to assign this clinical finding.");

                var teeth = await _toothRepository.GetByPatientIdAsync(model.PatientId, cancellationToken);

                ViewBag.PatientName = patient.FullName;
                ViewBag.Teeth = new SelectList(teeth, "Id", "ToothNumber", model.ToothId);

                return View("~/Views/Patient/AddClinicalFinding.cshtml", model);
            }

            var finding = new ClinicalFinding
            {
                PatientId = model.PatientId,
                ToothId = model.ToothId!.Value,
                Description = model.Description,
                CreatedByUserId = doctorId.Value,
                CreatedAt = DateTime.Now
            };

            await _clinicalFindingRepository.AddAsync(finding, cancellationToken);
            await _clinicalFindingRepository.SaveChangesAsync(cancellationToken);

            TempData["Success"] = "Clinical finding added successfully.";

            return RedirectToAction(nameof(DentalChart), new { id = model.PatientId });
        }
    }
}
