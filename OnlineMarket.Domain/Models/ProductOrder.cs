using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineMarket.Domain.Models
{
    /// <summary>
    /// Заказанный товар
    /// </summary>
    public class ProductOrder
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public long Id { get; set; }
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public long OrderId { get; set; }
        /// <summary>
        /// Идентификатор товара
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public long ProductId { get; set; }
        /// <summary>
        /// Цена 1 шт. на момент заказа
        /// </summary>
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Цена товара указана неверно")]
        public double Price { get; set; }
        /// <summary>
        /// Кол-во товара в заказе
        /// </summary>
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Количество товара указано неверно")]
        public double Count { get; set; }

        /// <summary>
        /// Заказ
        /// </summary>
        [JsonIgnore]
        public Order Order { get; set; }
        /// <summary>
        /// Продукт
        /// </summary>
        public Product Product { get; set; }
    }
}
