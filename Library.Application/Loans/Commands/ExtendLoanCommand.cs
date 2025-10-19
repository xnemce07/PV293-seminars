using Library.Application.CQRS;
using Library.Application.Repositories;
using MediatR;

namespace Library.Application.Loans.Commands
{
    public class ExtendLoanCommand : ICommand<Guid>
    {
        public Guid LoanId { get; set; }
        public int ExtendDurationDays { get; set; }
    }

    public class ExtendLoanHandler(
        ILoanRepository loanRepository) : IRequestHandler<ExtendLoanCommand, Guid>
    {
        public async Task<Guid> Handle(ExtendLoanCommand request, CancellationToken cancellationToken)
        {
            var loan = await loanRepository.GetByIdAsync(request.LoanId);

            loan?.ExtendDueDate(request.ExtendDurationDays);

            return loan?.Id ?? Guid.Empty;
        }
    }
}
