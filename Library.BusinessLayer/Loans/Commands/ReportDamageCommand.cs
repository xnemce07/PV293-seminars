using Library.BusinessLayer.CQRS;
using Library.DataAccess.Repositories;
using Library.DataAccess.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.BusinessLayer.Loans.Commands
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
