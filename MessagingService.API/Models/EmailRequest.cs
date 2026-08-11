namespace MessagingService.API.Models;

// Models/EmailRequest.cs
    public class EmailRequest
    {
        public List<string> To { get; set; } = new();
        public string Subject { get; set; }
        public string Content { get; set; }
    }
