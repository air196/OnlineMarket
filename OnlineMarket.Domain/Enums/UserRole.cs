using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Domain.Enums
{
    /// <summary>
    /// Роль пользователя
    /// </summary>
    public enum UserRole
    {
        [Display(Name = "Администратор")]
        Admin = 0,
        [Display(Name = "Модератор")]
        Moderator = 1,
        [Display(Name = "Пользователь")]
        User = 2
    }
}
