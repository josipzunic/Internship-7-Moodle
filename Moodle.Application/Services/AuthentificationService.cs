using Moodle.Application.Common.Model;
using Moodle.Application.Common.Validation;
using Moodle.Application.Enums;
using Moodle.Application.Interfaces;
using Moodle.Domain.Entities;
using Moodle.Domain.Enums;

namespace Moodle.Application.Services;

public class AuthentificationService
{
    private readonly IUserRepository _userRepository;
    public const int MinPasswordLength = 8;
    
    public AuthentificationService(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<Result<bool>> RegisterUserAsync(string email, string password)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(email);
        var validationResult = new Result<bool>();
        var validationItem = new ValidationItem();

        if (existingUser != null)
        {
            RetrieveValidationData(validationItem, ValidationItems.User.ExistingUser);
            SetValidationResult(validationResult, validationItem, false);
        }

        var user = new User
        {
            Email = email,
            Role = Role.Student,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var validatePassword = ValidatePassword(password);

        if (!validatePassword.Value)
        {
            return validatePassword;
        }
        
        user.SetPasswordHash(password);
        await _userRepository.AddUserAsync(user);

        return new Result<bool>
        {
            ValidationSeverity = ValidationSeverity.Info,
            ValidationType = ValidationType.BusinessRule,
            ValidationMessage = "Uspješno ste registrirani",
            Value = true
        };
    }

    public async Task<Result<User>> LoginUserAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        var validationResult = new Result<User>();
        var  validationItem = new ValidationItem();

        if (user == null)
        {
            RetrieveValidationData(validationItem, ValidationItems.User.NonExistingUser);
            SetValidationResult<User>(validationResult, validationItem, null);
            return validationResult;
        }

        if (!user.VerifyPassword(password))
        {
            RetrieveValidationData(validationItem, ValidationItems.User.IncorrectPassword);
            SetValidationResult(validationResult, validationItem, null);
            return validationResult;
        }
        
        return new Result<User>
        {
            ValidationSeverity = ValidationSeverity.Info,
            ValidationType = ValidationType.BusinessRule,
            ValidationMessage = "Login uspješan",
            Value = user
        };
    }
    
    public Result<bool> ValidatePassword(string password)
    {
        List<char> specialSymbols = new List<char>
        {
            '!', '@', '#', '$', '%', '^', '&', '*', '(', ')',
            '-', '_', '+', '=', '{', '}', '[', ']', '|', '\\',
            ':', ';', '"', '\'', '<', '>', ',', '.', '?', '/'
        };
        List<char> digits = new List<char>
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        var validationResult = new Result<bool>();
        var validationItem = new ValidationItem();
        
        if (string.IsNullOrEmpty(password))
        {
            RetrieveValidationData(validationItem, ValidationItems.User.PasswordRequired);
            SetValidationResult(validationResult, validationItem, false);
            return validationResult;
        }

        if (password.Length < MinPasswordLength)
        {
            RetrieveValidationData(validationItem, ValidationItems.User.MinPasswordLength);
            SetValidationResult(validationResult, validationItem, false);
            return validationResult;
        }

        if (!password.Any(s => specialSymbols.Contains(s)))
        {
            RetrieveValidationData(validationItem, ValidationItems.User.ContainsSpecialSymbol);
            SetValidationResult(validationResult, validationItem, false);
            return validationResult;
        }

        if (!password.Any(d => digits.Contains(d)))
        {
            RetrieveValidationData(validationItem, ValidationItems.User.ContainsDigit);
            SetValidationResult(validationResult, validationItem, false);
            return validationResult;
        }

        return new Result<bool>
        {
            ValidationSeverity = ValidationSeverity.Info,
            ValidationType = ValidationType.BusinessRule,
            ValidationMessage = "Lozinka uspješno postavljena",
            Value = true
        };
    }

    public void RetrieveValidationData(ValidationItem validationItem, ValidationItem sourceItem)
    {
        validationItem.Message = sourceItem.Message;
        validationItem.ValidationType = sourceItem.ValidationType;
        validationItem.ValidationSeverity = sourceItem.ValidationSeverity;
        validationItem.Code = sourceItem.Code;
    }

    public void SetValidationResult<T>(Result<T> validationResult, ValidationItem validationItem, T? result)
    {
        validationResult.ValidationSeverity = validationItem.ValidationSeverity;
        validationResult.ValidationType = validationItem.ValidationType;
        validationResult.ValidationMessage = validationItem.Message;
        validationResult.ErrorCode = validationItem.Code;
        validationResult.Value = result;
    }
}