using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookAPI.ExternalModels;

namespace BookAPI.Services
{
	public interface IBookRepository
	{
		Task<IEnumerable<Entities.Book>> GetBooksAsync();
		Task<Entities.Book> GetBookAsync(Guid id);
		Task<IEnumerable<Entities.Book>> GetBooksAsync(IEnumerable<Guid> ids);
		IEnumerable<Entities.Book> GetBooks();
		Entities.Book GetBook(Guid id);
		Task<BookCover> GetBookCoverAsync(string coverId);
		Task<IEnumerable<BookCover>> GetBookCoversAsync(Guid bookId);
		void AddBook(Entities.Book book);
		Task<bool> SaveChangesAsync();
	}
}
