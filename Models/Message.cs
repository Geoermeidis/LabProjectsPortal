using System.ComponentModel.DataAnnotations;

namespace LabProjectsPortal.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = string.Empty;
        public string SenderId { get; set; }
        public Guid ConversationId { get; set; }
        [Editable(false)]
        public DateTimeOffset SentAt { get; set; } = DateTimeOffset.Now;
        public ApplicationUser? Sender { get; set; } = new ApplicationUser();
        public Conversation? Conversation { get; set; } = new Conversation();
    }
}
