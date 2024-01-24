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
using Microsoft.AspNetCore.Authorization;

namespace LabProjectsPortal.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly DataContext _context;

        public ContactsController(DataContext context)
        {
            _context = context;
        }

        // GET: Contacts
        public IActionResult Index()
        {
            var user = User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userApp = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (userApp == null)
                return RedirectToAction("NotFound", "Home");

            // get all contacts where the request is accepted and the user is either receiver or sender
            var dataContext = _context.Contacts
                .Where(c => c.IsAccepted && (c.Sender.Id.Equals(userId) || c.Receiver.Id.Equals(userId)))
                .Include(c => c.Receiver).Include(c => c.Sender);

            List<Contact> contacts = new();
            
            foreach (var c in dataContext)
            {
                contacts.Add(c);
            }

            return View(contacts);
        }


        // GET: Contacts/Create
        public IActionResult Create()
        {
            var user = User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userApp = _context.Users.Include(c => c.UserReceivedContacts).
                Include(c => c.UserSentContacts).FirstOrDefault(u => u.Id == userId);
            
            if (userApp == null)
                return RedirectToAction("NotFound", "Home");

            // take all user ids who the current has already sent or be sent a contact request 
            var userContactsSent = _context.Contacts.Where(c => c.SenderId == userId).Select(c => c.ReceiverId).ToList();
            var userContactsReceived = _context.Contacts.Where(c => c.ReceiverId == userId).Select(c => c.SenderId).ToList();
            var nonAvailableUsers = userContactsReceived.Concat(userContactsSent).ToList();
            nonAvailableUsers.Add(userId);

            var usernames = _context.Users.Where(u => !nonAvailableUsers.Contains(u.Id)).Select(u => u.UserName).ToList();
                //_context.Users.Where(u => !u.Id.Equals(userId)).Select(u => u.UserName).ToList();
            
            return View(usernames);
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(string Username)
        {
            var user = User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userApp = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            
            if (userApp == null)
                return RedirectToAction("NotFound", "Home");

            var receiver = await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(Username));
            
            if (receiver == null)
                return RedirectToAction("NotFound", "Home");

            var contact = new Contact()
            {
                Sender = userApp,
                SenderId = userApp.Id,
                Receiver = receiver,
                ReceiverId = receiver.Id
            };
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Requests()
        {
            var user = User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userApp = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (userApp == null)
                return RedirectToAction("NotFound", "Home");

            var dataContext = _context.Contacts
                .Where(c => !c.IsAccepted && (c.Receiver.Id.Equals(userId) || c.Sender.Id.Equals(userId)))
                .Include(c => c.Sender).Include(c => c.Receiver).ToList();

            return View(dataContext);
        }


        public async Task<IActionResult> Accept(Guid? id)
        {
            var contact = _context.Contacts.First(c => c.Id == id);
            contact.IsAccepted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            var contact = _context.Contacts.First(c => c.Id == id);
            _context.Remove(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
