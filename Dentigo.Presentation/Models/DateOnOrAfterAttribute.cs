using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dentigo.Presentation.Models;

/// <summary>
/// Custom DataAnnotations rule: this date must be on or after another date property of the same model.
/// Server side: IsValid. Client side: IClientModelValidator emits data-val-dateonorafter, and
/// wwwroot/js/dentigo-validation.js registers the matching jquery-validation adapter.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DateOnOrAfterAttribute : ValidationAttribute, IClientModelValidator
{
    public DateOnOrAfterAttribute(string otherProperty)
    {
        OtherProperty = otherProperty;
        ErrorMessage = "{0} must be on or after the start date.";
    }

    public string OtherProperty { get; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime end) return ValidationResult.Success;

        var other = validationContext.ObjectType.GetProperty(OtherProperty);
        if (other?.GetValue(validationContext.ObjectInstance) is not DateTime start) return ValidationResult.Success;

        return end.Date >= start.Date
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-dateonorafter", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
        MergeAttribute(context.Attributes, "data-val-dateonorafter-other", OtherProperty);
    }

    private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key)) attributes.Add(key, value);
    }
}
