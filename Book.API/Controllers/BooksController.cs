using System;
using System.Threading.Tasks;
using BookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
	[ApiController]
	[Route("api/books")]
	public class BooksController : ControllerBase
	{
		private readonly IBookRepository _bookRepository;

		public BooksController(IBookRepository bookRepository)
		{
			_bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
		}

		[HttpGet]
		public async Task<IActionResult> GetBooks()
		{
			var bookEntities = await _bookRepository.GetBooksAsync();
			return Ok(bookEntities);
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> GetBooks(Guid id)
		{
			var bookEntitie = await _bookRepository.GetBookAsync(id);
			return Ok(bookEntitie);
		}
	}
}
