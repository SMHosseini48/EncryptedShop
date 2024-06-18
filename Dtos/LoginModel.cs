using FluentValidation;
using FluentValidation.Results;

namespace MultiLevelEncryptedEshop.Dtos;

public class LoginModel
{
    public ValidationResult Validate() => new LoginAccountModelValidator().Validate(this); 

    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginAccountModelValidator : AbstractValidator<LoginModel>
{
    public LoginAccountModelValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty()
            .Matches("^[a-zA-Z0-9.!#$%&'*+=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$");
        RuleFor(x => x.Password).NotEmpty().NotNull();
    }
}