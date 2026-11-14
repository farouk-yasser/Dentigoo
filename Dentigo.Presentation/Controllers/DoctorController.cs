using System.Globalization;
using System.Linq.Expressions;
using System.Security.Claims;
using Dentigo.Domain.Entities;
using Dentigo.Domain.Enums;
using Dentigo.Infrastructure.DbContexts;
using Dentigo.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dentigo.Presentation.Controllers;

public sealed class DoctorController(DentigoDbContext db, IConfiguration configuration, IWebHostEnvironment environment) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();

        var cases = await db.DentalCases.AsNoTracking()
            .Where(c => c.CreatedByUserId == doctorId)
            .Include(c => c.Patient)
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .ToListAsync(cancellationToken);

        var latestDesigns = await GetLatestDesignsAsync(doctorId, cancellationToken);
        var pending = latestDesigns
            .Where(d => d.ReviewStatus == DesignReviewStatus.PendingReview)
            .OrderByDescending(d => d.SubmittedAt)
            .Select(ToReviewRow)
            .ToList();

        var model = new DoctorDashboardViewModel
        {
            ActiveCases = cases.Count(c => c.Status is not (DentalCaseStatus.Completed or DentalCaseStatus.Cancelled
                or DentalCaseStatus.DesignSubmitted or DentalCaseStatus.RevisionRequested)),
            DoctorReview = pending.Count,
            RevisionRequested = cases.Count(c => c.Status == DentalCaseStatus.RevisionRequested),
            CompletedCases = cases.Count(c => c.Status == DentalCaseStatus.Completed),
            RecentCases = cases.Take(8).Select(c => new DoctorCaseRowViewModel
            {
                CaseId = c.Id,
                PatientName = c.Patient.FullName,
                TreatmentName = c.TreatmentName,
                Status = c.Status,
                UpdatedAt = c.UpdatedAt ?? c.CreatedAt
            }).ToList(),
            PendingReviews = pending.Take(5).ToList()
        };

        return View(model);
    }

    // ------------------------------------------------------------------
    // Design review queue (search + filter + sort + server-side pagination)
    // ------------------------------------------------------------------
    public async Task<IActionResult> Reviews(ReviewListViewModel vm, CancellationToken cancellationToken)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (!ModelState.IsValid)
        {
            vm = new ReviewListViewModel(); // show defaults; the field messages come from ModelState
        }

        var q = LatestDesigns(doctorId);

        if (vm.Status is { } status) q = q.Where(d => d.ReviewStatus == status);
        if (vm.Treatment is { Length: > 0 } treatment) q = q.Where(d => d.DentalCase.TreatmentName == treatment);
        if (vm.Search is { } term)
        {
            var hasCaseId = TryParseCaseId(term, out var caseId);
            q = q.Where(d => d.DentalCase.Patient.FullName.Contains(term)
                || d.DentalCase.TreatmentName.Contains(term)
                || d.CaseFile.FileName.Contains(term)
                || d.SubmittedByUser.FullName.Contains(term)
                || (hasCaseId && d.DentalCaseId == caseId));
        }
        if (vm.From is { } fromDate) { var fromUtc = ToUtcStart(fromDate); q = q.Where(d => d.SubmittedAt >= fromUtc); }
        if (vm.To is { } toDate) { var toUtc = ToUtcStart(toDate.Date.AddDays(1)); q = q.Where(d => d.SubmittedAt < toUtc); }

        var asc = vm.Dir == "asc";
        IOrderedQueryable<DesignVersion> ordered = vm.Sort switch
        {
            "case" => q.OrderByDirection(d => d.DentalCaseId, asc),
            "patient" => q.OrderByDirection(d => d.DentalCase.Patient.FullName, asc),
            "treatment" => q.OrderByDirection(d => d.DentalCase.TreatmentName, asc),
            "version" => q.OrderByDirection(d => d.VersionNumber, asc),
            _ => q.OrderByDirection(d => d.SubmittedAt, asc)
        };
        ordered = ordered.ThenBy(d => d.Id);

        vm.Results = await ToPageAsync(q, ordered, vm, d => new DesignReviewRowViewModel
        {
            DesignVersionId = d.Id,
            CaseId = d.DentalCaseId,
            PatientName = d.DentalCase.Patient.FullName,
            TreatmentName = d.DentalCase.TreatmentName,
            FileName = d.CaseFile.FileName,
            VersionNumber = d.VersionNumber,
            SubmittedBy = d.SubmittedByUser.FullName,
            SubmittedAt = d.SubmittedAt,
            ReviewStatus = d.ReviewStatus
        }, cancellationToken);

        vm.Treatments = await LatestDesigns(doctorId)
            .Select(d => d.DentalCase.TreatmentName).Distinct().OrderBy(n => n)
            .ToListAsync(cancellationToken);
        return View(vm);
    }

    // ------------------------------------------------------------------
    // All cases (search + filter + sort + server-side pagination)
    // ------------------------------------------------------------------
    public async Task<IActionResult> Cases(CaseListViewModel vm, CancellationToken cancellationToken)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (!ModelState.IsValid)
        {
            vm = new CaseListViewModel();
        }

        var q = db.DentalCases.AsNoTracking().Where(c => c.CreatedByUserId == doctorId);

        if (vm.Status is { } status) q = q.Where(c => c.Status == status);
        if (vm.Search is { } term)
        {
            var hasCaseId = TryParseCaseId(term, out var caseId);
            q = q.Where(c => c.Patient.FullName.Contains(term)
                || c.TreatmentName.Contains(term)
                || (hasCaseId && c.Id == caseId));
        }
        if (vm.From is { } fromDate) { var fromUtc = ToUtcStart(fromDate); q = q.Where(c => (c.UpdatedAt ?? c.CreatedAt) >= fromUtc); }
        if (vm.To is { } toDate) { var toUtc = ToUtcStart(toDate.Date.AddDays(1)); q = q.Where(c => (c.UpdatedAt ?? c.CreatedAt) < toUtc); }

        var asc = vm.Dir == "asc";
        IOrderedQueryable<DentalCase> ordered = vm.Sort switch
        {
            "case" => q.OrderByDirection(c => c.Id, asc),
            "patient" => q.OrderByDirection(c => c.Patient.FullName, asc),
            "treatment" => q.OrderByDirection(c => c.TreatmentName, asc),
            "status" => q.OrderByDirection(c => c.Status, asc),
            _ => q.OrderByDirection(c => c.UpdatedAt ?? c.CreatedAt, asc)
        };
        ordered = ordered.ThenBy(c => c.Id);

        vm.Results = await ToPageAsync(q, ordered, vm, c => new DoctorCaseRowViewModel
        {
            CaseId = c.Id,
            PatientName = c.Patient.FullName,
            TreatmentName = c.TreatmentName,
            Status = c.Status,
            UpdatedAt = c.UpdatedAt ?? c.CreatedAt
        }, cancellationToken);

        return View(vm);
    }

    // ------------------------------------------------------------------
    // Review one design
    // ------------------------------------------------------------------
    public async Task<IActionResult> Review(int id, CancellationToken cancellationToken)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var model = await BuildReviewModelAsync(id, doctorId, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    private async Task<DesignReviewDetailViewModel?> BuildReviewModelAsync(int id, int doctorId, CancellationToken cancellationToken)
    {
        var design = await db.DesignVersions.AsNoTracking()
            .Include(d => d.CaseFile)
            .Include(d => d.SubmittedByUser)
            .Include(d => d.DentalCase).ThenInclude(c => c.Patient)
            .SingleOrDefaultAsync(d => d.Id == id && d.DentalCase.CreatedByUserId == doctorId, cancellationToken);
        if (design is null) return null;

        var versions = await db.DesignVersions.AsNoTracking()
            .Where(d => d.DentalCaseId == design.DentalCaseId)
            .Include(d => d.CaseFile)
            .Include(d => d.SubmittedByUser)
            .OrderByDescending(d => d.VersionNumber)
            .ToListAsync(cancellationToken);
        var latestVersionNumber = versions.Max(d => d.VersionNumber);

        return new DesignReviewDetailViewModel
        {
            DesignVersionId = design.Id,
            CaseId = design.DentalCaseId,
            PatientName = design.DentalCase.Patient.FullName,
            TreatmentName = design.DentalCase.TreatmentName,
            DoctorInstructions = design.DentalCase.DoctorInstructions,
            FileName = design.CaseFile.FileName,
            FileType = design.CaseFile.FileType,
            FileSize = design.CaseFile.FileSize,
            SubmittedBy = design.SubmittedByUser.FullName,
            SubmittedAt = design.SubmittedAt,
            VersionNumber = design.VersionNumber,
            ReviewStatus = design.ReviewStatus,
            ReviewReason = design.ReviewReason,
            IsLatestVersion = design.VersionNumber == latestVersionNumber,
            PreviousVersions = versions.Where(v => v.Id != design.Id).Select(v => new DesignReviewRowViewModel
            {
                DesignVersionId = v.Id,
                CaseId = v.DentalCaseId,
                PatientName = design.DentalCase.Patient.FullName,
                TreatmentName = design.DentalCase.TreatmentName,
                FileName = v.CaseFile.FileName,
                VersionNumber = v.VersionNumber,
                SubmittedBy = v.SubmittedByUser.FullName,
                SubmittedAt = v.SubmittedAt,
                ReviewStatus = v.ReviewStatus
            }).ToList(),
            ApproveForm = new ApproveDesignViewModel { DesignVersionId = design.Id },
            RevisionForm = new RequestRevisionViewModel { DesignVersionId = design.Id },
            RejectForm = new RejectDesignViewModel { DesignVersionId = design.Id }
        };
    }

    public async Task<IActionResult> DownloadDesign(int id, CancellationToken cancellationToken)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var design = await db.DesignVersions.AsNoTracking()
            .Include(d => d.CaseFile)
            .Include(d => d.DentalCase)
            .SingleOrDefaultAsync(d => d.Id == id && d.DentalCase.CreatedByUserId == doctorId, cancellationToken);
        if (design is null) return NotFound();

        var storageRoot = configuration["CaseFiles:StorageRoot"];
        if (string.IsNullOrWhiteSpace(storageRoot))
            storageRoot = Path.Combine(environment.ContentRootPath, "App_Data", "case-files");

        var root = Path.GetFullPath(Path.IsPathRooted(storageRoot)
            ? storageRoot
            : Path.Combine(environment.ContentRootPath, storageRoot));
        var rootPrefix = Path.EndsInDirectorySeparator(root) ? root : root + Path.DirectorySeparatorChar;
        var filePath = Path.GetFullPath(Path.Combine(root, design.CaseFile.StoragePath));
        if (!filePath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(filePath))
            return NotFound();

        var contentType = design.CaseFile.FileType.ToLowerInvariant() switch
        {
            ".pdf" or "pdf" => "application/pdf",
            ".jpg" or ".jpeg" or "jpg" or "jpeg" => "image/jpeg",
            ".png" or "png" => "image/png",
            _ => "application/octet-stream"
        };
        return PhysicalFile(filePath, contentType, design.CaseFile.FileName, enableRangeProcessing: true);
    }

    public async Task<IActionResult> PreviewDesign(int id, CancellationToken cancellationToken)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var design = await db.DesignVersions.AsNoTracking()
            .Include(d => d.CaseFile)
            .Include(d => d.DentalCase)
            .SingleOrDefaultAsync(d => d.Id == id && d.DentalCase.CreatedByUserId == doctorId, cancellationToken);
        if (design is null || !string.Equals(Path.GetExtension(design.CaseFile.FileName), ".stl", StringComparison.OrdinalIgnoreCase))
            return NotFound();

        var configuredRoot = configuration["CaseFiles:StorageRoot"];
        var storageRoot = string.IsNullOrWhiteSpace(configuredRoot)
            ? Path.Combine(environment.ContentRootPath, "App_Data", "case-files")
            : Path.IsPathRooted(configuredRoot) ? configuredRoot : Path.Combine(environment.ContentRootPath, configuredRoot);
        var root = Path.GetFullPath(storageRoot);
        var rootPrefix = Path.EndsInDirectorySeparator(root) ? root : root + Path.DirectorySeparatorChar;
        var filePath = Path.GetFullPath(Path.Combine(root, design.CaseFile.StoragePath));
        if (!filePath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(filePath))
            return NotFound();

        return PhysicalFile(filePath, "application/octet-stream", enableRangeProcessing: true);
    }

    // ------------------------------------------------------------------
    // Decisions. Each form is bound to its own ViewModel (prefix = property name on the page model),
    // so ModelState keys match the asp-for names and the page can be redisplayed with the errors.
    // ------------------------------------------------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(
        [Bind(Prefix = nameof(DesignReviewDetailViewModel.ApproveForm))] ApproveDesignViewModel input,
        CancellationToken cancellationToken)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (!ModelState.IsValid)
        {
            TempData["ReviewError"] = "The approval request was not valid.";
            return RedirectToAction(nameof(Reviews));
        }
        return await RecordDecision(doctorId, input.DesignVersionId, DesignReviewStatus.Approved, null, cancellationToken);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestRevision(
        [Bind(Prefix = nameof(DesignReviewDetailViewModel.RevisionForm))] RequestRevisionViewModel input,
        CancellationToken cancellationToken)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (!ModelState.IsValid) return await RedisplayReviewAsync(input.DesignVersionId, doctorId, cancellationToken);
        return await RecordDecision(doctorId, input.DesignVersionId, DesignReviewStatus.RevisionRequested, input.Reason, cancellationToken);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(
        [Bind(Prefix = nameof(DesignReviewDetailViewModel.RejectForm))] RejectDesignViewModel input,
        CancellationToken cancellationToken)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (!ModelState.IsValid) return await RedisplayReviewAsync(input.DesignVersionId, doctorId, cancellationToken);
        return await RecordDecision(doctorId, input.DesignVersionId, DesignReviewStatus.Rejected, input.Reason, cancellationToken);
    }

    private async Task<IActionResult> RedisplayReviewAsync(int designVersionId, int doctorId, CancellationToken cancellationToken)
    {
        var model = await BuildReviewModelAsync(designVersionId, doctorId, cancellationToken);
        return model is null ? NotFound() : View("Review", model);
    }

    // ------------------------------------------------------------------
    // Case history (search + filter + order + server-side pagination)
    // ------------------------------------------------------------------
    public async Task<IActionResult> History(CaseHistoryViewModel vm, CancellationToken cancellationToken)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (!ModelState.IsValid)
        {
            vm = new CaseHistoryViewModel { CaseId = vm.CaseId };
        }

        var caseId = vm.CaseId;
        var dentalCase = await db.DentalCases.AsNoTracking()
            .Include(c => c.Patient)
            .SingleOrDefaultAsync(c => c.Id == caseId && c.CreatedByUserId == doctorId, cancellationToken);
        if (dentalCase is null) return NotFound();

        vm.PatientName = dentalCase.Patient.FullName;
        vm.TreatmentName = dentalCase.TreatmentName;

        var all = db.CaseHistoryEvents.AsNoTracking().Where(e => e.DentalCaseId == caseId);
        var q = all;

        if (vm.EventType is { Length: > 0 } eventType) q = q.Where(e => e.EventType == eventType);
        if (vm.Search is { } term)
        {
            q = q.Where(e => e.Description.Contains(term)
                || e.EventType.Contains(term)
                || (e.Reason != null && e.Reason.Contains(term))
                || (e.ActorUser != null && e.ActorUser.FullName.Contains(term)));
        }
        if (vm.From is { } fromDate) { var fromUtc = ToUtcStart(fromDate); q = q.Where(e => e.CreatedAt >= fromUtc); }
        if (vm.To is { } toDate) { var toUtc = ToUtcStart(toDate.Date.AddDays(1)); q = q.Where(e => e.CreatedAt < toUtc); }

        var asc = vm.Dir == "asc";
        var ordered = q.OrderByDirection(e => e.CreatedAt, asc).ThenBy(e => e.Id);

        vm.Results = await ToPageAsync(q, ordered, vm, e => new CaseHistoryRowViewModel
        {
            EventType = e.EventType,
            Description = e.Description,
            ActorName = e.ActorUser == null ? null : e.ActorUser.FullName,
            PreviousStatus = e.PreviousStatus,
            NewStatus = e.NewStatus,
            Reason = e.Reason,
            CreatedAt = e.CreatedAt
        }, cancellationToken);

        vm.EventTypes = await all.Select(e => e.EventType).Distinct().OrderBy(t => t).ToListAsync(cancellationToken);
        return View(vm);
    }

    // ------------------------------------------------------------------
    // Notifications (search + filter + sort + server-side pagination)
    // ------------------------------------------------------------------
    public async Task<IActionResult> Notifications(NotificationListViewModel vm, CancellationToken cancellationToken)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        if (!ModelState.IsValid)
        {
            vm = new NotificationListViewModel();
        }

        var all = db.Notifications.AsNoTracking().Where(n => n.RecipientUserId == doctorId);
        var q = all;

        if (vm.Filter == "unread") q = q.Where(n => !n.IsRead);
        else if (vm.Filter == "read") q = q.Where(n => n.IsRead);
        if (vm.Type is { Length: > 0 } type) q = q.Where(n => n.NotificationType == type);
        if (vm.Search is { } term) q = q.Where(n => n.Message.Contains(term) || n.NotificationType.Contains(term));
        if (vm.From is { } fromDate) { var fromUtc = ToUtcStart(fromDate); q = q.Where(n => n.CreatedAt >= fromUtc); }
        if (vm.To is { } toDate) { var toUtc = ToUtcStart(toDate.Date.AddDays(1)); q = q.Where(n => n.CreatedAt < toUtc); }

        var asc = vm.Dir == "asc";
        var ordered = (vm.Sort == "type"
                ? q.OrderByDirection(n => n.NotificationType, asc)
                : q.OrderByDirection(n => n.CreatedAt, asc))
            .ThenByDescending(n => n.Id);

        vm.Results = await ToPageAsync(q, ordered, vm, n => new DoctorNotificationRowViewModel
        {
            NotificationId = n.Id,
            CaseId = n.DentalCaseId,
            NotificationType = n.NotificationType,
            Message = n.Message,
            CreatedAt = n.CreatedAt,
            IsRead = n.IsRead
        }, cancellationToken);

        vm.UnreadCount = await all.CountAsync(n => !n.IsRead, cancellationToken);
        vm.Types = await all.Select(n => n.NotificationType).Distinct().OrderBy(t => t).ToListAsync(cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id, string? returnUrl, CancellationToken cancellationToken)
    {
        if (!TryGetDoctorId(out var doctorId)) return Unauthorized();
        var notification = await db.Notifications.SingleOrDefaultAsync(
            n => n.Id == id && n.RecipientUserId == doctorId, cancellationToken);
        if (notification is null) return NotFound();
        notification.IsRead = true;
        await db.SaveChangesAsync(cancellationToken);
        // Go back to the same filtered/paged list (only to a local URL, to avoid open redirects).
        return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToAction(nameof(Notifications));
    }

    private async Task<IActionResult> RecordDecision(
        int doctorId,
        int designVersionId,
        DesignReviewStatus decision,
        string? reason,
        CancellationToken cancellationToken)
    {
        var design = await db.DesignVersions
            .Include(d => d.DentalCase)
            .Include(d => d.SubmittedByUser)
            .SingleOrDefaultAsync(d => d.Id == designVersionId
                && d.DentalCase.CreatedByUserId == doctorId, cancellationToken);
        if (design is null) return NotFound();

        var isLatest = !await db.DesignVersions.AnyAsync(
            newer => newer.DentalCaseId == design.DentalCaseId && newer.VersionNumber > design.VersionNumber,
            cancellationToken);
        if (!isLatest || design.ReviewStatus != DesignReviewStatus.PendingReview)
        {
            TempData["ReviewError"] = "This design is no longer the latest version awaiting review.";
            return RedirectToAction(nameof(Review), new { id = design.Id });
        }

        var previousStatus = design.DentalCase.Status.ToString();
        design.ReviewStatus = decision;
        design.ReviewReason = reason?.Trim();
        design.DentalCase.Status = decision == DesignReviewStatus.Approved
            ? DentalCaseStatus.Approved
            : DentalCaseStatus.RevisionRequested;
        design.DentalCase.UpdatedAt = DateTime.UtcNow;

        var actionName = decision switch
        {
            DesignReviewStatus.Approved => "DesignApproved",
            DesignReviewStatus.Rejected => "DesignRejected",
            _ => "RevisionRequested"
        };
        var description = decision switch
        {
            DesignReviewStatus.Approved => "The doctor approved the latest submitted design.",
            DesignReviewStatus.Rejected => "The doctor rejected the latest submitted design and returned the case to the laboratory.",
            _ => "The doctor requested changes to the latest submitted design."
        };

        db.CaseHistoryEvents.Add(new CaseHistoryEvent
        {
            DentalCaseId = design.DentalCaseId,
            ActorUserId = doctorId,
            EventType = actionName,
            Description = description,
            PreviousStatus = previousStatus,
            NewStatus = design.DentalCase.Status.ToString(),
            RelatedFileId = design.CaseFileId,
            RelatedDesignVersionId = design.Id,
            Reason = design.ReviewReason,
            CreatedAt = DateTime.UtcNow
        });
        db.Notifications.Add(new Notification
        {
            RecipientUserId = design.SubmittedByUserId,
            DentalCaseId = design.DentalCaseId,
            NotificationType = actionName,
            Message = decision == DesignReviewStatus.Approved
                ? $"Design version {design.VersionNumber} was approved by the doctor."
                : $"Design version {design.VersionNumber} needs lab follow-up. Reason: {design.ReviewReason}",
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        });

        await db.SaveChangesAsync(cancellationToken);
        TempData["ReviewSuccess"] = "The review decision and case history were saved.";
        return RedirectToAction(nameof(Review), new { id = design.Id });
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    /// <summary>Latest design version of every case that belongs to this doctor (translated to SQL, no in-memory grouping).</summary>
    private IQueryable<DesignVersion> LatestDesigns(int doctorId) =>
        db.DesignVersions.AsNoTracking()
            .Where(d => d.DentalCase.CreatedByUserId == doctorId)
            .Where(d => !db.DesignVersions.Any(newer =>
                newer.DentalCaseId == d.DentalCaseId && newer.VersionNumber > d.VersionNumber));

    private Task<List<DesignVersion>> GetLatestDesignsAsync(int doctorId, CancellationToken cancellationToken) =>
        LatestDesigns(doctorId)
            .Include(d => d.CaseFile)
            .Include(d => d.SubmittedByUser)
            .Include(d => d.DentalCase).ThenInclude(c => c.Patient)
            .OrderByDescending(d => d.SubmittedAt)
            .ToListAsync(cancellationToken);

    /// <summary>Server-side pagination: COUNT for the page numbers, then OFFSET/FETCH for just this page.</summary>
    private static async Task<ListPage<TRow>> ToPageAsync<TEntity, TRow>(
        IQueryable<TEntity> filtered,
        IOrderedQueryable<TEntity> ordered,
        ListQueryBase query,
        Expression<Func<TEntity, TRow>> projection,
        CancellationToken cancellationToken)
    {
        var total = await filtered.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)query.PageSize));
        var page = Math.Min(query.Page, totalPages); // e.g. filters shrank the list while on page 5
        var rows = await ordered
            .Skip((page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(projection)
            .ToListAsync(cancellationToken);
        return new ListPage<TRow>(rows, total, page, query.PageSize);
    }

    /// <summary>A date typed by the doctor is a local date; stored values are UTC.</summary>
    private static DateTime ToUtcStart(DateTime localDate) =>
        DateTime.SpecifyKind(localDate.Date, DateTimeKind.Local).ToUniversalTime();

    /// <summary>Accepts "42", "DC-42" or "DC-000042".</summary>
    private static bool TryParseCaseId(string term, out int caseId)
    {
        var text = term.Trim();
        if (text.StartsWith("DC-", StringComparison.OrdinalIgnoreCase)) text = text[3..];
        return int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out caseId) && caseId > 0;
    }

    private static DesignReviewRowViewModel ToReviewRow(DesignVersion d) => new()
    {
        DesignVersionId = d.Id,
        CaseId = d.DentalCaseId,
        PatientName = d.DentalCase.Patient.FullName,
        TreatmentName = d.DentalCase.TreatmentName,
        FileName = d.CaseFile.FileName,
        VersionNumber = d.VersionNumber,
        SubmittedBy = d.SubmittedByUser.FullName,
        SubmittedAt = d.SubmittedAt,
        ReviewStatus = d.ReviewStatus
    };

    private bool TryGetDoctorId(out int doctorId)
    {
        doctorId = 0;
        if (User.Identity?.IsAuthenticated == true)
            return User.IsInRole(nameof(UserRole.Doctor))
                && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out doctorId);

        return int.TryParse(configuration["DoctorWorkspace:DoctorUserId"] ?? "1", out doctorId)
            && doctorId > 0;
    }
}
