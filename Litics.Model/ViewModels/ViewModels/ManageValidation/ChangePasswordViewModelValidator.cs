using FluentValidation;
using Litics.Model.ViewModels.ManageViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litics.Model.ViewModels.Validation.ManageValidation
{
    public class ChangePasswordViewModelValidator : AbstractValidator<ChangePasswordViewModel>
    {
        public ChangePasswordViewModelValidator()
        {
            RuleFor(vm => vm.OldPassword).NotNull().Length(6, 100).WithMessage("Old Password cannot be empty");
            RuleFor(vm => vm.NewPassword).NotNull().Length(6, 100).WithMessage("New Password cannot be empty");
            RuleFor(vm => vm.ConfirmPassword).Equal(x => x.NewPassword).WithMessage("The new password and confirmation password do not match.");
        }
    }
}
