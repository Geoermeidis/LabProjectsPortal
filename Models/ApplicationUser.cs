using System.ComponentModel.DataAnnotations;

namespace LabProjectsPortal.Models
{
    public class ApplicationUser
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        [Editable(false)]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<Contact> UserSentContacts { get; set; } = new List<Contact>();
        public ICollection<Contact> UserReceivedContacts { get; set; } = new List<Contact>();
        public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
