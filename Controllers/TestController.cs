using LabProjectsPortal.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LabProjectsPortal.Controllers
{
    [Route("[controller]")]
    public class TestController : Controller
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;
        public TestController(IHubContext<NotificationHub> hubContext, INotificationService notificationService) 
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
        }
        [HttpGet]
        public async Task<IActionResult> SendMessage()
        {
            await _notificationService.SendNotification(new NotificationDto()
                                                        {
                                                            Email = "tonykristaki@gmail.com",
                                                            Message = "testtt"
                                                        });

            return Ok("popa");
        }
    }
}
