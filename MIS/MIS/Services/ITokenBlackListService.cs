using Microsoft.EntityFrameworkCore;
using MIS.Models.DB;

namespace MIS.Services
{
    public interface ITokenBlackListService
    {
        Task AddTokenToBlackList(string token);
        Task<bool> iSTokenRevoked(string token);
    }
    public class TokenBlackListService : ITokenBlackListService
    {
        private readonly MisDbContext _context;

        public TokenBlackListService(MisDbContext context)
        {
            _context = context;
        }

        public async Task AddTokenToBlackList(string token)
        {
            var DbToken = new DbTokenBlackList
            {
                token = token
            };

            await _context.TokenBlackList.AddAsync(DbToken);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> iSTokenRevoked(string token)
        {
            return await _context.TokenBlackList.AnyAsync(t => t.token == token);
        }
    }
}
