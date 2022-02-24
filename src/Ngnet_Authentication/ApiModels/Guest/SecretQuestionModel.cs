using Database.Models;
using Mapper;

namespace ApiModels.Guest
{
    public class SecretQuestionModel : IMapFrom<SecretQuestion>
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }
    }
}
