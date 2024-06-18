using FluentValidation;
using FluentValidation.Results;

namespace MultiLevelEncryptedEshop.Dtos;

public class RegisterAccountModel
{
    public ValidationResult Validate() => new RegisterAccountModelValidator().Validate(this); 
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string UserName { get; set; }
}

public class RegisterAccountModelValidator : AbstractValidator<RegisterAccountModel>
{
    public RegisterAccountModelValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty()
            .Matches("^[a-zA-Z0-9.!#$%&'*+=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$");
        RuleFor(x => x.ConfirmPassword).NotEmpty().NotNull().Equal(x => x.Password);
    }
}