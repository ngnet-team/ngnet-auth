using Common.Json.Models;

namespace Services
{
    public class ServiceResponseModel
    {
        public ServiceResponseModel(LanguagesModel errors, LanguagesModel success, object rawData = null)
        {
            this.Errors = errors;
            this.Success = success;
            this.RawData = rawData;
        }

        public LanguagesModel Errors { get; set; }

        public LanguagesModel Success { get; set; }

        public object RawData { get; set; }
    }
}
