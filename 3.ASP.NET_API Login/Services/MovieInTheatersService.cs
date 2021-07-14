using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASP.NET_API.Services
{
    /// <summary>
    /// Pone en true automaticamente en DB Movie el campo [InTheaters] si la fecha de lanzamiento de la pelicula coincide con el dia de hoy (es decir que estara en cines)
    /// </summary>
    public class MovieInTheatersService : IHostedService, IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private Timer timer;

        public MovieInTheatersService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var today = DateTime.Today;
                var movies = await context.Movie.Where(x => x.ReleaseDate == today).ToListAsync();
                if (movies.Any())
                {
                    foreach (var movie in movies)
                    {
                        movie.InTheaters = true;
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite,0);
            return Task.CompletedTask;
        }
    }
}