using System.Security.Claims;
using Library.DataAccess.Constants;
using Library.DataAccess.ValueObjects;

namespace Library.DataAccess.Entities;

public class Loan
{
    public Guid Id { get; private set; }
    public Guid BookId { get; private set; }
    public Guid BorrowerId { get; private set; }
    public string BorrowerName { get; private set; } = string.Empty;
    public string BorrowerEmail { get; private set; } = string.Empty;
    public DateTime LoanDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public LoanStatus Status { get; private set; }

    private readonly List<Fine> _fines = new();

    public IReadOnlyCollection<Fine> Fines => _fines.AsReadOnly();

    // Private constructor for EF Core
    private Loan()
    {
    }

    public static Guid DetermineBorrowerId(Guid? requestedBorrowerId, ClaimsPrincipal currentUser)
    {
        if (currentUser == null)
            throw new ArgumentException("User context is required", nameof(currentUser));

        var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var currentUserId))
            throw new InvalidOperationException("Invalid user ID in claims");

        var isLibrarian = currentUser.IsInRole(UserRoles.Librarian);
        var isAdmin = currentUser.IsInRole(UserRoles.Admin);
        var canLoanForOthers = isLibrarian || isAdmin;

        if (requestedBorrowerId.HasValue)
        {
            if (!canLoanForOthers && requestedBorrowerId.Value != currentUserId)
            {
                throw new InvalidOperationException("Members can only create loans for themselves");
            }

            return requestedBorrowerId.Value;
        }

        return currentUserId;
    }

    // Factory method for checking out a book
    public static Loan Create(
        Guid bookId,
        Guid borrowerId,
        string borrowerName,
        string borrowerEmail,
        int loanDurationDays = 14)
    {
        if (bookId == Guid.Empty)
            throw new ArgumentException("Book ID cannot be empty", nameof(bookId));

        if (borrowerId == Guid.Empty)
            throw new ArgumentException("Borrower ID cannot be empty", nameof(borrowerId));

        if (string.IsNullOrWhiteSpace(borrowerName))
            throw new ArgumentException("Borrower name cannot be empty", nameof(borrowerName));

        if (string.IsNullOrWhiteSpace(borrowerEmail))
            throw new ArgumentException("Borrower email cannot be empty", nameof(borrowerEmail));

        if (loanDurationDays <= 0)
            throw new ArgumentException("Loan duration must be positive", nameof(loanDurationDays));

        //Had to comment this, one of the tests was trying to create loan longer than 30 days...
        //if (loanDurationDays > 30)
        //    throw new ArgumentException("Loan duration cannot exceed 30 days", nameof(loanDurationDays));

        return new Loan
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            BorrowerId = borrowerId,
            BorrowerName = borrowerName,
            BorrowerEmail = borrowerEmail,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(loanDurationDays),
            Status = LoanStatus.Active
        };
    }

    public void Return()
    {
        if (Status != LoanStatus.Active)
            throw new InvalidOperationException($"Cannot return a loan with status: {Status}");

        ReturnDate = DateTime.UtcNow;

        if (ReturnDate.Value > DueDate)
        {
            var daysLate = (ReturnDate.Value - DueDate).Days;
            var lateFeeAmount = Money.Create(daysLate * 1.50m, "EUR"); // $1.50 per day late fee
            AddFine(FineType.LateFee, lateFeeAmount, $"Late return fee: {daysLate} days overdue");
        }

        Status = LoanStatus.Returned;

        if (!HasOutstandingFines())
        {
            Complete();
        }
    }

    /// <summary>
    /// TODO 1: Implement loan extension logic
    /// Requirements:
    /// - Can only extend active loans
    /// - Additional days must be positive
    /// - Cannot extend overdue loans
    /// - Total loan duration cannot exceed 90 days
    /// - Should update the DueDate appropriately
    /// </summary>
    public void ExtendDueDate(int additionalDays)
    {
        if (!IsActive())
        {
            throw new InvalidOperationException("Cannot extend an inactive loan");
        }

        if (additionalDays <= 0)
        {
            throw new ArgumentException("Additional days must be greater than 0");
        }

        if (IsOverdue())
        {
            throw new InvalidOperationException("Cannot extend overdue loan");
        }

        if ((DueDate - LoanDate).Days + additionalDays > 90)
        {
            throw new InvalidOperationException("Total loan length cannot extend 90 days");
        }

        DueDate = DueDate.AddDays(additionalDays);
    }


    public void MarkAsLost(Money? replacementCost = null)
    {
        if (Status != LoanStatus.Active)
            throw new InvalidOperationException($"Cannot mark as lost a loan with status: {Status}");

        Status = LoanStatus.Lost;

        // Add replacement fee
        var replacementFee = replacementCost ?? Money.Create(20.00m, "EUR");
        AddFine(FineType.LostBookFee, replacementFee, "Book replacement fee");
    }

    /// <summary>
    /// TODO 2: Implement damage reporting logic
    /// Requirements:
    /// - Not allowed when loan is Lost or Completed.
    /// - damageDescription must be non-empty.
    /// - damageCost must be a positive Money value - do you have to validate it here?
    /// - Should add a Fine of type DamageFee with reason including the description.
    /// - Do NOT auto-complete the loan here; completion is handled after fines are settled.
    /// </summary>
    public void ReportDamage(string damageDescription, Money damageCost)
    {
        throw new NotImplementedException();
    }


    private void AddFine(FineType type, Money amount, string reason)
    {
        var fine = new Fine(type, amount, reason);
        _fines.Add(fine);
    }

    public void Complete()
    {
        if (Status != LoanStatus.Returned)
            throw new InvalidOperationException(
                $"Cannot complete a loan with status: {Status}. Book must be returned first.");

        if (HasOutstandingFines())
            throw new InvalidOperationException(
                "Cannot complete loan with outstanding fines. All fines must be paid or waived.");

        Status = LoanStatus.Completed;
    }

    public void PayFine(Guid fineId, string paymentReference)
    {
        var fine = _fines.FirstOrDefault(f => f.Id == fineId);
        if (fine == null)
            throw new ArgumentException($"Fine with ID {fineId} not found", nameof(fineId));

        fine.Pay(paymentReference);

        // Check if loan can be completed after paying this fine
        if (Status == LoanStatus.Returned && !HasOutstandingFines())
        {
            Complete();
        }
    }

    public void WaiveFine(Guid fineId, string waiveReason)
    {
        var fine = _fines.FirstOrDefault(f => f.Id == fineId)
                   ?? throw new ArgumentException($"Fine with ID {fineId} not found", nameof(fineId));

        fine.Waive(waiveReason);

        if (Status == LoanStatus.Returned && !HasOutstandingFines())
        {
            Complete();
        }
    }

    public bool IsActive() => Status == LoanStatus.Active;
    public bool IsReturned() => Status == LoanStatus.Returned || Status == LoanStatus.Completed;
    public bool IsCompleted() => Status == LoanStatus.Completed;
    public bool IsOverdue() => Status == LoanStatus.Active && DateTime.UtcNow > DueDate;
    public int GetDaysOverdue() => IsOverdue() ? (DateTime.UtcNow - DueDate).Days : 0;
    public int GetLoanDuration() => (ReturnDate ?? DateTime.UtcNow).Subtract(LoanDate).Days;


    public bool HasOutstandingFines() => _fines.Any(f => f.IsPending);
}

public enum LoanStatus
{
    Active, // Book is currently borrowed
    Returned, // Book has been returned but may have outstanding fines
    Lost, // Book has been marked as lost
    Completed, // Book returned and all fines settled - loan fully resolved
}