using System.Security.Claims;
using Library.Application.CQRS;
using Library.Application.Repositories;
using Library.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Library.Application.Loans.Commands;

public class CheckOutBookCommand : ICommand<Guid>
{
    public Guid BookId { get; set; }
    public Guid? BorrowerId { get; set; } // Optional - if not provided, use current user
    public int LoanDurationDays { get; set; } = 14;
    public ClaimsPrincipal? User { get; set; } // User context for authorization
}

public class CheckOutBookCommandHandler(
    ILoanRepository loanRepository,
    IBookRepository bookRepository,
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<CheckOutBookCommand, Guid>
{
    public async Task<Guid> Handle(CheckOutBookCommand command, CancellationToken cancellationToken)
    {
        if (command.User == null)
            throw new InvalidOperationException("User context is required");

        var book = await bookRepository.GetByIdAsync(command.BookId);
        if (book == null)
            throw new InvalidOperationException($"Book with ID {command.BookId} not found");

        var existingLoan = await loanRepository.GetActiveLoanByBookIdAsync(command.BookId);
        if (existingLoan != null)
            throw new InvalidOperationException($"Book is already checked out");

        var actualBorrowerId = Loan.DetermineBorrowerId(command.BorrowerId, command.User);

        var borrower = await userManager.FindByIdAsync(actualBorrowerId.ToString())
            ?? throw new InvalidOperationException($"User with ID {actualBorrowerId} not found");

        var loan = Loan.Create(
            command.BookId,
            actualBorrowerId,
            borrower.FullName,
            borrower.Email ?? string.Empty,
            command.LoanDurationDays
        );

        loanRepository.Add(loan);

        return loan.Id;
    }
}