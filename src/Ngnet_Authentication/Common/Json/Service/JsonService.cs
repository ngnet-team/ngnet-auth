using System;
using System.IO;
using System.Text.Json;

namespace Common.Json.Service
{
    public class JsonService : IJsonService
    {
        public T Deserialiaze<T>(string fileName)
        {
            try
            {
                var jsonFile = File.ReadAllText(Paths.JsonDirectory + fileName);
                return JsonSerializer.Deserialize<T>(jsonFile, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public object Serialiaze<T>(string fileName, T model)
        {
            try
            {
                return JsonSerializer.Serialize(model);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
