using Common.Json.Models;

namespace Services.Base
{
    public class ServiceResponseModel
    {
        public ServiceResponseModel(ResponseMessage errors, ResponseMessage success, object rawData = null)
        {
            this.Errors = errors;
            this.Success = success;
            this.RawData = rawData;
        }

        public ResponseMessage Errors { get; set; }

        public ResponseMessage Success { get; set; }

        public object RawData { get; set; }
    }
}
