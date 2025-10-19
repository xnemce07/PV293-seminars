using Library.BusinessLayer.CQRS;
using Library.DataAccess.Entities;
using Library.DataAccess.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Library.BusinessLayer.Loans.Commands
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

            loan.ExtendDueDate(request.ExtendDurationDays);

            return loan.Id;
        }
    }
}
