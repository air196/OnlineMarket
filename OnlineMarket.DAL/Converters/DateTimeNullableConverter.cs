using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace OnlineMarket.DAL.Converters
{
    public class DateTimeNullableConverter : ValueConverter<DateTime?, DateTime?>
    {
        private static Expression<Func<DateTime?, DateTime?>> ToProviderExpression { get; } =
            (x) => x.HasValue ? DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) : null;

        private static Expression<Func<DateTime?, DateTime?>> FromProviderExpression { get; } =
            (x) => x.HasValue ? DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) : null;

        public DateTimeNullableConverter() : base(ToProviderExpression, FromProviderExpression)
        { }
    }
}
