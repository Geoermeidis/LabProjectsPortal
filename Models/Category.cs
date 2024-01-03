using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace LabProjectsPortal.Models
{
    public class Category
    {
        [Key]
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = null!;
    }
}
