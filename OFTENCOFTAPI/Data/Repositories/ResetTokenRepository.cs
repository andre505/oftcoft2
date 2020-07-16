using Microsoft.EntityFrameworkCore;
using OFTENCOFTAPI.ApplicationCore.Interfaces;
using OFTENCOFTAPI.ApplicationCore.Models;
using OFTENCOFTAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.Data.Repositories
{
    public class ResetTokenRepository : IResetTokenRepository
    {
        private readonly OFTENCOFTDBContext _context;

        public ResetTokenRepository(OFTENCOFTDBContext context)
        {
            _context = context;
        }

        public async Task Create(ResetToken resetToken)
        {
            _context.ResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();
        }

        public ResetToken FindTokenByUserId(string userId)
        {
            return _context.ResetTokens.Where(token => token.UserId == userId).SingleOrDefault();
        }

        public async Task SaveToken(ResetToken resetToken)
        {
            var userToken = this.FindTokenByUserId(resetToken.UserId);

            if(userToken == null)
            {
                await this.Create(resetToken);
            }
            else
            {
                await this.UpdateByUserId(resetToken);
            }
        }

        public async Task UpdateByUserId(ResetToken resetToken)
        {
            var resetTokenInDb = await _context.ResetTokens.Where(t => t.UserId == resetToken.UserId).SingleOrDefaultAsync();

            if(resetToken != null)
            {
                resetTokenInDb.Token = resetToken.Token;
                resetTokenInDb.TokenExpiry = resetToken.TokenExpiry;

                _context.Entry(resetTokenInDb).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
                                    
        }

        public ResetToken FindToken(string token)
        {
            return _context.ResetTokens.Where(t => t.Token == token).SingleOrDefault();
        }

        public bool VerifyIfTokenExpired(ResetToken resetToken)
        {
            //check for expiry
            var currentDateTime = DateTime.Now;
            var tokenExipry = resetToken.TokenExpiry;

            TimeSpan span = tokenExipry.Subtract(currentDateTime);

            if(span.TotalMinutes > 0)
            {
                return false;
            }

            return true;
        }
    }
}
