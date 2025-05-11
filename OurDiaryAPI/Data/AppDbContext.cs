using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OurDiaryAPI.Models;

namespace OurDiaryAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<DiaryEntry> DiaryEntries { get; set; }
    }
}