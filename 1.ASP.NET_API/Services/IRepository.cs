using ASP.NET_API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.Services
{
    public interface IRepository
    {
        void AddGenre(Genre gen);
        List<Genre> GetAllGenres();
        Genre GetGenreById(int id);
    }
}
