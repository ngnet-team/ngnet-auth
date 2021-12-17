namespace Services.Email
{
    public class EmailSenderModel
    {
        public EmailSenderModel(string fromAddress, string toAddress)
        {
            this.FromAddress = fromAddress;
            this.ToAddress = toAddress;
        }

        public string FromAddress { get; set; }

        public string FromName { get; set; }

        public string ToAddress { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }
    }
}
