﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookAPI.Services
{
	interface IBookRepository
	{
		Task<IEnumerable<Entities.Book>> GetBooksAsync();
		Task<Entities.Book> GetBookAsync(Guid id);
	}
}
