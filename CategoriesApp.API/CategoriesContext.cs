using CategoriesApp.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CategoriesApp.API
{

    public class CategoriesContext: DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public CategoriesContext(DbContextOptions<CategoriesContext> options): base(options)
        {

        }
    }
}
