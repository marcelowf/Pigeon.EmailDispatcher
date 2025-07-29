namespace Pigeon.EmailDispatcher.Service.Models
{
    public class EmailModel
    {
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public List<string> ToRecipients { get; set; } = new List<string>();
        public List<string>? CcRecipients { get; set; }
        public List<string>? BccRecipients { get; set; }
        public List<AttachmentModel>? Attachments { get; set; }
    }
}
