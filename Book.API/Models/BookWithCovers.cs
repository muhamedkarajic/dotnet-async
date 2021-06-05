using System.Collections.Generic;

namespace BookAPI.Models
{
	public class BookWithCovers : Book
	{
		public IEnumerable<BookCover> BookCovers { get; set; } = new List<BookCover>();
	}
}
