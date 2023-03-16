using Microsoft.Extensions.Options;
using OnlineMarket.Configuration;
using OnlineMarket.DAL;
using OnlineMarket.Domain.Models;
using OnlineMarket.Others;

namespace OnlineMarket.Services
{
    public partial class OnlineMarketService
    {
        private readonly MarketDbContext _context;
        private readonly JwtSettings _jwtSettings;
        private static Dictionary<User, RefreshToken> _refreshTokens = new Dictionary<User, RefreshToken>();
        private User _currentUser { get; set; }
        private long _userId => _currentUser.Id;
        public OnlineMarketService(MarketDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        private async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesWithLogsAsync(_userId);
        }
    }
}
