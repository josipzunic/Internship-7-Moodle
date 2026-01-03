using Moodle.Application.Enums;

namespace Moodle.Application.Common.Validation;

public class ValidationItem
{
    public ValidationSeverity ValidationSeverity { get; set; }
    public ValidationType ValidationType { get; set; }
    public string Code { get; set; } = null!;
    public string Message { get; set; } = null!;
}