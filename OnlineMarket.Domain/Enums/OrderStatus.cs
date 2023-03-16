using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Domain.Enums
{
    /// <summary>
    /// Статус заказа
    /// </summary>
    public enum OrderStatus
    {
        [Display(Name = "Создан")]
        Created = 0,
        [Display(Name = "Принят оператором")]
        OperatorApproved = 1,
        [Display(Name = "Отклонен оператором")]
        OperatorCancelled = 2,
        [Display(Name = "Отменен пользователем")]
        UserCancelled = 3,
        [Display(Name = "Оплачен")]
        Paid = 4,
        [Display(Name = "Доставляется")]
        Delivering = 5,
        [Display(Name = "Доставлен")]
        Delivered = 6
    }
}
