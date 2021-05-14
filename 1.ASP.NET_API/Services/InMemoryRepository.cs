using ASP.NET_API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Services
{
    public class InMemoryRepository : IRepository
    {
        private List<Genre> _genres;

        public InMemoryRepository()
        {
            _genres = new List<Genre>()
            {
                new Genre(){ Id= 1 , Name = "Comedy"},
                new Genre(){ Id= 2 , Name = "Action"}
            };
        }       


        public List<Genre> GetAllGenres()
        {
            return _genres;
        }

        public Genre GetGenreById(int id)
        {
            return _genres.FirstOrDefault(genre => genre.Id == id);
        }


        public void AddGenre(Genre gen)
        {
            gen.Id = _genres.Max(x => x.Id) + 1;
            _genres.Add(gen);
        }
    }
}
