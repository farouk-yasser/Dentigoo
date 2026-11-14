using System.ComponentModel.DataAnnotations;
using Dentigo.Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dentigo.Presentation.Models;

// NOTE: [BindNever] + [ValidateNever] on the "output" properties (Results, option lists, display names)
// means a user can't fill them from the query string and they are not validated.

public sealed class ReviewListViewModel : ListQueryBase
{
    [RegularExpression("^(submitted|case|patient|treatment|version)$", ErrorMessage = "Unknown sort column.")]
    public string Sort { get; set; } = "submitted";

    // null = "All statuses". Default = designs waiting for the doctor.
    [EnumDataType(typeof(DesignReviewStatus), ErrorMessage = "Unknown review status.")]
    [Display(Name = "Status")]
    public DesignReviewStatus? Status { get; set; } = DesignReviewStatus.PendingReview;

    [StringLength(100, ErrorMessage = "Treatment can't be longer than {1} characters.")]
    public string? Treatment { get; set; }

    [BindNever, ValidateNever]
    public IReadOnlyList<string> Treatments { get; set; } = [];

    [BindNever, ValidateNever]
    public ListPage<DesignReviewRowViewModel> Results { get; set; } = ListPage<DesignReviewRowViewModel>.Empty;
}

public sealed class CaseListViewModel : ListQueryBase
{
    [RegularExpression("^(updated|case|patient|treatment|status)$", ErrorMessage = "Unknown sort column.")]
    public string Sort { get; set; } = "updated";

    [EnumDataType(typeof(DentalCaseStatus), ErrorMessage = "Unknown case status.")]
    [Display(Name = "Status")]
    public DentalCaseStatus? Status { get; set; }

    [BindNever, ValidateNever]
    public ListPage<DoctorCaseRowViewModel> Results { get; set; } = ListPage<DoctorCaseRowViewModel>.Empty;
}

public sealed class NotificationListViewModel : ListQueryBase
{
    [RegularExpression("^(created|type)$", ErrorMessage = "Unknown sort column.")]
    public string Sort { get; set; } = "created";

    [RegularExpression("^(all|unread|read)$", ErrorMessage = "Filter must be all, unread or read.")]
    public string Filter { get; set; } = "all";

    [StringLength(60, ErrorMessage = "Type can't be longer than {1} characters.")]
    public string? Type { get; set; }

    [BindNever, ValidateNever]
    public int UnreadCount { get; set; }

    [BindNever, ValidateNever]
    public IReadOnlyList<string> Types { get; set; } = [];

    [BindNever, ValidateNever]
    public ListPage<DoctorNotificationRowViewModel> Results { get; set; } = ListPage<DoctorNotificationRowViewModel>.Empty;
}

public sealed class CaseHistoryViewModel : ListQueryBase
{
    [Range(1, int.MaxValue, ErrorMessage = "A valid case id is required.")]
    public int CaseId { get; set; }

    [StringLength(60, ErrorMessage = "Event type can't be longer than {1} characters.")]
    [Display(Name = "Event type")]
    public string? EventType { get; set; }

    [BindNever, ValidateNever]
    public string PatientName { get; set; } = "";

    [BindNever, ValidateNever]
    public string TreatmentName { get; set; } = "";

    [BindNever, ValidateNever]
    public IReadOnlyList<string> EventTypes { get; set; } = [];

    [BindNever, ValidateNever]
    public ListPage<CaseHistoryRowViewModel> Results { get; set; } = ListPage<CaseHistoryRowViewModel>.Empty;
}
