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

        // if more nullable generic issues arrise Expression.Convert may be worth looking into.
        private static Expression<Func<TEntity, bool>> CreateContainsExpression<TEntity, TKey>(
            Expression<Func<TEntity, TKey>> selector,
            List<TKey> values,
            bool invert = false)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var parameterName = GetParameterName(selector);
            Expression leftExp = Expression.Property(parameter, parameterName);


            var containsMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                   .Single(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);
            MethodInfo contains = containsMethod.MakeGenericMethod(typeof(TKey));

            var underlyingTKeyType = Nullable.GetUnderlyingType(typeof(TKey)) ?? typeof(TKey);
            var rightListItemType = values.GetType().GetGenericArguments()[0];
            Expression rightExp = Expression.Constant(values);

            if (IsNullableType(typeof(TKey)))
            {
                if (!IsNullableType(leftExp.Type))
                    leftExp = Expression.Convert(leftExp, typeof(Nullable<>).MakeGenericType(leftExp.Type));

                if (!IsNullableType(rightListItemType))
                {
                    rightExp = Expression.Constant(values, typeof(Nullable<>).MakeGenericType(underlyingTKeyType));
                }

            }
            else
            {
                if (IsNullableType(leftExp.Type))
                    leftExp = Expression.Convert(leftExp, Nullable.GetUnderlyingType(leftExp.Type)!);

                if (IsNullableType(rightListItemType))
                    rightExp = Expression.Constant(values);
            }

            Expression containsExpression = Expression.Call(
                contains,
                rightExp,
                leftExp);

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

            Expression right;
            if (IsNullableType(left.Type) && !IsNullableType(typeof(TKey)))
            {

                right = Expression.Constant(value, typeof(Nullable<>).MakeGenericType(typeof(TKey)));
            }
            else
            {
                right = Expression.Constant(value, typeof(TKey));
            }

            if (!IsNullableType(left.Type) && IsNullableType(right.Type))
            {
                left = Expression.Convert(left, typeof(TKey));
            }

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
    }
}