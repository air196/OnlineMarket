using Microsoft.EntityFrameworkCore;
using OnlineMarket.Domain.Models;
using OnlineMarket.Domain.Response;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Data;
using System.ComponentModel.DataAnnotations;
using OnlineMarket.Domain.Dto;
using OnlineMarket.Domain.Enums;

namespace OnlineMarket.Services
{
    // Управление заказами
    public partial class OnlineMarketService
    {
        /// <summary>
        /// Оформление заказа
        /// </summary>
        /// <param name="order">Информация о заказе</param>
        /// <returns>Заказ</returns>
        public async Task<Response<Order>> CreateOrder([Required] OrderSaveDto order)
        {
            var response = new Response<Order>();
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == _userId);
                if (user == null)
                    throw new Exception("Пользователь не найден");

                var promo = await _context.Promocodes.FirstOrDefaultAsync(p => p.PromoCode.ToLower() == order.Promocode.ToLower());
                if (!string.IsNullOrEmpty(order.Promocode))
                {
                    if (promo == null)
                        throw new Exception("Промокод не найден");
                    if (!promo.IsActiveNow)
                        throw new Exception("Промокод недействителен");
                }

                var productsIds = order.OrderedProducts.Select(p => p.ProductId).Distinct().ToList();
                var products = await _context.Products.Where(p => productsIds.Contains(p.Id)).ToListAsync();
                if (productsIds.Count != products.Count)
                    throw new Exception("Некоторые товары не найдены в каталоге");

                var addedOrder = await _context.Orders.AddAsync(new Order()
                {
                    UserId = _userId,
                    AddressToDeliver = order.AddressToDeliver,
                    Comment = order.Comment,
                    Status = (long)OrderStatus.Created,
                    CreateDate = DateTime.Now,
                    DeliverBy = order.DeliverBy,
                    PromocodeId = promo?.Id ?? null
                });
                var orderEntity = addedOrder.Entity;
                
                var orderedProducts = new List<ProductOrder>();
                foreach(var op in order.OrderedProducts)
                {
                    orderedProducts.Add(new ProductOrder()
                    {
                        Count = op.Count,
                        ProductId = op.ProductId,
                        Price = products.Single(p => p.Id == op.ProductId).CurrentPrice
                    });
                }
                orderEntity.ProductsOrders = orderedProducts;

                await SaveChangesAsync();
                response.SetOk(addedOrder.Entity);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                response.SetInternalError(ex);
            }

            return response;
        }

        /// <summary>
        /// Список заказов
        /// </summary>
        /// <param name="loadOptions">Параметры загрузки</param>
        /// <returns>Список заказов</returns>
        public async Task<Response<LoadResult>> GetOrdersList(DataSourceLoadOptionsBase loadOptions)
        {
            var response = new Response<LoadResult>();
            try
            {
                var orders = _context.Orders
                    .Where(o => o.UserId == _userId)
                    .Include(o => o.User)
                    .Include(o => o.Promocode)
                    .Include(o => o.ProductsOrders).ThenInclude(o => o.Product);

                response.SetOk(await DataSourceLoader.LoadAsync(orders, loadOptions ?? new DataSourceLoadOptionsBase()));
            }
            catch (Exception ex)
            {
                response.SetInternalError(ex);
            }

            return response;
        }

        /// <summary>
        /// Сумма заказа к оплате
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Сумма заказа к оплате</returns>
        public async Task<Response<double>> GetOrderTotalPrice([Required] long orderId)
        {
            var response = new Response<double>();
            try
            {
                var order = await _context.Orders
                    .Where(o => o.UserId == _userId)
                    .Include(o => o.ProductsOrders)
                    .Include(o => o.Promocode)
                    .SingleOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                    throw new Exception("Заказ не найден");

                response.SetOk(order.TotalSum);
            }
            catch (Exception ex)
            {
                response.SetInternalError(ex);
            }

            return response;
        }
    }
}
