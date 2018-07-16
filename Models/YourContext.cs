using Microsoft.EntityFrameworkCore;

namespace Planner.Models//a namespace where this lives and enables connection to other model classes
{
    public class YourContext : DbContext//this is a model class called Yourconext
    {
        public YourContext(DbContextOptions<YourContext> options) : base(options) {}
        public DbSet<User> users { get; set; }//connects the "User" class within models to the users table within db
        public DbSet<Wedding> weddings { get; set; }//connects the "Wedding" class within models to the weddings table within db
        public DbSet<RSVP> rsvps { get; set; }//connects the "RSVP" class within models to the rsvps table within db
    }
}