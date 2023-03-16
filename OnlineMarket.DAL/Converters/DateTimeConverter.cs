using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace OnlineMarket.DAL.Converters
{
    public class DateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        private static Expression<Func<DateTime, DateTime>> ToProviderExpression { get; } =
            (x) => DateTime.SpecifyKind(x, DateTimeKind.Utc);

        private static Expression<Func<DateTime, DateTime>> FromProviderExpression { get; } =
            (x) => DateTime.SpecifyKind(x, DateTimeKind.Utc);

        public DateTimeConverter() : base(ToProviderExpression, FromProviderExpression)
        { }
    }
}
