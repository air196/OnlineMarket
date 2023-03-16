using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Domain.Dto
{
    /// <summary>
    /// Заказанный товар
    /// </summary>
    public class OrderedProductDto
    {
        /// <summary>
        /// Идентификатор товара
        /// </summary>
        [Required]
        public long ProductId { get; set; }
        /// <summary>
        /// Кол-во товара в заказе
        /// </summary>
        [Required]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Количество товара указано неверно")]
        public double Count { get; set; }
    }
}
