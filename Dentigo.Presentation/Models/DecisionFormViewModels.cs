using System.ComponentModel.DataAnnotations;

namespace Dentigo.Presentation.Models;

/// <summary>
/// Base for the three decision forms on the review page. The id is validated on the server and,
/// because it is rendered with asp-for, also carries data-val-* attributes for the client.
/// </summary>
public abstract class DesignDecisionBase
{
    [Range(1, int.MaxValue, ErrorMessage = "A valid design version is required.")]
    [Display(Name = "Design version")]
    public int DesignVersionId { get; set; }
}

public sealed class ApproveDesignViewModel : DesignDecisionBase
{
}

public sealed class RequestRevisionViewModel : DesignDecisionBase
{
    private string? _reason;

    [Required(ErrorMessage = "Please describe the changes the laboratory should make.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "The revision reason must be between {2} and {1} characters.")]
    [Display(Name = "Revision reason")]
    public string? Reason
    {
        get => _reason;
        // Trim first, so "     " counts as empty and padding doesn't count towards the minimum length.
        set => _reason = value?.Trim();
    }
}

public sealed class RejectDesignViewModel : DesignDecisionBase
{
    private string? _reason;

    [Required(ErrorMessage = "Please explain why this design is unsuitable.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "The rejection reason must be between {2} and {1} characters.")]
    [Display(Name = "Rejection reason")]
    public string? Reason
    {
        get => _reason;
        set => _reason = value?.Trim();
    }
}
