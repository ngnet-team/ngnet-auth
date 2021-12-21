namespace Common
{
    public static class DevValidationMessages
    {
        public const string RequiredAppSettingDevelopment = 
            "You must create an appsetting.Developer.json file with Admin { Email, Username and Password } required and { FirstName, LastName and Age } optianal properties";

        public static string EmptryEmailSenderAddresses = "Empty email sender addresses!"; 

        public static string ByAuthor(string author) => $"By {author}";
    }
}
