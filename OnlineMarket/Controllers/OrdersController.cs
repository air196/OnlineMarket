using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using OnlineMarket.Domain.Dto;
using OnlineMarket.Domain.Models;
using System.ComponentModel.DataAnnotations;
using OnlineMarket.Services;
using Microsoft.AspNetCore.Authorization;

namespace OnlineMarket.Controllers
{
    /// <summary>
    /// Управление заказами
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class OrdersController : Controller
    {
        private readonly OnlineMarketService _service;
        public OrdersController(OnlineMarketService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получение списка товаров каталога
        /// </summary>
        /// <param name="loadOptions">Параметры загрузки</param>
        /// <returns>Список товаров</returns>
        [HttpPost]
        public async Task<ActionResult<LoadResult>> GetProductsList([FromBody] DataSourceLoadOptionsBase loadOptions)
        {
            var response = await _service.GetProductsListAsync(loadOptions);

            if (response.StatusCode == StatusCodes.Status200OK)
                return Ok(response.Data);

            throw response.Exception;
        }

        /// <summary>
        /// Оформление заказа
        /// </summary>
        /// <param name="order">Информация о заказе</param>
        /// <returns>Заказ</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Order>> CreateOrder([Required][FromBody] OrderSaveDto order)
        {
            if (!ModelState.IsValid)
                throw new Exception(ModelState.First().Value.Errors.First().ErrorMessage);

            var response = await _service.CreateOrder(order);

            if (response.StatusCode == StatusCodes.Status200OK)
                return Ok(response.Data);

            throw response.Exception;
        }

        /// <summary>
        /// Список заказов
        /// </summary>
        /// <param name="loadOptions">Параметры загрузки</param>
        /// <returns>Список заказов</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<LoadResult>> GetOrdersList([FromBody] DataSourceLoadOptionsBase loadOptions)
        {
            var response = await _service.GetOrdersList(loadOptions);

            if (response.StatusCode == StatusCodes.Status200OK)
                return Ok(response.Data);

            throw response.Exception;
        }

        /// <summary>
        /// Сумма заказа к оплате
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Сумма заказа к оплате</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<double>> GetOrderTotalPrice([Required] long orderId)
        {
            var response = await _service.GetOrderTotalPrice(orderId);

            if (response.StatusCode == StatusCodes.Status200OK)
                return Ok(response.Data);

            throw response.Exception;
        }
    }
}
