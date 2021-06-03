using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAPI.Context;
using BookAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Services
{
	public class BookRepository : IBookRepository, IDisposable
	{
		private BooksContext _context;

		public BookRepository(BooksContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<Book> GetBookAsync(Guid id)
		{
			await _context.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:02';");
			return await _context.Books
				.Include(b => b.Author)
				.FirstOrDefaultAsync(b => b.Id  == id);
		}

		public async Task<IEnumerable<Book>> GetBooksAsync()
		{
			await _context.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:02';");
			return await _context.Books
				.Include(b => b.Author)
				.ToListAsync();
		}

		public Book GetBook(Guid id)
		{
			_context.Database.ExecuteSqlRaw("WAITFOR DELAY '00:00:02';");
			return _context.Books
				.Include(b => b.Author)
				.FirstOrDefault(b => b.Id == id);
		}

		public IEnumerable<Book> GetBooks()
		{
			_context.Database.ExecuteSqlRaw("WAITFOR DELAY '00:00:02';");
			return _context.Books
				.Include(b => b.Author)
				.ToList();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_context != null)
				{
					_context.Dispose();
					_context = null;
				}
			}
		}
	}
}
