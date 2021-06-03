﻿using AutoMapper;

namespace BookAPI.Profiles
{
	public class BooksProfile : Profile
	{
		public BooksProfile()
		{
			CreateMap<Entities.Book, Models.Book>()
				.ForMember(
					dest => dest.Author,
					opt => opt
						.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}")
				);
		}
	}
}
