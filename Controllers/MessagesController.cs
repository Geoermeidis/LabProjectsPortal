using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LabProjectsPortal.Data;
using LabProjectsPortal.Models;
using NuGet.ProjectModel;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace LabProjectsPortal.Controllers
{
    public class MessagesController : Controller
    {
        private readonly DataContext _context;

        public MessagesController(DataContext context)
        {
            _context = context;
        }

        // GET: Conversations/{id}/Messages TODO
        public async Task<IActionResult> Index(Guid conversation)
        {
            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversation)
                .Include(m => m.Conversation).Include(m => m.Conversation.Participants)
                .Include(m => m.Sender).ToListAsync();

            messages = messages.OrderBy(m => m.SentAt).ToList();

            return View(messages);
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Messages == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Conversation)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string content, string conversation)
        {
            if (content.IsNullOrEmpty() || conversation == null) {
                return NotFound();
            }
            var convId = Guid.Parse(conversation);
            var conv = _context.Conversations.FirstOrDefault(c => c.Id == convId);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _context.Users.Where(c => c.Id == userId).First();

            if (user == null || conv == null)
                return NotFound();

            Message message = new Message()
            {
                Id = Guid.NewGuid(),
                ConversationId = convId,
                SenderId = userId,
                Content = content,
                Conversation = conv,
                Sender = user
            };

            
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new {conversation =convId});
        }
    }
}
