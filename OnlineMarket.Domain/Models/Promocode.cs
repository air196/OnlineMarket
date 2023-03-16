using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineMarket.Domain.Models
{
    /// <summary>
    /// Промокод
    /// </summary>
    public class Promocode
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Промокод
        /// </summary>
        public string PromoCode { get; set; }
        /// <summary>
        /// Скидка (%)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [Range(double.Epsilon, 100d, ErrorMessage = "Размер скидки указан неверно")]
        public long? DiscountPercent { get; set; }
        /// <summary>
        /// Скидка (руб.)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Размер скидки указан неверно")]
        public double? DiscountSum { get; set; }
        /// <summary>
        /// Действует с
        /// </summary>
        public DateTime ActiveFrom { get; set; }
        /// <summary>
        /// Действует по
        /// </summary>
        public DateTime ActiveTo { get; set; }
        /// <summary>
        /// Минимальная сумма заказа
        /// </summary>
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Минимальная сумма заказа указана неверно")]
        public double? MinimumOrderCost { get; set; }

        /// <summary>
        /// Действует ли на данный момент
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public bool IsActiveNow => ActiveFrom <= DateTime.Now.Date && ActiveTo >= DateTime.Now.Date;

        /// <summary>
        /// Заказы
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    }
}
