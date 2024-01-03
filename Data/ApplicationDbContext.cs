using Microsoft.EntityFrameworkCore;
using LabProjectsPortal.Models;

namespace LabProjectsPortal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Courses)
                .WithMany(c => c.Users);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Hobbies)
                .WithMany(h => h.Users);

            modelBuilder.Entity<Hobby>()
                .HasBaseType<Category>();

            modelBuilder.Entity<Course>()
                .HasBaseType<Category>();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.UserSentContacts)
                .WithOne(c => c.Sender)
                .HasForeignKey(c => c.SenderId)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.UserReceivedContacts)
                .WithOne(c => c.Receiver)
                .HasForeignKey(c => c.ReceiverId)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Conversations)
                .WithMany(c => c.Participants);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.Publisher)
                .HasForeignKey(c => c.PublisherId)
                .IsRequired();

            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Messages)
                .WithOne(p => p.Conversation)
                .HasForeignKey(p => p.ConversationId)
                .IsRequired();

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Category)
                .WithMany(c => c.Conversations)
                .HasForeignKey(c => c.CategoryId)
                .IsRequired();

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(c => c.CategoryId)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(s => s.Messages)
                .HasForeignKey(m => m.SenderId)
                .IsRequired();
        }


        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Hobby> Hobbies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
