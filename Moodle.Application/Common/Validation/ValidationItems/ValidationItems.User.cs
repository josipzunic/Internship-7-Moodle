

using Moodle.Application.Common.Validation;
using Moodle.Application.Enums;
using Moodle.Application.Services;

public static partial class ValidationItems
{
    public static class User
    {
        public static string CodePrefix = nameof(User);

        public static readonly ValidationItem PasswordRequired = new()
        {
            Code = $"{CodePrefix}1",
            Message = "Polje s lozinkom ne smije biti prazno",
            ValidationSeverity = ValidationSeverity.Error,
            ValidationType = ValidationType.FormalValidation
        };
        
        public static readonly ValidationItem MinPasswordLength = new()
        {
            Code = $"{CodePrefix}2",
            Message = $"Lozinka mora sadržavati najmanje {AuthentificationService.MinPasswordLength} znakova",
            ValidationSeverity = ValidationSeverity.Error,
            ValidationType = ValidationType.BusinessRule
        };
        
        public static readonly ValidationItem ContainsSpecialSymbol = new()
        {
            Code = $"{CodePrefix}3",
            Message = "Lozinka mora sadržavati najmanje jedan posebni znak",
            ValidationSeverity = ValidationSeverity.Error,
            ValidationType = ValidationType.BusinessRule
        };
        
        public static readonly ValidationItem ContainsDigit = new()
        {
            Code = $"{CodePrefix}4",
            Message = "Lozinka mora sadržavati najmanje jednu znamenku",
            ValidationSeverity = ValidationSeverity.Error,
            ValidationType = ValidationType.BusinessRule
        };
        
        public static readonly ValidationItem ExistingUser = new()
        {
            Code = $"{CodePrefix}5",
            Message = "Korisnik s danom adresom već postoji",
            ValidationSeverity = ValidationSeverity.Error,
            ValidationType = ValidationType.BusinessRule
        };
        
        public static readonly ValidationItem NonExistingUser = new()
        {
            Code = $"{CodePrefix}6",
            Message = "Korisnik ne postoji",
            ValidationSeverity = ValidationSeverity.Error,
            ValidationType = ValidationType.BusinessRule
        };
        
        public static readonly ValidationItem IncorrectPassword = new()
        {
            Code = $"{CodePrefix}7",
            Message = "Netočno korisničko ime ili lozinka",
            ValidationSeverity = ValidationSeverity.Error,
            ValidationType = ValidationType.BusinessRule
        };

    }
}