using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineMarket.Domain.Models
{
    /// <summary>
    /// Лог
    /// </summary>
    public class Audit
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public long Id { get; set; }
        /// <summary>
        /// Идентификатор транзакции
        /// </summary>
        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public long TransactionId { get; set; }
        /// <summary>
        /// Идентификатор сущности
        /// </summary>
        [Required]
        public long EntityId { get; set; }
        /// <summary>
        /// Имя таблицы
        /// </summary>
        [Required]
        public string TableName { get; set; }
        /// <summary>
        /// Имя столбца
        /// </summary>
        [Required]
        public string ColumnName { get; set; }
        /// <summary>
        /// Предыдущее значение
        /// </summary>
        public string OldValue { get; set; }
        /// <summary>
        /// Новое значение
        /// </summary>
        public string NewValue { get; set; }
        /// <summary>
        /// Действие
        /// </summary>
        [Required]
        public string Action { get; set; }
        /// <summary>
        /// Дата и время действия
        /// </summary>
        [Required]
        public DateTime Tmstamp { get; set; }
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public User User { get; set; }
    }
}
