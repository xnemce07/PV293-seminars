using Library.Domain.Constants;
using Library.Domain.Entities;
using Library.Domain.ValueObjects;
using System.Reflection;
using System.Security.Claims;


namespace Library.Tests.Domain
{
    public class LoanTests
    {
        [Fact]
        public void Create_SetsFields_AndStartsActive()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var borrowerId = Guid.NewGuid();

            // Act
            var loan = Loan.Create(bookId, borrowerId, "John Doe", "john@example.com", loanDurationDays: 14);

            // Assert
            Assert.Equal(bookId, loan.BookId);
            Assert.Equal(borrowerId, loan.BorrowerId);
            Assert.Equal("John Doe", loan.BorrowerName);
            Assert.Equal("john@example.com", loan.BorrowerEmail);
            Assert.True(loan.IsActive());
            Assert.True(loan.DueDate > loan.LoanDate);
        }

        [Fact]
        public void DetermineBorrowerId_MemberWithoutPrivilege_CannotLoanForOthers()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var someoneElse = Guid.NewGuid();
            var member = BuildUser(memberId /* no librarian/admin roles */);

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() =>
                Loan.DetermineBorrowerId(someoneElse, member));
        }

        [Fact]
        public void DetermineBorrowerId_Librarian_CanLoanForOthers()
        {
            // Arrange
            var librarianId = Guid.NewGuid();
            var targetBorrower = Guid.NewGuid();
            var librarian = BuildUser(librarianId, UserRoles.Librarian);

            // Act
            var borrowerId = Loan.DetermineBorrowerId(targetBorrower, librarian);

            // Assert
            Assert.Equal(targetBorrower, borrowerId);
        }

        [Fact]
        public void Return_OnTime_NoFines_AutoCompletes()
        {
            // Arrange
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Alice", "alice@example.com", 14);

            // Act
            loan.Return();

            // Assert
            Assert.True(loan.IsCompleted());
            Assert.NotNull(loan.ReturnDate);
            Assert.Empty(loan.Fines);
        }

        [Fact]
        public void Return_Overdue_AddsLateFee_AndKeepsReturnedUntilPaid()
        {
            // Arrange
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Bob", "bob@example.com", 14);

            SetPrivateProperty(loan, nameof(Loan.DueDate), DateTime.UtcNow.AddDays(-3));

            // Act
            loan.Return();

            // Assert
            Assert.Equal(LoanStatus.Returned, GetStatus(loan));
            var fine = Assert.Single(loan.Fines);
            Assert.Equal(FineType.LateFee, fine.Type);
            Assert.True(fine.IsPending);
        }

        [Fact]
        public void PayFine_AfterReturn_CompletesWhenAllFinesPaid()
        {
            // Arrange
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Bob", "bob@example.com", 14);
            SetPrivateProperty(loan, nameof(Loan.DueDate), DateTime.UtcNow.AddDays(-1)); // 1 day after due date
            loan.Return();

            var fine = loan.Fines.Single();
            Assert.Equal(LoanStatus.Returned, GetStatus(loan));
            Assert.True(fine.IsPending);

            // Act
            loan.PayFine(fine.Id, paymentReference: "TX-123");

            // Assert
            Assert.True(loan.IsCompleted());
            Assert.True(fine.IsPaid);
        }

        [Fact]
        public void MarkAsLost_SetsStatusLost_AndAddsReplacementFee()
        {
            // Arrange
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Carol", "carol@example.com", 14);

            // Act
            loan.MarkAsLost(Money.Create(42m, "EUR"));

            // Assert
            Assert.Equal(LoanStatus.Lost, GetStatus(loan));
            var fine = Assert.Single(loan.Fines);
            Assert.Equal(FineType.LostBookFee, fine.Type);
            Assert.True(fine.IsPending);
        }
        
        
        // ---------- Not Implemented Methods ----------
        
        // -----------------------
        // ExtendDueDate tests
        // -----------------------
        
        [Fact]
        public void ExtendDueDate_ActiveLoan_UpdatesDueDateByAdditionalDays()
        {
            // Arrange
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Alice", "alice@example.com", 14);
            var originalDue = loan.DueDate;

            // Act
            loan.ExtendDueDate(7);

            // Assert
            Assert.Equal(originalDue.AddDays(7), loan.DueDate);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void ExtendDueDate_AdditionalDaysMustBePositive(int additionalDays)
        {
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Bob", "bob@example.com", 14);

            Assert.Throws<ArgumentException>(() => loan.ExtendDueDate(additionalDays));
        }

        [Fact]
        public void ExtendDueDate_CannotExtendOverdueLoan()
        {
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Carol", "carol@example.com", 14);
            // Make it overdue
            SetPrivateProperty(loan, nameof(Loan.DueDate), DateTime.UtcNow.AddDays(-1));

            Assert.Throws<InvalidOperationException>(() => loan.ExtendDueDate(3));
        }

        [Fact]
        public void ExtendDueDate_TotalDurationCannotExceed90Days()
        {
            // Start with 60 days, try to add 31 => 91 total -> should fail
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Dan", "dan@example.com", 60);

            Assert.Throws<InvalidOperationException>(() => loan.ExtendDueDate(31));
        }

        [Fact]
        public void ExtendDueDate_OnlyForActiveLoans()
        {
            // Arrange: make the loan Returned (and auto-completed)
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Eve", "eve@example.com", 14);
            loan.Return(); // on-time -> completes

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => loan.ExtendDueDate(5));
        }

        // -----------------------
        // ReportDamage tests
        // -----------------------

        [Fact]
        public void ReportDamage_AddsDamageFine_WithReasonContainingDescription()
        {
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Frank", "frank@example.com", 14);

            loan.ReportDamage("Torn cover", Money.Create(5m, "EUR"));

            var fine = Assert.Single(loan.Fines);
            Assert.Equal(FineType.DamageFee, fine.Type);
            Assert.Contains("Torn cover", fine.Reason, StringComparison.OrdinalIgnoreCase);
            Assert.True(fine.IsPending);
            Assert.False(loan.IsCompleted()); // no auto-complete here
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ReportDamage_DescriptionMustBeNonEmpty(string? desc)
        {
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Gina", "gina@example.com", 14);

            Assert.Throws<ArgumentException>(() => loan.ReportDamage(desc!, Money.Create(3m, "EUR")));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ReportDamage_CostMustBePositive(decimal amount)
        {
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Hank", "hank@example.com", 14);

            // Money doesn't allow negative value
            if (amount < 0)
            {
                Assert.Throws<ArgumentException>(() => Money.Create(amount, "EUR"));
                return;
            }
            
            var money = Money.Create(amount, "EUR");
            Assert.ThrowsAny<Exception>(() => loan.ReportDamage("Scratched page", money));
        }

        [Fact]
        public void ReportDamage_NotAllowedWhenLost()
        {
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Ivy", "ivy@example.com", 14);
            loan.MarkAsLost(Money.Create(20m, "EUR"));

            Assert.Throws<InvalidOperationException>(() =>
                loan.ReportDamage("Cracked spine", Money.Create(7m, "EUR")));
        }

        [Fact]
        public void ReportDamage_NotAllowedWhenCompleted()
        {
            var loan = Loan.Create(Guid.NewGuid(), Guid.NewGuid(), "Jack", "jack@example.com", 14);
            loan.Return(); // on-time -> completes

            Assert.Throws<InvalidOperationException>(() =>
                loan.ReportDamage("Stains", Money.Create(6m, "EUR")));
        }

        // ---------- Helpers ----------
        private static ClaimsPrincipal BuildUser(Guid userId, params string[] roles)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            }.Concat(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        private static void SetPrivateProperty<T>(T obj, string propertyName, object value)
        {
            var prop = typeof(T).GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop == null)
                throw new InvalidOperationException($"Property '{propertyName}' not found on {typeof(T).Name}.");

            prop.SetValue(obj, value);
        }

        private static LoanStatus GetStatus(Loan loan)
        {
            var prop = typeof(Loan).GetProperty(nameof(Loan.Status),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return (LoanStatus)prop!.GetValue(loan)!;
        }
    }
}
