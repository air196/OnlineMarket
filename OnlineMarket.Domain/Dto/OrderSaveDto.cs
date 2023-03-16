using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Domain.Dto
{
    /// <summary>
    /// Заказ
    /// </summary>
    public class OrderSaveDto
    {
        /// <summary>
        /// Адрес доставки
        /// </summary>
        [Required]
        public string AddressToDeliver { get; set; }
        /// <summary>
        /// Доставить к
        /// </summary>
        [Required]
        public DateTime DeliverBy { get; set; }
        /// <summary>
        /// Промокод
        /// </summary>
        public string Promocode { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Товары в заказе
        /// </summary>
        [Required]
        public IEnumerable<OrderedProductDto> OrderedProducts { get; set; } = new List<OrderedProductDto>();
    }
}
