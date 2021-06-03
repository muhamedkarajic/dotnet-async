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
			return await _context.Books
				.Include(b => b.Author)
				.FirstOrDefaultAsync(b => b.Author.Id == id);
		}

		public async Task<IEnumerable<Book>> GetBooksAsync()
		{
			return await _context.Books
				.Include(b => b.Author)
				.ToListAsync();
		}

		public Book GetBook(Guid id)
		{
			return _context.Books
				.Include(b => b.Author)
				.FirstOrDefault(b => b.Author.Id == id);
		}

		public IEnumerable<Book> GetBooks()
		{
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
