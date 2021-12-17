using Microsoft.AspNetCore.Identity;
using Database.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Infrastructure
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetId(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public async static Task<string> GetRoleAsync(this ClaimsPrincipal user, UserManager<User> userManager, User otherUser = null)
        {
            if (user == null)
            {
                return null;
            }

            User result;
            if (otherUser != null)
            {
                result = otherUser;
            } 
            else
            {
                result = await userManager.FindByIdAsync(GetId(user));
            }

            string role = userManager.GetRolesAsync(result).GetAwaiter().GetResult().FirstOrDefault();

            return role;
        }
    }
}
