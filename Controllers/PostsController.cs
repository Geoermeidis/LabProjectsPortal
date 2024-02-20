using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LabProjectsPortal.Data;
using LabProjectsPortal.Models;
using System.Security.Claims;
using LabProjectsPortal.Dto;

namespace LabProjectsPortal.Controllers
{
    public class PostsController : Controller
    {
        private readonly DataContext _context;

        public PostsController(DataContext context)
        {
            _context = context;
        }

        private void InitCategories()
        {
            if (_context.Hobbies.Any() || _context.Courses.Any())
                return;

            var courseNames = new List<string>() { "Maths", "Logic Programming", "Image Analysis", "Web Development" };
            var hobbieNames = new List<string>() { "Painting", "Music", "Basketball", "Gym", "Tennis", "Reading", "Cooking" };
            var courses = new List<Course>();
            var hobbies = new List<Hobby>();
            foreach (var name in courseNames)
                courses.Add(new Course()
                {
                    Title = name
                });
            foreach (var name in hobbieNames)
                hobbies.Add(new Hobby()
                {
                    Title = name
                });
            _context.Courses.AddRange(courses);
            _context.Hobbies.AddRange(hobbies);
            _context.SaveChanges();
        }

        // GET: Posts
        public async Task<IActionResult> Index([FromQuery] string? category)
        {
            InitCategories();

            List<string> courses = _context.Courses.Select(c => c.Title).ToList();
            List<string> hobbies = _context.Hobbies.Select(h => h.Title).ToList();

            List<Post> dataContext;
            if (category is null || category.Equals("All"))
                dataContext = await _context.Posts.Include(p => p.Category).Include(p => p.Publisher).ToListAsync();
            else
            {
                dataContext = await _context.Posts.Where(p => p.Category.Title.Equals(category)).Include(p => p.Category).Include(p => p.Publisher).ToListAsync();
            }

            var categ = new List<string>();
            categ.Add("All");

            return View(new PostsCategoriesDto()
            {
                Posts = dataContext.OrderBy( m => m.UploadedAt).Reverse().ToList(),
                Categories = categ.Concat(courses).ToList().Concat(hobbies).ToList()
            });
        }

        

        // GET: Posts/Create
        public IActionResult Create()
        {
            List<string> courses = _context.Courses.Select(c => c.Title).ToList();
            List<string> hobbies = _context.Hobbies.Select(h => h.Title).ToList();
            return View(courses.Concat(hobbies).ToList());
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(string Content, string CategoryTitle)
        {
            var user = User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userApp = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (userApp == null)
                return RedirectToAction("NotFound", "Home");
           
            var course = _context.Courses.FirstOrDefault(c => c.Title.Equals(CategoryTitle));
            var hobby = _context.Hobbies.FirstOrDefault(h => h.Title.Equals(CategoryTitle));

            Category category = course is null ? hobby : course;

            Post post = new Post()
            {
                Content = Content,
                Category = category,
                CategoryId = category.Title,
                Publisher = userApp,
                PublisherId = userApp.Id
            };
            _context.Posts.Add(post);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
