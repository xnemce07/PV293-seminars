using Library.Application.CQRS;
using Library.Application.Repositories;
using Library.Domain.ValueObjects;
using MediatR;

namespace Library.Application.Loans.Commands
{
    public class ReportDamageCommand : ICommand<Guid>
    {
        public Guid LoanId { get; set; }
        public required string DamageDescription { get; set; }
        public required Money DamageCost { get; set; }
    }

    public class ReportDamageCommandHandler(ILoanRepository loanRepository) : IRequestHandler<ReportDamageCommand, Guid>
    {
        async Task<Guid> IRequestHandler<ReportDamageCommand, Guid>.Handle(ReportDamageCommand request, CancellationToken cancellationToken)
        {
            var loan = await loanRepository.GetByIdAsync(request.LoanId);
            loan?.ReportDamage(request.DamageDescription, request.DamageCost);

            return loan?.Id ?? Guid.Empty;
        }
    }
}
