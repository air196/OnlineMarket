using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineMarket.Domain.Dto;
using OnlineMarket.Domain.Helpers;
using OnlineMarket.Domain.Models;
using OnlineMarket.Domain.Response;
using OnlineMarket.Others;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OnlineMarket.Services
{
    // Аутентификация
    public partial class OnlineMarketService
    {
        /// <summary>
        /// Аутентификация
        /// </summary>
        /// <param name="userCredentials">Данные для аутентификации</param>
        /// <returns>JWT-токен</returns>
        public async Task<Response<string>> Auth([Required] UserCredentialsDto userCredentials)
        {
            var response = new Response<string>();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == userCredentials.Login);

                if (user == null)
                    throw new Exception("Пользователь с данным логином не найден");

                if (user.Password != Sha256Helper.GetHash(userCredentials.Password))
                    throw new Exception("Пароль указан неверно");

                _currentUser = user;

                response.SetOk(CreateJwtToken());
            }
            catch (Exception ex)
            {
                response.SetInternalError(ex);
            }

            return response;
        }

        /// <summary>
        /// Refresh токен
        /// </summary>
        /// <returns>Refresh токен</returns>
        public RefreshToken GetRefreshToken()
        {
            CheckAuthorized();

            return _refreshTokens[_currentUser];
        }

        private void CheckAuthorized()
        {
            if (_currentUser == null || _currentUser.Id <= 0)
                throw new Exception("Пользователь не авторизован");
        }

        /// <summary>
        /// Создать JWT-токен
        /// </summary>
        /// <returns>JWT-токен</returns>
        public string CreateJwtToken()
        {
            CheckAuthorized();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _currentUser.Login,
                audience: "OnlineMarket",
                claims: GetUserClaims(_currentUser),
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void AddOrUpdateRefreshToken(RefreshToken refreshToken)
        {
            CheckAuthorized();

            if (!_refreshTokens.TryAdd(_currentUser, refreshToken))
                _refreshTokens[_currentUser] = refreshToken;
        }

        /// <summary>
        /// Обновить токен
        /// </summary>
        /// <returns>Refresh токен</returns>
        public RefreshToken RefreshToken()
        {
            CheckAuthorized();

            var refreshToken = GenerateRefreshToken();
            AddOrUpdateRefreshToken(refreshToken);

            return refreshToken;
        }

        private List<Claim> GetUserClaims(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, "User")
            };

            if (user.Role == Domain.Enums.UserRole.Moderator)
                claims.Add(new Claim(ClaimTypes.Role, "Moderator"));
            if (user.Role == Domain.Enums.UserRole.Admin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            return claims;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var curDateTime = DateTime.UtcNow;

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = curDateTime.AddDays(7),
                Created = curDateTime
            };

            return refreshToken;
        }
    }
}
