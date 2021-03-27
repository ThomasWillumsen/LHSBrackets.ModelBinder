using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LHSBrackets.ModelBinder.EF
{
    public static class LinqFilterExpressionBuilder
    {
        public static IQueryable<TEntity> ApplyFilters<TEntity, TKey>(
            this IQueryable<TEntity> source,
            Expression<Func<TEntity, TKey>> selector,
            FilterOperations<TKey> filters)
        {
            var filterExpressions = new List<Expression<Func<TEntity, bool>>>();

            filterExpressions.AddRange(CreateFilters<TEntity, TKey>(selector, filters));
            foreach (var expression in filterExpressions)
            {
                source = source.Where(expression);
            }

            return source;

        }

        private static List<Expression<Func<TEntity, bool>>> CreateFilters<TEntity, TKey>(Expression<Func<TEntity, TKey>> selector, FilterOperations<TKey> filters)
        {
            var expressions = new List<Expression<Func<TEntity, bool>>>();

            if (filters.EQ != null) expressions.Add(CreateBasicExpression(Expression.Equal, selector, filters.EQ));
            if (filters.NE != null) expressions.Add(CreateBasicExpression(Expression.NotEqual, selector, filters.NE));
            if (filters.GT != null) expressions.Add(CreateBasicExpression(Expression.GreaterThan, selector, filters.GT));
            if (filters.GTE != null) expressions.Add(CreateBasicExpression(Expression.GreaterThanOrEqual, selector, filters.GTE));
            if (filters.LT != null) expressions.Add(CreateBasicExpression(Expression.LessThan, selector, filters.LT));
            if (filters.LTE != null) expressions.Add(CreateBasicExpression(Expression.LessThanOrEqual, selector, filters.LTE));
            if (filters.IN?.Any() == true) expressions.Add(CreateContainsExpression(selector, filters.IN));
            if (filters.NIN?.Any() == true) expressions.Add(CreateContainsExpression(selector, filters.NIN, true));

            return expressions;
        }

        private static Expression<Func<TEntity, bool>> CreateContainsExpression<TEntity, TKey>(
            Expression<Func<TEntity, TKey>> selector,
            List<TKey> values,
            bool invert = false)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var parameterName = GetParameterName(selector);
            Expression left = Expression.Property(parameter, parameterName);
            left = Expression.Convert(left, typeof(TKey)); // this is necessary in case TKey is nullable and property isnt
            Expression right = Expression.Constant(values);

            var containsMethodRef = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                   .Single(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);
            MethodInfo containsMethod = containsMethodRef.MakeGenericMethod(typeof(TKey));

            Expression containsExpression = Expression.Call(containsMethod, right, left);
            if (invert == true) containsExpression = Expression.Not(containsExpression);

            var lambdaExpression = Expression.Lambda<Func<TEntity, bool>>(containsExpression, parameter);
            return lambdaExpression;
        }

        private static Expression<Func<TEntity, bool>> CreateBasicExpression<TEntity, TKey>(
            Func<Expression, Expression, Expression> expressionOperator,
            Expression<Func<TEntity, TKey>> selector,
            TKey value)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var parameterName = GetParameterName(selector);
            Expression left = Expression.Property(parameter, parameterName);
            // we need to convert stuff to the same type so they are aligned
            left = Expression.Convert(left, typeof(TKey));
            Expression right = Expression.Constant(value, typeof(TKey));

            var finalExpression = expressionOperator.Invoke(left, right);
            var lambdaExpression = Expression.Lambda<Func<TEntity, bool>>(finalExpression, parameter);
            return lambdaExpression;
        }

        private static string GetParameterName<TEntity, TKey>(Expression<Func<TEntity, TKey>> expression)
        {
            // this is the case for datetimes, enums and possibly others. Possibly navigation properties.
            if (!(expression.Body is MemberExpression memberExpression))
            {
                memberExpression = (MemberExpression)((UnaryExpression)expression.Body).Operand;
            }

            return memberExpression.ToString().Substring(2);
        }

        private static bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static T Cast<T>(object o)
        {
            return (T)o;
        }
    }
}