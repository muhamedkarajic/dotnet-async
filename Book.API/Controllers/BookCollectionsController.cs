using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookAPI.Models;
using BookAPI.Services;
using Books.API.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
	[ApiController]
	[Route("api/bookcollections")]
	public class BookCollectionsController : ControllerBase
	{
		private readonly IBookRepository _bookRepository;
		private readonly IMapper _mapper;

		public BookCollectionsController(IBookRepository bookRepository, IMapper mapper)
		{
			_bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		[HttpPost]
		public async Task<IActionResult> CreateBookCollection(IEnumerable<BookForCreation> booksForCreation)
		{
			var books = _mapper.Map<IEnumerable<Entities.Book>>(booksForCreation);

			foreach (var book in books)
				_bookRepository.AddBook(book);

			await _bookRepository.SaveChangesAsync();

			return Ok();
		}

	}
}
