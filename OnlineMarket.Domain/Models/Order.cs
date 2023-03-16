using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineMarket.Domain.Models
{
    /// <summary>
    /// Заказ
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required]
        public long Id { get; set; }
        /// <summary>
        /// Идентификатор пользователя, оформившего заказ
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        [Required]
        public long UserId { get; set; }
        /// <summary>
        /// Дата заказа
        /// </summary>
        [Required]
        public DateTime CreateDate { get; set; }
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
        /// Статус заказа
        /// </summary>
        [Required]
        public long Status { get; set; }
        /// <summary>
        /// Идентификатор промокода
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public long? PromocodeId { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Общая сумма заказа (с учетом скидок)
        /// </summary>
        [NotMapped]
        public double TotalSum
        {
            get
            {
                var totalSum = 0d;

                totalSum = ProductsOrders.Sum(po => po.Count * po.Price);

                if (Promocode != null
                    && Promocode.ActiveFrom.Date <= CreateDate
                    && Promocode.ActiveTo.Date >= CreateDate
                    && totalSum >= Promocode.MinimumOrderCost)
                {
                    if (Promocode.DiscountPercent.HasValue && Promocode.DiscountPercent.Value > 0)
                        totalSum *= (100d - Promocode.DiscountPercent.Value) / 100;
                    else if (Promocode.DiscountSum.HasValue && Promocode.DiscountSum.Value > 0)
                    {
                        totalSum -= Promocode.DiscountSum.Value;
                        if (totalSum < 0)
                            totalSum = 0;
                    }
                }

                return Math.Round(totalSum, 2);
            }

            //set { return; }
        }

        /// <summary>
        /// Пользователь, оформивший заказ
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Промокод
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Promocode? Promocode { get; set; }
        public IEnumerable<ProductOrder> ProductsOrders { get; set; } = new List<ProductOrder>();
    }
}
