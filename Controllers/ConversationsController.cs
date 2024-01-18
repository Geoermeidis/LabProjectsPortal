using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LabProjectsPortal.Data;
using LabProjectsPortal.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LabProjectsPortal.Controllers
{
    [Authorize]
    public class ConversationsController : Controller
    {
        private readonly DataContext _context;

        public ConversationsController(DataContext context)
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

        // GET: User/{id}/Conversations TODO
        public IActionResult Index(string? category)
        {
            InitCategories();

            List<string> courses = _context.Courses.Select(c => c.Title).ToList();
            List<string> hobbies = _context.Hobbies.Select(h => h.Title).ToList();
            List<string> categories = courses;

            categories.AddRange(hobbies);

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var user =  _context.Users
                               .Include(c => c.Conversations).ThenInclude(c => c.Category)
                               .Where(c => c.Id ==  userId).First();

            var conversations = user.Conversations;

            if (category != null)
                conversations = conversations.Where(s => s.Category.Title == category).ToList();

            ViewData["Categories"] = new SelectList(categories, "Category", "Category");

            return View(conversations);
        }

        // GET: Conversations/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Guid", "Discriminator");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,CategoryId")] Conversation conversation)
        {
            if (ModelState.IsValid)
            {
                conversation.Id = Guid.NewGuid();
                _context.Add(conversation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "Guid", "Discriminator", conversation.CategoryId);
            return View(conversation);
        }

        // GET: Conversations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            InitCategories();

            List<string> courses = _context.Courses.Select(c => c.Title).ToList()!;
            List<string> hobbies = _context.Hobbies.Select(h => h.Title).ToList()!;
            List<string> categories = courses;

            categories.AddRange(hobbies);

            if (id == null || _context.Conversations == null)
            {
                return NotFound();
            }

            var conversation = await _context.Conversations
                .Include(c => c.Category)
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (conversation == null)
            {
                return NotFound();
            }
            // they dont participate in this current conversation
            var users = _context.Users.Where(u => !u.Conversations.Contains(conversation)).Select(c => c.UserName);

            //ViewData["CatIds"] = new SelectList(_context.Hobbies.Select(c => c.Guid).ToList().Concat(_context.Courses.Select(c => c.Guid).ToList()));
            ViewData["Categories"] = new SelectList(categories);
            ViewData["Users"] = new SelectList(users);
            return View(conversation);
        }

        // POST: Conversations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, string category, [Bind("Id,Title,CategoryId")] Conversation conversation)
        {
            if (id != conversation.Id)
            {
                return NotFound();
            }

            List<string> courses = _context.Courses.Select(c => c.Title).ToList();
            List<string> hobbies = _context.Hobbies.Select(h => h.Title).ToList();
            
            Category newCat;

            if (courses.Contains(category))
                newCat = _context.Courses.Where(c => c.Title == category).First();
            else if (courses.Contains(category)) 
                newCat = _context.Hobbies.Where(c => c.Title == category).First();
            else
                return NotFound();

            conversation.Category = newCat;
            var users = _context.Users.Where(u => !u.Conversations.Contains(conversation)).Select(c => c.UserName);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(conversation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConversationExists(conversation.Id))
                    {
                        return NotFound();
                    }
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["Categories"] = new SelectList(courses.Concat(hobbies).ToList());
            ViewData["Users"] = new SelectList(users);

            return View(conversation);
        }

        // TODO DELETE IF NO OTHER PARTICIPANTS REMAIN
        // GET: Conversations/{}/Leave TODO
        public IActionResult Leave(Guid? id)
        {

            if (id == null || _context.Conversations == null)
            {
                return NotFound();
            }

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var user = _context.Users
                               .Include(c => c.Conversations).ThenInclude(c => c.Category)
                               .Where(c => c.Id == userId).First();

            var conversation = user.Conversations.Where(c => c.Id == id).First();
            
            if (conversation == null)
                return NotFound();

            user.Conversations.Remove(conversation);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult AddUser(string conversationId, string username)
        {
            if (conversationId == null || username == null)
                return NotFound();

            List<string> courses = _context.Courses.Select(c => c.Title).ToList()!;
            List<string> hobbies = _context.Hobbies.Select(h => h.Title).ToList()!;
            List<string> categories = courses;

            categories.AddRange(hobbies);

            var conversation = _context.Conversations
                            .Include(c => c.Category)
                            .Include(c => c.Participants)
                            .FirstOrDefault(c => c.Id == Guid.Parse(conversationId));

            if (conversation == null)
                return NotFound();

            var users = _context.Users.Where(u => !u.Conversations.Contains(conversation)).Select(c => c.UserName);
            var userToAdd = _context.Users.FirstOrDefault(c => c.UserName == username);
            
            if (userToAdd == null)
                return NotFound();

            conversation.Participants.Add(userToAdd);
            
            _context.SaveChanges();

            ViewData["Categories"] = new SelectList(categories);
            ViewData["Users"] = new SelectList(users);

            return RedirectToAction("Edit", "Conversations", new {id= Guid.Parse(conversationId)});
        }

        private bool ConversationExists(Guid id)
        {
          return (_context.Conversations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
