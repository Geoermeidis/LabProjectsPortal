﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabProjectsPortal.Models
{
    public partial class ApplicationUser: IdentityUser
    {
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;

        [Editable(false)]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<Contact> UserSentContacts { get; set; } = new List<Contact>();
        public ICollection<Contact> UserReceivedContacts { get; set; } = new List<Contact>();
        public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();
    }
}
