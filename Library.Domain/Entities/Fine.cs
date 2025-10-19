using Library.Domain.ValueObjects;

namespace Library.Domain.Entities;

public class Fine
{
    public Guid Id { get; private set; }
    public FineType Type { get; private set; }
    public Money Amount { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTime IssuedDate { get; private set; }
    public DateTime? PaidDate { get; private set; }
    public FineStatus Status { get; private set; }
    public string? PaymentReference { get; private set; }

    public bool IsPaid => Status == FineStatus.Paid;
    public bool IsPending => Status == FineStatus.Pending;
    public int GetDaysOverdue() => Status == FineStatus.Pending ? (DateTime.UtcNow - IssuedDate).Days : 0;

    // Private constructor for EF Core
    private Fine()
    {
        Amount = Money.Create(0, "EUR"); // EF Core needs this
    }

    // Internal constructor - only Loan aggregate can create fines
    internal Fine(FineType type, Money amount, string reason)
    {
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason cannot be empty", nameof(reason));

        Id = Guid.NewGuid();
        Type = type;
        Amount = amount;
        Reason = reason;
        IssuedDate = DateTime.UtcNow;
        Status = FineStatus.Pending;
    }

    internal void Pay(string paymentReference)
    {
        if (Status == FineStatus.Paid)
            throw new InvalidOperationException("Fine is already paid");

        if (Status == FineStatus.Waived)
            throw new InvalidOperationException("Cannot pay a waived fine");

        if (string.IsNullOrWhiteSpace(paymentReference))
            throw new ArgumentException("Payment reference cannot be empty", nameof(paymentReference));

        PaidDate = DateTime.UtcNow;
        PaymentReference = paymentReference;
        Status = FineStatus.Paid;
    }

    internal void Waive(string reason)
    {
        if (Status == FineStatus.Paid)
            throw new InvalidOperationException("Cannot waive a paid fine");

        if (Status == FineStatus.Waived)
            throw new InvalidOperationException("Fine is already waived");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Waive reason cannot be empty", nameof(reason));

        Reason = $"{Reason} - WAIVED: {reason}";
        Status = FineStatus.Waived;
    }
}

public enum FineType
{
    LateFee,
    DamageFee,
    LostBookFee,
}

public enum FineStatus
{
    Pending,
    Paid,
    Waived,
}