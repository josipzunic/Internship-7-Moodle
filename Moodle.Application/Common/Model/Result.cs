using Moodle.Application.Enums;

namespace Moodle.Application.Common.Model;

public class Result<TValue>
{
    public TValue? Value { get;  set; }
    public ValidationSeverity ValidationSeverity { get; set; }
    public ValidationType ValidationType { get; set; }
    public string ValidationMessage { get; set; } = null!;
    public string ErrorCode { get; set; } = null!;
}