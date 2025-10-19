using FluentValidation;
using Library.Application.Repositories;

namespace Library.Application.Books.Commands;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;

    public CreateBookCommandValidator(IBookRepository bookRepository, IAuthorRepository authorRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Book title is required")
            .MaximumLength(200).WithMessage("Book title must not exceed 200 characters");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Valid author ID is required")
            .MustAsync(AuthorExists).WithMessage(x => $"Author with ID {x.AuthorId} not found");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN is required")
            .MaximumLength(20).WithMessage("ISBN must not exceed 20 characters")
            .MustAsync(IsUniqueIsbn).WithMessage(x => $"Book with ISBN {x.ISBN} already exists");

        RuleFor(x => x.Year)
            .InclusiveBetween(1000, DateTime.Now.Year + 10)
            .WithMessage($"Year must be between 1000 and {DateTime.Now.Year + 10}");

        RuleFor(x => x.Pages)
            .GreaterThan(0).WithMessage("Pages must be greater than 0");

        RuleFor(x => x.Genre)
            .NotEmpty().WithMessage("Genre is required")
            .MaximumLength(50).WithMessage("Genre must not exceed 50 characters");
    }

    private async Task<bool> AuthorExists(Guid authorId, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetByIdAsync(authorId);
        return author != null;
    }

    private async Task<bool> IsUniqueIsbn(string isbn, CancellationToken cancellationToken)
    {
        var existingBook = await _bookRepository.GetBookByIsbnAsync(isbn);
        return existingBook == null;
    }
}