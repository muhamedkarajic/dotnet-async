using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookAPI.Filters;
using BookAPI.Models;
using BookAPI.Services;
using Books.API.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
	[ApiController]
	[Route("api/bookcollections")]
	[BooksResultFilter]
	public class BookCollectionsController : ControllerBase
	{
		private readonly IBookRepository _bookRepository;
		private readonly IMapper _mapper;

		public BookCollectionsController(IBookRepository bookRepository, IMapper mapper)
		{
			_bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		[HttpGet("({bookIds})", Name="GetBookCollection")]
		public async Task<IActionResult> GetBookCollection(
			[ModelBinder(binderType: typeof(ArrayModelBinder))] IEnumerable<Guid> bookIds
		)
		{
			var books = await _bookRepository.GetBooksAsync(bookIds);
			if (books.Count() != bookIds.Count())
				return NotFound();
			return Ok(books);
		}

		[HttpPost]
		public async Task<IActionResult> CreateBookCollection(IEnumerable<BookForCreation> booksForCreation)
		{
			var books = _mapper.Map<IEnumerable<Entities.Book>>(booksForCreation);

			foreach (var book in books)
				_bookRepository.AddBook(book);

			await _bookRepository.SaveChangesAsync();

			var booksToReturn = await _bookRepository
				.GetBooksAsync(books.Select(b => b.Id).ToList());
			var bookIds = string.Join(",", booksToReturn.Select(b => b.Id));

			return CreatedAtRoute("GetBookCollection", new { bookIds }, booksToReturn);
		}

	}
}
