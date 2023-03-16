using Microsoft.EntityFrameworkCore;
using OnlineMarket.Domain.Models;
using OnlineMarket.Domain.Response;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Data;
using System.ComponentModel.DataAnnotations;
using OnlineMarket.Domain.Helpers;

namespace OnlineMarket.Services
{
    // Администрирование
    public partial class OnlineMarketService
    {
        /// <summary>
        /// Инициализация схемы БД и заполнение тестовых данных
        /// </summary>
        public async Task<Response<int>> InitDatabaseAsync()
        {
            var response = new Response<int>();
            try
            {
                await _context.Database.EnsureCreatedAsync();

                await _context.Users.AddAsync(new User()
                {
                    FirstName = "Петр",
                    SecondName = "Евгеньевич",
                    LastName = "Савельев",
                    Phone = "+79993334455",
                    Email = "savelev_pe@onlinemarket.ru",
                    Login = "savelev_pe",
                    Password = Sha256Helper.GetHash("savelev123"),
                    Role = Domain.Enums.UserRole.Admin
                });
                await _context.Users.AddAsync(new User()
                {
                    FirstName = "Андрей",
                    SecondName = "Петрович",
                    LastName = "Морозов",
                    Phone = "+79875552233",
                    Email = "morozov_ap@mail.ru",
                    Login = "morozov_ap",
                    Password = Sha256Helper.GetHash("morozov5456"),
                    Role = Domain.Enums.UserRole.User
                });

                await _context.Products.AddAsync(new Product() { Name = "сахар 1кг", CurrentPrice = 49.9 });
                await _context.Products.AddAsync(new Product() { Name = "картофель 1кг", CurrentPrice = 39.9 });
                await _context.Products.AddAsync(new Product() { Name = "квас 2л", CurrentPrice = 89.9 });

                await _context.Promocodes.AddAsync(new Promocode()
                {
                    PromoCode = "ПРИВЕТ",
                    DiscountPercent = 15,
                    MinimumOrderCost = 1000,
                    ActiveFrom = DateTime.Now.Date,
                    ActiveTo = new DateTime(2023, 12, 31)
                });

                var result = await _context.SaveChangesWithLogsAsync(1);

                response.SetOk(result);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                response.SetInternalError(ex);
            }

            return response;
        }

        /// <summary>
        /// Добавление товара в каталог
        /// </summary>
        /// <param name="productName">Наименование</param>
        /// <param name="price">Цена</param>
        /// <returns>Товар</returns>
        public async Task<Response<Product>> AddProductAsync([Required] string productName, [Required] double price)
        {
            var response = new Response<Product>();
            try
            {
                var product = await _context.Products.AddAsync(new Product()
                {
                    Name = productName,
                    CurrentPrice = price
                });
                await SaveChangesAsync();

                response.SetOk(product.Entity);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                response.SetInternalError(ex);
            }

            return response;
        }

        /// <summary>
        /// Получение списка товаров из каталога
        /// </summary>
        /// <param name="loadOptions">Параметры загрузки</param>
        /// <returns>Список товаров</returns>
        public async Task<Response<LoadResult>> GetProductsListAsync(DataSourceLoadOptionsBase loadOptions)
        {
            var response = new Response<LoadResult>();
            try
            {
                var products = _context.Products.OrderBy(p => p.Id);
                response.SetOk(await DataSourceLoader.LoadAsync(products, loadOptions ?? new DataSourceLoadOptionsBase()));
            }
            catch (Exception ex)
            {
                response.SetInternalError(ex);
            }

            return response;
        }

        /// <summary>
        /// Редактирование информации о товаре из каталога
        /// </summary>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="name">Новое наименование</param>
        /// <param name="price">Новая цена</param>
        /// <returns>Товар</returns>
        public async Task<Response<Product>> UpdateProductAsync([Required] long productId, string name, double? price = null)
        {
            var response = new Response<Product>();
            try
            {
                var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                    throw new Exception($"Товар с идентификатором {productId} не найден");

                if (!string.IsNullOrEmpty(name))
                    product.Name = name;

                if (price.HasValue)
                    product.CurrentPrice = price.Value;

                await SaveChangesAsync();

                response.SetOk(product);
            }
            catch (Exception ex)
            {
                response.SetInternalError(ex);
            }

            return response;
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
        public async Task<Response<Promocode>> AddPromocodeAsync(
            [Required] string promocode,
            long? discountPercent,
            double? discountSum,
            double minOrderCost,
            DateTime? activeFrom,
            DateTime? activeTo)
        {
            var response = new Response<Promocode>();
            try
            {
                var discountPercentSet = discountPercent.HasValue && discountPercent.Value > 0 && discountPercent.Value < 100;
                var discountSumSet = discountSum.HasValue && discountSum.Value > 0;

                if (!discountPercentSet && !discountSumSet)
                    throw new Exception("Параметры заданы неверно");

                var promo = await _context.Promocodes.FirstOrDefaultAsync(p => p.PromoCode.ToLower() == promocode.ToLower());

                if (promo != null)
                    throw new Exception("Промокод уже существует");
                var currentDate = DateTime.Now.Date;

                var newPromo = await _context.Promocodes.AddAsync(new Promocode()
                {
                    PromoCode = promocode.ToUpper(),
                    DiscountPercent = discountPercentSet ? discountPercent.Value : 0,
                    DiscountSum = discountPercentSet ? 0 : discountSum.Value,
                    MinimumOrderCost = minOrderCost,
                    ActiveFrom = activeFrom.HasValue ? activeFrom.Value : currentDate,
                    ActiveTo = activeTo.HasValue ? activeTo.Value : currentDate.AddDays(-currentDate.DayOfYear).AddYears(1)
                });
                await SaveChangesAsync();

                response.SetOk(newPromo.Entity);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                response.SetInternalError(ex);
            }

            return response;
        }

        /// <summary>
        /// Получение списка промокодов
        /// </summary>
        /// <param name="onlyActive">Только активные на данный момент</param>
        /// <param name="loadOptions">Параметры загрузки</param>
        /// <returns>Список промокодов</returns>
        public async Task<Response<LoadResult>> GetPromocodesListAsync(bool onlyActive, DataSourceLoadOptionsBase loadOptions)
        {
            var response = new Response<LoadResult>();
            try
            {
                var promocodes = _context.Promocodes.Where(p => !onlyActive || p.IsActiveNow).OrderBy(p => p.Id);
                response.SetOk(await DataSourceLoader.LoadAsync(promocodes, loadOptions ?? new DataSourceLoadOptionsBase()));
            }
            catch (Exception ex)
            {
                response.SetInternalError(ex);
            }

            return response;
        }

        /// <summary>
        /// Проверка промокода на активность на данный момент
        /// </summary>
        /// <param name="promocode">Промокод</param>
        /// <returns>Активен ли промокод</returns>
        public async Task<Response<bool>> CheckPromocodeAsync([Required] string promocode)
        {
            var response = new Response<bool>();
            try
            {
                var promo = await _context.Promocodes.FirstOrDefaultAsync(p => p.PromoCode.ToLower() == promocode.ToLower());

                if (promo == null)
                    throw new Exception("Промокод не найден");

                response.SetOk(promo.IsActiveNow);
            }
            catch (Exception ex)
            {
                response.SetInternalError(ex);
            }

            return response;
        }
    }
}
