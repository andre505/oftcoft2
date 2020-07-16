using OFTENCOFTAPI.ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.ApplicationCore.Interfaces
{
    public interface IResetTokenRepository
    {
        ResetToken FindTokenByUserId(string userId);
        Task Create(ResetToken resetToken);
        Task UpdateByUserId(ResetToken resetToken);
        Task SaveToken(ResetToken resetToken);
        ResetToken FindToken(string token);
        bool VerifyIfTokenExpired(ResetToken resetToken);
    }
}
