using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MIS.Models;

namespace MIS.Models
{
    public class MisContext : DbContext
    {
        public DbSet<CommentCreateModel> CommentCreateModels { get; set; }
/*        public MisContext(DbContextOptions<CommentCreateModel> options) : base(options)
        {
            Database.EnsureCreated();
        }*/
    }
}