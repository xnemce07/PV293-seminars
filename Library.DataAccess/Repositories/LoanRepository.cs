using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class LoanRepository : Repository<Loan>, ILoanRepository
{
    public LoanRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Loan?> GetActiveLoanByBookIdAsync(Guid bookId)
    {
        return await Entities
            .FirstOrDefaultAsync(l => l.BookId == bookId && l.Status == LoanStatus.Active);
    }

    public async Task<bool> HasActiveLoanAsync(Guid borrowerId, Guid bookId)
    {
        return await Entities
            .AnyAsync(l => l.BorrowerId == borrowerId && l.BookId == bookId && l.Status == LoanStatus.Active);
    }

    private ApplicationDbContext ApplicationDbContext => (ApplicationDbContext)Context;
}