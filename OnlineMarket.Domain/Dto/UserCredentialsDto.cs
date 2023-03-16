using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Domain.Dto
{
    /// <summary>
    /// Данные для аутентификации
    /// </summary>
    public class UserCredentialsDto
    {
        /// <summary>
        /// Логин
        /// </summary>
        [Required]
        public string Login { get; set; }
        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
