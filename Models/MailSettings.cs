namespace NeoContact.Models
{
    public class MailSettings
    {
        //ADD #38 Email Contact - Adding The Mail Settings Model
        public string? Email { get; set; }
        public string? EmailPassword { get; set; }
        public string? DisplayName { get; set; }
        public string? EmailHost { get; set; }
        public int EmailPort { get; set; }
    }
}