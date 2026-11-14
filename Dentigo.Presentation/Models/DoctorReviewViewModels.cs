using Dentigo.Domain.Enums;

namespace Dentigo.Presentation.Models;

public sealed class DoctorDashboardViewModel
{
    public int ActiveCases { get; init; }
    public int DoctorReview { get; init; }
    public int RevisionRequested { get; init; }
    public int CompletedCases { get; init; }
    public IReadOnlyList<DoctorCaseRowViewModel> RecentCases { get; init; } = [];
    public IReadOnlyList<DesignReviewRowViewModel> PendingReviews { get; init; } = [];
}

public sealed class DoctorCaseRowViewModel
{
    public int CaseId { get; init; }
    public string PatientName { get; init; } = "";
    public string TreatmentName { get; init; } = "";
    public DentalCaseStatus Status { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed class DesignReviewRowViewModel
{
    public int DesignVersionId { get; init; }
    public int CaseId { get; init; }
    public string PatientName { get; init; } = "";
    public string TreatmentName { get; init; } = "";
    public string FileName { get; init; } = "";
    public int VersionNumber { get; init; }
    public string SubmittedBy { get; init; } = "";
    public DateTime SubmittedAt { get; init; }
    public DesignReviewStatus ReviewStatus { get; init; }
}

public sealed class DesignReviewDetailViewModel
{
    public int DesignVersionId { get; init; }
    public int CaseId { get; init; }
    public string PatientName { get; init; } = "";
    public string TreatmentName { get; init; } = "";
    public string? DoctorInstructions { get; init; }
    public string FileName { get; init; } = "";
    public string FileType { get; init; } = "";
    public long FileSize { get; init; }
    public string SubmittedBy { get; init; } = "";
    public DateTime SubmittedAt { get; init; }
    public int VersionNumber { get; init; }
    public DesignReviewStatus ReviewStatus { get; init; }
    public string? ReviewReason { get; init; }
    public bool IsLatestVersion { get; init; }
    public IReadOnlyList<DesignReviewRowViewModel> PreviousVersions { get; init; } = [];

    // Forms posted from the review page. Each one has its own DataAnnotations rules (see DecisionFormViewModels.cs).
    public ApproveDesignViewModel ApproveForm { get; set; } = new();
    public RequestRevisionViewModel RevisionForm { get; set; } = new();
    public RejectDesignViewModel RejectForm { get; set; } = new();
}

public sealed class CaseHistoryRowViewModel
{
    public string EventType { get; init; } = "";
    public string Description { get; init; } = "";
    public string? ActorName { get; init; }
    public string? PreviousStatus { get; init; }
    public string? NewStatus { get; init; }
    public string? Reason { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed class DoctorNotificationRowViewModel
{
    public int NotificationId { get; init; }
    public int? CaseId { get; init; }
    public string NotificationType { get; init; } = "";
    public string Message { get; init; } = "";
    public DateTime CreatedAt { get; init; }
    public bool IsRead { get; init; }
}
