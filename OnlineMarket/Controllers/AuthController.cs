using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineMarket.Domain.Dto;
using OnlineMarket.Others;
using OnlineMarket.Services;
using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Controllers
{
    /// <summary>
    /// Аутентификация
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly OnlineMarketService _service;
        public AuthController(OnlineMarketService service)
        {
            _service = service;
        }

        /// <summary>
        /// Аутентификация
        /// </summary>
        /// <param name="userCredentials">Данные для аутентификации</param>
        /// <returns>JWT-токен</returns>
        [HttpPost]
        public async Task<ActionResult<string>> Auth([Required][FromBody] UserCredentialsDto userCredentials)
        {
            var response = await _service.Auth(userCredentials);

            if (response.StatusCode == StatusCodes.Status200OK)
            {
                SetRefreshToken(_service.RefreshToken());

                return Ok(response.Data);
            }

            throw response.Exception;
        }

        /// <summary>
        /// Обновление refresh токена и JWT-токена
        /// </summary>
        /// <returns>Новый JWT-токен</returns>
        [HttpPost]
        [Authorize]
        public ActionResult<string> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var currentRefreshToken = _service.GetRefreshToken();

            if (!currentRefreshToken.Token.Equals(refreshToken))
                return Unauthorized("Токен невалиден");
            else if (currentRefreshToken.Expires < DateTime.UtcNow)
                return Unauthorized("Срок действия токена истек");

            var newRefreshToken = _service.RefreshToken();
            SetRefreshToken(newRefreshToken);

            var jwt = _service.CreateJwtToken();

            return Ok(jwt);
        }

        /// <summary>
        /// Проверка аутентификации
        /// </summary>
        /// <returns>Успешность аутентификации</returns>
        [HttpGet]
        [Authorize]
        public ActionResult<bool> CheckIfAuthorized()
        {
            return Ok(true);
        }

        /// <summary>
        /// Проверка наличия прав администратора
        /// </summary>
        /// <returns>Наличие прав администратора</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<bool> CheckAdminPrivs()
        {
            return Ok(true);
        }

        /// <summary>
        /// Проверка наличия прав администратора или модератора
        /// </summary>
        /// <returns>Наличие прав администратора или модератора</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult<bool> CheckAdminOrModerPrivs()
        {
            return Ok(true);
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
        }
    }
}
