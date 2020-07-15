using Microsoft.AspNetCore.Authentication;
using OFTENCOFTAPI.ApplicationCore.DTOs;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ApplicationCore.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthenticationResultDTO> RegisterAsync();
        Task<LoginResultDTO> LoginAsync(string email, string password, bool rememberMe);
    }
}
