using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookAPI.Services
{
	public interface IBookRepository
	{
		Task<IEnumerable<Entities.Book>> GetBooksAsync();
		Task<Entities.Book> GetBookAsync(Guid id);
		IEnumerable<Entities.Book> GetBooks();
		Entities.Book GetBook(Guid id);

	}
}
