using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Dentigo.Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dentigo.Presentation.Models;

/// <summary>One page of rows plus the numbers the infinite-scroll script needs.</summary>
public sealed record ListPage<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public static ListPage<T> Empty { get; } = new([], 0, 1, 12);
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    public PagerViewModel ToPager() => new(Page, TotalPages, TotalCount, Items.Count, PageSize);
}

/// <summary>What the pager partial needs: where we are and how many pages exist.</summary>
public sealed record PagerViewModel(int Page, int TotalPages, int TotalCount, int Shown, int PageSize)
{
    public int FirstItem => Shown == 0 ? 0 : (Page - 1) * PageSize + 1;
    public int LastItem => Shown == 0 ? 0 : FirstItem + Shown - 1;
}

/// <summary>Everything a sortable column header needs to render itself.</summary>
public sealed record SortHeaderViewModel(string Key, string Label, string CurrentSort, string CurrentDir, string DefaultDir = "asc");

/// <summary>
/// Search / date filter / paging values shared by every list page. These are bound from the query string
/// (GET), validated with DataAnnotations on the server and - because the filter form uses asp-for -
/// validated again in the browser by jquery-validation-unobtrusive.
/// </summary>
public abstract class ListQueryBase
{
    private string? _search;

    [StringLength(100, ErrorMessage = "Search text can't be longer than {1} characters.")]
    [Display(Name = "Search")]
    public string? Search
    {
        get => _search;
        set => _search = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    [DataType(DataType.Date)]
    [Display(Name = "From date")]
    public DateTime? From { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "To date")]
    [DateOnOrAfter(nameof(From), ErrorMessage = "The end date must be on or after the start date.")]
    public DateTime? To { get; set; }

    [RegularExpression("^(asc|desc)$", ErrorMessage = "Sort direction must be 'asc' or 'desc'.")]
    public string Dir { get; set; } = "desc";

    [Range(1, 10000, ErrorMessage = "Page must be between {1} and {2}.")]
    public int Page { get; set; } = 1;

    [Range(5, 50, ErrorMessage = "Page size must be between {1} and {2}.")]
    public int PageSize { get; set; } = 12;
}

public static class QueryExtensions
{
    public static IOrderedQueryable<T> OrderByDirection<T, TKey>(
        this IQueryable<T> source, Expression<Func<T, TKey>> key, bool ascending) =>
        ascending ? source.OrderBy(key) : source.OrderByDescending(key);
}

public static class DisplayExtensions
{
    public static string ToLabel(this DentalCaseStatus status) => status switch
    {
        DentalCaseStatus.DesignSubmitted => "Doctor Review",
        DentalCaseStatus.InProgress => "Lab Processing",
        _ => status.ToString()
    };

    public static string ToLabel(this DesignReviewStatus status) => status switch
    {
        DesignReviewStatus.PendingReview => "Pending review",
        DesignReviewStatus.RevisionRequested => "Revision requested",
        _ => status.ToString()
    };
}
