﻿using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;

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

			CreateMap<Models.BookForCreation, Entities.Book>();

			CreateMap<Entities.Book, Models.BookWithCovers>()
				.ForMember(
					dest => dest.Author,
					opt => opt
						.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}")
				);

			CreateMap<IEnumerable<ExternalModels.BookCover>, Models.BookWithCovers>()
				.ForMember(
					dest => dest.BookCovers,
					opt => opt.MapFrom(src => src)
				);

			CreateMap<ExternalModels.BookCover, Models.BookCover>();
		}
	}
}
