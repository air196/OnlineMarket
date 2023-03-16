using OnlineMarket.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineMarket.Domain.Models
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required]
        public long Id { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        public string FirstName { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
        public string LastName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        [Required]
        public string SecondName { get; set; }
        /// <summary>
        /// Телефон
        /// </summary>
        [Required]
        public string Phone { get; set; }
        /// <summary>
        /// Электронная почта
        /// </summary>
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+(\.[A-Za-z]{2,4}){1,2}", ErrorMessage = "Электронная почта указана неверно")]
        public string Email { get; set; }
        /// <summary>
        /// Логин
        /// </summary>
        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string Login { get; set; }
        /// <summary>
        /// Пароль в зашиврованном виде
        /// </summary>
        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string Password { get; set; }
        /// <summary>
        /// Роль
        /// </summary>
        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public UserRole Role { get; set; }

        /// <summary>
        /// Заказы
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
        /// <summary>
        /// Логи
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public IEnumerable<Audit> Logs { get; set; } = new List<Audit>();
    }
}
