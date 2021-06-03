using System;
using System.Threading.Tasks;
using BookAPI.Filters;
using BookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
	[ApiController]
	[Route("api/sync/books")]
	public class SyncBooksController : ControllerBase
	{
		private readonly IBookRepository _bookRepository;

		public SyncBooksController(IBookRepository bookRepository)
		{
			_bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
		}

		[HttpGet]
		[BooksResultFilter]
		public IActionResult GetBooks()
		{
			var bookEntities = _bookRepository.GetBooks();
			return Ok(bookEntities);
		}

		[HttpGet]
		[BookResultFilter]
		[Route("{id}")]
		public IActionResult GetBooks(Guid id)
		{
			var bookEntitie = _bookRepository.GetBook(id);
			return Ok(bookEntitie);
		}
	}
}
