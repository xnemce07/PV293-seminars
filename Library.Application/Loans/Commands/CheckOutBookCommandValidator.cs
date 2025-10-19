using FluentValidation;

namespace Library.Application.Loans.Commands;

public class CheckOutBookCommandValidator : AbstractValidator<CheckOutBookCommand>
{
    public CheckOutBookCommandValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Book ID is required");

        // BorrowerId is optional - will be determined based on user context
        RuleFor(x => x.BorrowerId)
            .NotEmpty().WithMessage("Borrower ID is required")
            .When(x => x.BorrowerId.HasValue);

        RuleFor(x => x.LoanDurationDays)
            .GreaterThan(0).WithMessage("Loan duration must be positive")
            .LessThanOrEqualTo(90).WithMessage("Loan duration cannot exceed 90 days");

        RuleFor(x => x.User)
            .NotNull().WithMessage("User context is required");
    }
}