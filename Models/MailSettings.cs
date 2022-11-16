namespace NeoContact.Models
{
    public class MailSettings
    {
        //ADD Lesson #38 Email Contact - Adding The Mail Settings Model
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? DisplayName { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
    }
}