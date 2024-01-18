using System.ComponentModel;

namespace LabProjectsPortal.Models.Metadata
{
    public class UserMetadata
    {
        [DisplayName("First name")]
        public string FirstName { get; set; }
        [DisplayName("Lasst name")]
        public string LastName { get; set; }
    }
}
