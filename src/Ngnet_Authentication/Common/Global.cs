using Common.Enums;
using Common.Json.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Common
{
    public static class Global
    {
        public const int EmailMinLength = 8;

        public const int EmailMaxLength = 50;

        public const int UsernameMinLength = 6;

        public const int UsernameMaxLength = 50;

        public const int PasswordMinLength = 6;

        public const int PasswordMaxLength = 50;

        public const int NameMinLength = 3;

        public const int NameMaxLength = 50;

        public const int AgeMin = 0;
    
        public const int AgeMax = 120;

        public static string CreateRandom => Guid.NewGuid().ToString().Substring(0, PasswordMinLength);

        public static bool EmailValidator(string emailAddress)
        {
            // ------- Local validation ------- 
            var matching = Regex.IsMatch(emailAddress, Constants.EmailPattern);
            if (!matching)
                return false;

            return true; // need valid send grid api key before code below...

            // ------- real email validation ------- 
            //EmailSenderModel model = new EmailSenderModel(this.Admin.Email, emailAddress);
            //Response response = await this.emailSenderService.EmailConfirmation(model);

            //if (response == null || !response.IsSuccessStatusCode)
            //{
            //    return this.GetErrors().InvalidEmail;
            //}

            //return null;
        }

        public static bool NullableObject(object instance)
        {
            if (instance == null)
                return true;

            foreach (PropertyInfo pi in instance.GetType().GetProperties())
            {
                var value = pi.GetValue(instance);
                if (value != null)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AnyNullObject(object instance)
        {
            foreach (PropertyInfo pi in instance.GetType().GetProperties())
            {
                var value = pi.GetValue(instance);
                if (value == null)
                {
                    return true;
                }
            }

            return false;
        }

        public static GenderType GetGender(string gender)
        {
            if (gender == null)
                return GenderType.Undefined;

            GenderType genderType;
            bool valid = Enum.TryParse<GenderType>(gender, out genderType);

            if (!valid)
                return GenderType.Undefined;

            return genderType;
        }

        public static ConstantsModel Constants => Deserialiaze<ConstantsModel>(Paths.Constants);

        public static ServerErrorsModel ServerErrors => Deserialiaze<ServerErrorsModel>(Paths.ServerErrors);

        private static T Deserialiaze<T>(string fileName)
        {
            try
            {
                var jsonFile = File.ReadAllText(Paths.JsonDirectory + fileName);
                return JsonSerializer.Deserialize<T>(jsonFile, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                });
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
