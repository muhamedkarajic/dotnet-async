using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookAPI.ExternalModels;
using BookAPI.Filters;
using BookAPI.Models;
using BookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
	[ApiController]
	[Route("api/books")]
	public class BooksController : ControllerBase
	{
		private readonly IBookRepository _bookRepository;
		private readonly IMapper _mapper;

		public BooksController(IBookRepository bookRepository, IMapper mapper)
		{
			_bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		[HttpGet]
		[BooksResultFilter]
		public async Task<IActionResult> GetBooks()
		{
			var bookEntities = await _bookRepository.GetBooksAsync();
			return Ok(bookEntities);
		}

		[HttpGet]
		[BookWithCoverResultFilter]
		[Route("{id}", Name="GetBook")]
		public async Task<IActionResult> GetBook(Guid id)
		{
			var bookEntitie = await _bookRepository.GetBookAsync(id);
			var bookCovers = await _bookRepository.GetBookCoversAsync(bookEntitie.Id);

			//var propertyBag = new Tuple<Entities.Book, IEnumerable<BookCover>>(bookEntitie, bookCovers);
			//(Entities.Book book, IEnumerable<BookCover> bookCovers) propertyBag = (bookEntitie, bookCovers);

			return Ok((bookEntitie, bookCovers));
		}

		[HttpPost]
		[BookResultFilter]
		public async Task<IActionResult> CreateBook(BookForCreation bookForCreation)
		{
			var book = _mapper.Map<Entities.Book>(bookForCreation);
			_bookRepository.AddBook(book);
			await _bookRepository.SaveChangesAsync();
			await _bookRepository.GetBookAsync(book.Id);
			return CreatedAtRoute("GetBook", new { id = book.Id }, book);
		}
	}
}
