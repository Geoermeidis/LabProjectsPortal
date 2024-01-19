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
using LabProjectsPortal.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace LabProjectsPortal.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly DataContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;
        public MessagesController(DataContext context, IHubContext<NotificationHub> hubContext, INotificationService notificationService)
        {
            _context = context;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        // GET: Conversations/{id}/Messages TODO
        public async Task<IActionResult> Index(Guid conversation)
        {
            var conv = _context.Conversations.Include(c => c.Participants).FirstOrDefault(m => m.Id == conversation);
            
            if (conv is null) { return NotFound();}
            
            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversation)
                .Include(m => m.Conversation).Include(m => m.Conversation.Participants)
                .Include(m => m.Sender).ToListAsync();

            messages = messages.OrderBy(m => m.SentAt).ToList();

            ViewData["ConversationId"] = conv.Id;
            ViewData["ConvTitle"] = conv.Title;
            ViewData["Participants"] = conv.Participants;

            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string content, string conversation)
        {
            if (content.IsNullOrEmpty() || conversation == null) {
                return NotFound();
            }
            var convId = Guid.Parse(conversation);
            var conv = _context.Conversations.Include(c => c.Participants).FirstOrDefault(c => c.Id == convId);

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

            await _notificationService.SendNotification(new NotificationDto()
            {
                Title = string.Format("Message at {0} by {1}", conv.Title, user.UserName),
                SentAt = DateTimeOffset.Now.DateTime.ToLocalTime().ToString(),
                Recipients = new List<string>() { conv.Participants.Where(c => c.Email != user.Email).First().Email },
                Message = message.Content
            });

            ViewData["ConversationId"] = conv.Id;
            ViewData["ConvTitle"] = conv.Title;
            ViewData["Participants"] = conv.Participants;

            return RedirectToAction("Index", new { conversation = convId});
        }
    }
}
