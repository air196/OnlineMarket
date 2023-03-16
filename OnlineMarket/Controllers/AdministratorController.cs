using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineMarket.Domain.Models;
using OnlineMarket.Services;
using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.Controllers
{
    /// <summary>
    /// Администрирование
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class AdministratorController : Controller
    {
        private readonly OnlineMarketService _service;
        public AdministratorController(OnlineMarketService service)
        {
            _service = service;
        }

#if DEBUG
        /// <summary>
        /// Инициализация БД
        /// </summary>
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> InitDatabase()
        {
            var response = await _service.InitDatabaseAsync();

            if (response.StatusCode == StatusCodes.Status200OK)
                return Ok(response.Data);

            throw response.Exception;
        }
#endif

        /// <summary>
        /// Добавление нового товара в каталог
        /// </summary>
        /// <param name="productName">Наименование</param>
        /// <param name="price">Цена</param>
        /// <returns>Товар</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<Product>> AddProduct([Required] string productName, [Required] double price)
        {
            var response = await _service.AddProductAsync(productName, price);

            if (response.StatusCode == StatusCodes.Status200OK)
                return Ok(response.Data);

            throw response.Exception;
        }

        /// <summary>
        /// Редактирование информации о товаре из каталога
        /// </summary>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="name">Новое наименование</param>
        /// <param name="price">Новая цена</param>
        /// <returns>Товар</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult> UpdateProduct([Required] long productId, string name, double? price = null)
        {
            var response = await _service.UpdateProductAsync(productId, name, price);

            if (response.StatusCode == StatusCodes.Status200OK)
                return Ok(response.Data);

            throw response.Exception;
        }

        /// <summary>
        /// Добавление промокода
        /// </summary>
        /// <param name="promocode">Промокод</param>
        /// <param name="discountPercent">Скидка (%)</param>
        /// <param name="discountSum">Скидка (руб.)</param>
        /// <param name="minOrderCost">Минимальная сумма заказа</param>
        /// <param name="activeFrom">Действует с</param>
        /// <param name="activeTo">Действует по</param>
        /// <returns>Промокод</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult> AddPromocode(
            [Required] string promocode,
            long? discountPercent,
            double? discountSum,
            double minOrderCost,
            DateTime? activeFrom,
            DateTime? activeTo)
        {
            var response = await _service.AddPromocodeAsync(promocode, discountPercent, discountSum, minOrderCost, activeFrom, activeTo);

            if (response.StatusCode == StatusCodes.Status200OK)
                return Ok(response.Data);

            throw response.Exception;
        }

        /// <summary>
        /// Получение списка промокодов
        /// </summary>
        /// <param name="onlyActive">Только активные на данный момент</param>
        /// <param name="loadOptions">Параметры загрузки</param>
        /// <returns>Список промокодов</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<LoadResult>> GetPromocodesList([FromBody] DataSourceLoadOptionsBase loadOptions, [FromHeader] bool onlyActive)
        {
            var response = await _service.GetPromocodesListAsync(onlyActive, loadOptions);

            if (response.StatusCode == StatusCodes.Status200OK)
                return Ok(response.Data);

            throw response.Exception;
        }

        /// <summary>
        /// Проверка действительности промокода
        /// </summary>
        /// <param name="promocode">Промокод</param>
        /// <returns>Действителен ли промокод</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<bool>> CheckPromocode([Required] string promocode)
        {
            var response = await _service.CheckPromocodeAsync(promocode);

            if (response.StatusCode == StatusCodes.Status200OK)
                return Ok(response.Data);

            throw response.Exception;
        }
    }
}
