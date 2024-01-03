using System.ComponentModel.DataAnnotations;

namespace LabProjectsPortal.Models
{
    public class Category
    {
        [Key]
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = null!;
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
