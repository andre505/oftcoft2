using Microsoft.AspNetCore.Authentication;
using OFTENCOFTAPI.ApplicationCore.DTOs;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ApplicationCore.Interfaces
{
    public interface IIdentityService
    {
        Task<RegistrationResultDTO> RegisterAsync(string email, string password, bool rememberMe, string lastname, string firstname, string phonenumber);
        Task<LoginResultDTO> LoginAsync(string email, string password, bool rememberMe);
        Task<bool> VerifyAccountExist(string email);
        Task<ResetRequestResultDTO> SendResetToken(string email);
        Task<ResetRequestResultDTO> ResetPassword(string token, string newPassword, string email);
    }
}
