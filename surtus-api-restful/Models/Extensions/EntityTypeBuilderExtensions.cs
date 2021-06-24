using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace surtus_api_restful.Models
{
    internal static class EntityTypeBuilderExtensions
    {
        public static void ConfigurarIdentityLongId<T>(this EntityTypeBuilder<T> entity) where T : class, IIdAutogenerado<long>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        }

        public static void ConfigurarIdentityId<T>(this EntityTypeBuilder<T> entity) where T : class, IIdAutogenerado<int>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        }

        public static void ConfigurarIdentityShortId<T>(this EntityTypeBuilder<T> entity) where T : class, IIdAutogenerado<short>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        }

        public static ReferenceCollectionBuilder<Y, T> MapearUnoMuchosUnidireccional<T, Y>(this EntityTypeBuilder<T> entity, Expression<Func<T, Y>> navigationExpression, Expression<Func<T, object>> foreignKeyExpression)
            where T : class
            where Y : class
        {
            entity.HasIndex(foreignKeyExpression);
            return entity.HasOne(navigationExpression).WithMany().HasForeignKey(foreignKeyExpression);
        }

        public static ReferenceCollectionBuilder<Y, T> MapearUnoMuchosBidireccional<T, Y>(this EntityTypeBuilder<T> entity, Expression<Func<T, Y>> navigationExpressionTo, Expression<Func<Y, IEnumerable<T>>> navigationExpressionFrom, Expression<Func<T, object>> foreignKeyExpression)
            where T : class
            where Y : class
        {
            entity.HasIndex(foreignKeyExpression);
            return entity.HasOne(navigationExpressionTo).WithMany(navigationExpressionFrom).HasForeignKey(foreignKeyExpression);
        }

        public static ReferenceCollectionBuilder<T, Y> MapearMuchosUnoBidireccional<T, Y>(this EntityTypeBuilder<T> entity, Expression<Func<T, IEnumerable<Y>>> navigationExpressionTo, Expression<Func<Y, T>> navigationExpressionFrom, Expression<Func<Y, object>> foreignKeyExpression)
            where T : class
            where Y : class
            => entity.HasMany(navigationExpressionTo).WithOne(navigationExpressionFrom).HasForeignKey(foreignKeyExpression);

        public static ReferenceReferenceBuilder<T, Y> MapearUnoUnoUnidireccional<T, Y>(this EntityTypeBuilder<T> entity, Expression<Func<T, Y>> navigationExpression, Expression<Func<T, object>> foreignKeyExpression)
            where T : class
            where Y : class
        {
            entity.HasIndex(foreignKeyExpression);
            return entity.HasOne(navigationExpression).WithOne().HasForeignKey(foreignKeyExpression);
        }

        public static ReferenceReferenceBuilder<T, Y> MapearUnoUnoBidireccional<T, Y>(this EntityTypeBuilder<T> entity, Expression<Func<T, Y>> navigationExpressionTo, Expression<Func<Y, T>> navigationExpressionFrom, Expression<Func<T, object>> foreignKeyExpression)
            where T : class
            where Y : class
        {
            entity.HasIndex(foreignKeyExpression);
            return entity.HasOne(navigationExpressionTo).WithOne(navigationExpressionFrom).HasForeignKey(foreignKeyExpression);
        }
    }
}
