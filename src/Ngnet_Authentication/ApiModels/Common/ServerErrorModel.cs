namespace ApiModels.Common
{
    public class ServerErrorModel
    {
        public ServerErrorModel(string message, string type = null)
        {
            this.Message = message;
            this.Type = type;
        }

        public string Message { get; set; }

        public string Type { get; set; }
    }
}
