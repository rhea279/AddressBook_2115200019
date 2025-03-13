using FluentValidation;
using ModelLayer.DTO;

namespace BusinessLayer.Validator
{
    public class AddressBookValidator:AbstractValidator<AddressBookDTO>
    {
        public AddressBookValidator() { 
            RuleFor(a => a.Name).NotEmpty().WithMessage("Nameis required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(a => a.PhoneNumber)
                .Matches(@"^\d{10}$").WithMessage("Phone number must be exactly 10 digits.");

            RuleFor(a => a.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .When(a => !string.IsNullOrEmpty(a.Email)); // Only validate if not null

            RuleFor(a => a.Address)
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.");
        }
    }
}
