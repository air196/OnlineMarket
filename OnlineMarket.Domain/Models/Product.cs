using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineMarket.Domain.Models
{
    public class Product
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required]
        public long Id { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        [Required]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Цена товара указана неверно")]
        public double CurrentPrice { get; set; }

        [JsonIgnore]
        public IEnumerable<ProductOrder> ProductsOrders { get; set; } = new List<ProductOrder>();
    }
}
