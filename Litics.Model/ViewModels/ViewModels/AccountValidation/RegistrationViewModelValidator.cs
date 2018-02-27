using FluentValidation;
using Litics.Model.ViewModels.AccountViewModels;

namespace Litics.Model.ViewModels.AccountValidation
{
    public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
    {
        public RegistrationViewModelValidator()
        {
            RuleFor(vm => vm.AccountName).NotEmpty().WithMessage("Account cannot be empty");
            RuleFor(vm => vm.Email).NotEmpty().WithMessage("Email cannot be empty");
            RuleFor(vm => vm.Password).NotNull().Length(6, 100).WithMessage("Password cannot be empty");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match");
        }
    }
}
