using Microsoft.AspNetCore.SignalR;

namespace LabProjectsPortal.Notifications
{
    public class NotificationService: INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService(IHubContext<NotificationHub> hubContext) 
        {
            _hubContext = hubContext;
        }

        public async Task SendNotification(NotificationDto message)
        {
            if (message.Message is string)
                await _hubContext.Clients.All.SendAsync("ReceiveText", message);
            else
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
            Console.WriteLine("Message sent successfully...");
        }
    }
}
