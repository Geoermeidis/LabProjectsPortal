using LabProjectsPortal.Models;

namespace LabProjectsPortal.Dto
{
    public class PostsCategoriesDto
    {
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<string> Categories { get; set; } = new List<string>();
    }
}
