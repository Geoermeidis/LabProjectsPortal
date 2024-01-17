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
        public async Task<IActionResult> Index()
        {
            var user = User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userApp = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (userApp == null)
                throw new Exception();

            var dataContext = _context.Contacts
                .Where(c => c.IsAccepted && (c.Sender.Id.Equals(userId) || c.Receiver.Id.Equals(userId)))
                .Include(c => c.Receiver).Include(c => c.Sender);
            List<string> contacts = new List<string>();
            foreach (var c in dataContext)
            {
                if (c.SenderId.Equals(userId))
                    contacts.Add(c.Receiver.Email);
                else
                    contacts.Add(c.Sender.Email);
            }
            return View(contacts);
        }


        // GET: Contacts/Create
        public IActionResult Create()
        {
            var user = User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userApp = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (userApp == null)
                throw new Exception();
            var emails = _context.Users.Where(u => !u.Id.Equals(userId)).Select(u => u.Email).ToList();
            return View(emails);
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(string Email)
        {
            var user = User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userApp = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (userApp == null)
                throw new Exception();

            var receiver = _context.Users.FirstOrDefault(u => u.Email.Equals(Email));
            if (receiver == null)
                throw new Exception();

            var contact = new Contact()
            {
                Sender = userApp,
                SenderId = userApp.Id,
                Receiver = receiver,
                ReceiverId = userApp.Id
            };
            _context.Contacts.Add(contact);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Requests()
        {
            var user = User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userApp = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (userApp == null)
                throw new Exception();

            var dataContext = _context.Contacts
                .Where(c => !c.IsAccepted && c.Receiver.Id.Equals(userId))
                .Include(c => c.Sender).ToList();

            return View(dataContext);
        }


        public async Task<IActionResult> Accept(Guid? id)
        {
            var contact = _context.Contacts.First(c => c.Id == id);
            contact.IsAccepted = true;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            var contact = _context.Contacts.First(c => c.Id == id);
            _context.Remove(contact);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
