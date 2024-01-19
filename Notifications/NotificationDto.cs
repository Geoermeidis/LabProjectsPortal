namespace LabProjectsPortal.Notifications
{
    public class NotificationDto
    {
        public List<string> Recipients { get; set; } = null!;
        public object Message { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string SentAt { get; set; } = DateTimeOffset.Now.DateTime.ToLocalTime().ToString();
    }
}
