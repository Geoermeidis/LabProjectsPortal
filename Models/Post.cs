using System.ComponentModel.DataAnnotations;

namespace LabProjectsPortal.Models
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = string.Empty;
        [Editable(false)]
        public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
        public string PublisherId { get; set; }
        public string CategoryId { get; set; }
        public ApplicationUser? Publisher { get; set; } = new ApplicationUser();
        public Category? Category { get; set; } = new Category();
    }
}
