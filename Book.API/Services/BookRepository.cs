using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BookAPI.Context;
using BookAPI.Entities;
using BookAPI.ExternalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookAPI.Services
{
	public class BookRepository : IBookRepository, IDisposable
	{
		private BooksContext _context;
		private IHttpClientFactory _httpClientFactory;
		private readonly ILogger<BookRepository> _logger;
		private CancellationTokenSource _cancellationTokenSource;

		public BookRepository(BooksContext context, IHttpClientFactory httpClientFactory, ILogger<BookRepository> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
			_cancellationTokenSource = new CancellationTokenSource();

			var bookCoverUrls = new[]
			{
				$"http://localhost:52644/api/bookcovers/{bookId}-dummycover1",
				$"http://localhost:52644/api/bookcovers/{bookId}-dummycover2",
				//$"http://localhost:52644/api/bookcovers/{bookId}-dummycover2?returnFault=true",
				$"http://localhost:52644/api/bookcovers/{bookId}-dummycover3",
				$"http://localhost:52644/api/bookcovers/{bookId}-dummycover4",
				$"http://localhost:52644/api/bookcovers/{bookId}-dummycover5"
			};

			var downloadBookCoverTasksQuery =
				from bookCoverUrl
				in bookCoverUrls
				select DownloadBookCoverAsync(httpClient, bookCoverUrl, _cancellationTokenSource.Token);

			var downloadBookCoverTasks = downloadBookCoverTasksQuery.ToList();

			try
			{
				return await Task.WhenAll(downloadBookCoverTasks);
			}
			catch (OperationCanceledException operationCanceledException)
			{
				_logger.LogInformation($"{operationCanceledException.Message}");
				foreach (var task in downloadBookCoverTasks)
					_logger.LogInformation($"Task {task.Id} as status {task.Status}");

				return new List<BookCover>();
			}
			catch (Exception ex)
			{
				_logger.LogError($"{ex.Message}");
				throw;
			}
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
				if (_cancellationTokenSource != null)
				{
					_cancellationTokenSource.Dispose();
					_cancellationTokenSource = null;
				}
			}
		}

		private async Task<BookCover> DownloadBookCoverAsync(
            HttpClient httpClient, string bookCoverUrl, CancellationToken cancellationToken
		)
        {

			//throw new Exception("Cannot download book cover, " +
			//	"writer isn't finishing book fast enough.");

			var response = await httpClient.GetAsync(bookCoverUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var bookCover = JsonSerializer.Deserialize<BookCover>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });
                return bookCover;
            }

			_cancellationTokenSource.Cancel();
            return null;
        }
	}
}
