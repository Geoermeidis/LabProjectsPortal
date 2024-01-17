using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LabProjectsPortal.Models
{
    public class Conversation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [AllowNull]
        public string Title { get; set; }
        [AllowNull]
        public Guid CategoryId { get; set; }

        public ICollection<ApplicationUser> Participants { get; set; } = new List<ApplicationUser>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public Category Category { get; set; } = new Category();
    }
}
