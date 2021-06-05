using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BookAPI.Context;
using BookAPI.Entities;
using BookAPI.ExternalModels;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Services
{
	public class BookRepository : IBookRepository, IDisposable
	{
		private BooksContext _context;
		private IHttpClientFactory _httpClientFactory;

		public BookRepository(BooksContext context, IHttpClientFactory httpClientFactory)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
		}

		public async Task<Book> GetBookAsync(Guid id)
		{
			await _context.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:02';");
			return await _context.Books
				.Include(b => b.Author)
				.FirstOrDefaultAsync(b => b.Id == id);
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

		public void AddBook(Book book)
		{
			if (book == null) throw new ArgumentNullException(nameof(book));
			_context.Add(book);
		}

		public async Task<bool> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> ids)
		{
			return await _context.Books
				.Where(b => ids.Contains(b.Id))
				.Include(b => b.Author)
				.ToListAsync();
		}

		public async Task<BookCover> GetBookCoverAsync(string coverId)
		{
			var httpClient = _httpClientFactory.CreateClient();
			var response = await httpClient.GetAsync($"http://localhost:52644/api/bookcovers/{coverId}");
			if (response.IsSuccessStatusCode)
				return JsonSerializer
					.Deserialize<BookCover>(
						await response.Content
							.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
					);

			return null;
		}

		public async Task<IEnumerable<BookCover>> GetBookCoversAsync(Guid bookId) {
			var httpClient = _httpClientFactory.CreateClient();

			var bookCover = new List<BookCover>();

			var bookCoverUrls = new[]
			{
				$"http://localhost:52644/api/bookcovers/{bookId}-dummycover1",
				$"http://localhost:52644/api/bookcovers/{bookId}-dummycover2",
				$"http://localhost:52644/api/bookcovers/{bookId}-dummycover3",
				$"http://localhost:52644/api/bookcovers/{bookId}-dummycover4",
				$"http://localhost:52644/api/bookcovers/{bookId}-dummycover5"
			};

			foreach (var bookCoverUrl in bookCoverUrls)
			{
				var response = await httpClient
					.GetAsync(bookCoverUrl);

				if (response.IsSuccessStatusCode)
				{
					bookCover.Add(JsonSerializer
						.Deserialize<BookCover>(
							await response.Content
								.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
						));
				}
			}


			return bookCover;
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
