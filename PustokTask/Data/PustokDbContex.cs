using Microsoft.EntityFrameworkCore;
using PustokTask.Models;

namespace PustokTask.Data
{
    public class PustokDbContex : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }
        public PustokDbContex(DbContextOptions options) : base(options)
        {
        }
    }
}
