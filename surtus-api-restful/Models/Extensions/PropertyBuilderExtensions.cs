using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace surtus_api_restful.Models
{
    internal static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<string> IsRequiredVariableLengthString(this PropertyBuilder<string> property, int maxLength, bool isUnicode = true)
            => property.IsRequired().IsUnicode(isUnicode).HasMaxLength(maxLength);
        public static PropertyBuilder<string> IsRequiredVariableLengthString(this PropertyBuilder<string> property, bool isUnicode = true)
            => property.IsRequired().IsUnicode(isUnicode);
        public static PropertyBuilder<string> IsVariableLengthString(this PropertyBuilder<string> property, int maxLength, bool isUnicode = true)
            => property.IsUnicode(isUnicode).HasMaxLength(maxLength);
        public static PropertyBuilder<string> IsVariableLengthString(this PropertyBuilder<string> property, bool isUnicode = true)
            => property.IsUnicode(isUnicode);

        public static PropertyBuilder<string> IsRequiredFixedLengthString(this PropertyBuilder<string> property, int maxLength, bool isUnicode = true)
            => property.IsRequiredVariableLengthString(maxLength, isUnicode).IsFixedLength();
        public static PropertyBuilder<string> IsRequiredFixedLengthString(this PropertyBuilder<string> property, bool isUnicode = true)
            => property.IsRequiredVariableLengthString(isUnicode).IsFixedLength();
        public static PropertyBuilder<string> IsFixedLengthString(this PropertyBuilder<string> property, int maxLength, bool isUnicode = true)
            => property.IsVariableLengthString(maxLength, isUnicode).IsFixedLength();
        public static PropertyBuilder<string> IsFixedLengthString(this PropertyBuilder<string> property, bool isUnicode = true)
            => property.IsVariableLengthString(isUnicode).IsFixedLength();

        public static PropertyBuilder<decimal> HasPrecision(this PropertyBuilder<decimal> property, int precision, int scale)
          => property.HasColumnType($"decimal({precision},{scale})");
        public static PropertyBuilder<decimal?> HasPrecision(this PropertyBuilder<decimal?> property, int precision, int scale)
            => property.HasColumnType($"decimal({precision},{scale})");
    }
}
