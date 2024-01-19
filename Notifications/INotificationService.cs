namespace LabProjectsPortal.Notifications
{
    public interface INotificationService
    {
        public Task SendNotification(NotificationDto message);
    }
}
