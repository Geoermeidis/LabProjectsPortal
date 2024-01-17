using LabProjectsPortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LabProjectsPortal.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hobby>()
            .HasBaseType<Category>();

            modelBuilder.Entity<Course>()
                .HasBaseType<Category>();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Hobbies)
                .WithMany();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Courses)
                .WithMany();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.UserSentContacts)
                .WithOne(c => c.Sender)
                .HasForeignKey(c => c.SenderId);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.UserReceivedContacts)
                .WithOne(c => c.Receiver)
                .HasForeignKey(c => c.ReceiverId);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Conversations)
                .WithMany(c => c.Participants);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.Publisher)
                .HasForeignKey(c => c.PublisherId);

            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Messages)
                .WithOne(p => p.Conversation)
                .HasForeignKey(p => p.ConversationId);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Category)
                .WithMany();

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany();

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany();

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Sender)
                .WithMany()
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Receiver)
                .WithMany()
                .HasForeignKey(c => c.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);
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
