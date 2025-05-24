using Microsoft.EntityFrameworkCore;

namespace MessCoreEvi.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public decimal Amount { get; set; }
        public bool IsPermanent { get; set; }
        public DateTime Date { get; set; }
        public string ImgUrl { get; set; }
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();
    }

    public class Room
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public virtual ICollection<Member> Members { get; set; } = new List<Member>();
    }


    public class Facility
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int MemberId { get; set; }
        public virtual Member? Member { get; set; }
    }


    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> op) : base(op)
        {

        }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>().HasData(
                new Room() { RoomId = 1, RoomName = "RN-01" }
                );
        }
    }
}
