using System.ComponentModel.DataAnnotations;

namespace LabProjectsPortal.Models
{
    public class Contact
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Editable(false)]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public bool IsAccepted { get; set; } = false;
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }

        public ApplicationUser Sender { get; set; } = new ApplicationUser();
        public ApplicationUser Receiver { get; set; } = new ApplicationUser();
    }
}
