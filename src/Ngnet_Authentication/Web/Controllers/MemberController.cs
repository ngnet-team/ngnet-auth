using Microsoft.Extensions.Configuration;

using Common.Enums;
using Common.Json.Service;
using Services.Email;
using Services.Interfaces;

namespace Web.Controllers
{
    public class MemberController : UserController
    {
        private readonly IMemberService memberService;

        public MemberController
            (IMemberService memberService,
             IEmailSenderService emailSenderService,
             IConfiguration configuration,
             JsonService jsonService)
            : base(memberService, emailSenderService, configuration, jsonService)
        {
            this.memberService = memberService;
        }

        protected override RoleType RoleRequired { get; } = RoleType.Member;
    }
}
