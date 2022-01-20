using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Common
{
    public static class Global
    {
        private const string EmailPattern = 
            @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"; //TODO: needs to be upgraded, copied from: regexr.com/3e48o

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

        public const int HashBytes = 10;

        public const double JwtTokenExpires = 30; // Days

        public static string CreateRandom => Guid.NewGuid().ToString().Substring(0, PasswordMinLength);

        public static bool EmailValidator(string emailAddress)
        {
            // ------- Local validation ------- 
            var matching = Regex.IsMatch(emailAddress, EmailPattern);
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
    }
}
