using Moodle.Application.Common.Model;
using Moodle.Application.Common.Validation;
using Moodle.Application.Enums;
using Moodle.Application.Interfaces;
using Moodle.Domain.Entities;
using Moodle.Domain.Enums;

public class AuthentificationService
{
    private readonly IUserRepository _userRepository;
    public const int MinPasswordLength = 8;

    public AuthentificationService(IUserRepository userRepository)
        => _userRepository = userRepository;
    
    public async Task<Result<bool>> RegisterUserAsync(
        string email,
        string password,
        string confirmPassword,
        string captchaInput,
        string captchaExpected)
    {
        var validationResult = new Result<bool>();
        var validationItem = new ValidationItem();
        
        if (captchaInput != captchaExpected)
        {
            RetrieveValidationData(validationItem, ValidationItems.User.InvalidCaptcha);
            SetValidationResult(validationResult, validationItem, false);
            return validationResult;
        }
        
        if (!ValidateEmailFormat(email))
        {
            RetrieveValidationData(validationItem, ValidationItems.User.InvalidEmailFormat);
            SetValidationResult(validationResult, validationItem, false);
            return validationResult;
        }
        
        var existingUser = await _userRepository.GetUserByEmailAsync(email);
        if (existingUser != null)
        {
            RetrieveValidationData(validationItem, ValidationItems.User.ExistingUser);
            SetValidationResult(validationResult, validationItem, false);
            return validationResult;
        }
        
        if (password != confirmPassword)
        {
            RetrieveValidationData(validationItem, ValidationItems.User.PasswordsDoNotMatch);
            SetValidationResult(validationResult, validationItem, false);
            return validationResult;
        }
        
        var validatePassword = ValidatePassword(password);
        if (!validatePassword.Value)
            return validatePassword;

        var user = new User
        {
            Email = email,
            Role = Role.Student,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

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
        var validationItem = new ValidationItem();

        if (user == null)
        {
            RetrieveValidationData(validationItem, ValidationItems.User.NonExistingUser);
            SetValidationResult(validationResult, validationItem, null);
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

    
    private bool ValidateEmailFormat(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var parts = email.Split('@');
        if (parts.Length != 2)
            return false;

        if (parts[0].Length < 1)
            return false;

        var domainParts = parts[1].Split('.');
        if (domainParts.Length != 2)
            return false;

        if (domainParts[0].Length < 2)
            return false;

        if (domainParts[1].Length < 3)
            return false;

        return true;
    }

    
    public string GenerateCaptcha(int length = 6)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string all = letters + digits;

        var random = new Random();
        var chars = new List<char>
        {
            letters[random.Next(letters.Length)],
            digits[random.Next(digits.Length)]
        };

        for (int i = 2; i < length; i++)
            chars.Add(all[random.Next(all.Length)]);

        return new string(chars.OrderBy(_ => random.Next()).ToArray());
    }

    
    public Result<bool> ValidatePassword(string password)
    {
        List<char> specialSymbols = new()
        {
            '!', '@', '#', '$', '%', '^', '&', '*', '(', ')',
            '-', '_', '+', '=', '{', '}', '[', ']', '|', '\\',
            ':', ';', '"', '\'', '<', '>', ',', '.', '?', '/'
        };

        List<char> digits = new()
        {
            '0','1','2','3','4','5','6','7','8','9'
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
