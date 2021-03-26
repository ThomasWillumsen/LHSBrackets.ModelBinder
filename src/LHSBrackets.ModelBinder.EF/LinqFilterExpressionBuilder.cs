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
            Expression<Func<TEntity, Nullable<TKey>>> selector,
            FilterOperations<TKey> filters)
            where TKey : struct
        {
            var filterExpressions = new List<Expression<Func<TEntity, bool>>>();

            filterExpressions.AddRange(CreateFilters<TEntity, TKey>(selector, filters));
            foreach (var expression in filterExpressions)
            {
                source = source.Where(expression);
            }

            return source;

        }

        private static List<Expression<Func<TEntity, bool>>> CreateFilters<TEntity, TKey>(Expression<Func<TEntity, Nullable<TKey>>> selector, FilterOperations<TKey> filters)
            where TKey : struct
        {
            var expressions = new List<Expression<Func<TEntity, bool>>>();
            var parameter = Expression.Parameter(typeof(TEntity));
            Action<Expression> addExp = (Expression expression) =>
            {
                expressions.Add(Expression.Lambda<Func<TEntity, bool>>(expression, parameter));
            };

            var expParam = Expression.Property(parameter, GetParameterName(selector));

            if (filters.EQ != null) expressions.Add(CreateLambdaExpression(Expression.Equal, selector, filters.EQ.Value));
            if (filters.NE != null) expressions.Add(CreateLambdaExpression(Expression.NotEqual, selector, filters.NE.Value));
            if (filters.GT != null) expressions.Add(CreateLambdaExpression(Expression.GreaterThan, selector, filters.GT.Value));
            if (filters.GTE != null) expressions.Add(CreateLambdaExpression(Expression.GreaterThanOrEqual, selector, filters.GTE.Value));
            if (filters.LT != null) expressions.Add(CreateLambdaExpression(Expression.LessThan, selector, filters.LT.Value));
            if (filters.LTE != null) expressions.Add(CreateLambdaExpression(Expression.LessThanOrEqual, selector, filters.LTE.Value));
            if (filters.IN?.Any() == true) expressions.Add(CreateContainsLambdaExpression(selector, filters.IN));
            if (filters.NIN?.Any() == true) expressions.Add(CreateContainsLambdaExpression(selector, filters.NIN, true));

            return expressions;
        }

        private static Expression<Func<TEntity, bool>> CreateLambdaExpression<TEntity, TKey>(
            Func<Expression, Expression, Expression> expressionOperator,
            Expression<Func<TEntity, Nullable<TKey>>> selector,
            TKey value) where TKey : struct
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var left = Expression.Property(parameter, GetParameterName(selector));

            var expression = CreateBasicExpression(expressionOperator, left, value);
            var lambdaExpression = Expression.Lambda<Func<TEntity, bool>>(expression, parameter);

            return lambdaExpression;
        }

        private static Expression<Func<TEntity, bool>> CreateContainsLambdaExpression<TEntity, TKey>(
            Expression<Func<TEntity, Nullable<TKey>>> selector,
            List<TKey> values,
            bool invert = false) where TKey : struct
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var left = Expression.Property(parameter, GetParameterName(selector));

            var expression = CreateContainsExpression(left, values);
            if (invert == true) expression = Expression.Not(expression);
            var lambdaExpression = Expression.Lambda<Func<TEntity, bool>>(expression, parameter);

            return lambdaExpression;
        }

        private static string GetParameterName<TEntity, TKey>(Expression<Func<TEntity, TKey>> expression)
        {
            if (!(expression.Body is MemberExpression memberExpression))
            {
                memberExpression = (MemberExpression)((UnaryExpression)expression.Body).Operand;
            }

            return memberExpression.ToString().Substring(2);
        }

        private static Expression CreateContainsExpression<TKey>(
            Expression left,
            List<TKey> values) where TKey : struct
        {
            var containsMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                   .Single(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);

            var contains = containsMethod.MakeGenericMethod(typeof(TKey));

            var rightUnderlyingType = values.GetType().GetGenericArguments()[0];

            if (IsNullableType(left.Type))
                left = Expression.Convert(left, Nullable.GetUnderlyingType(left.Type)!);

            if (IsNullableType(rightUnderlyingType))
                values = values.Select(x => (TKey)x).ToList();

            var containsExpression = Expression.Call(
                contains,
                Expression.Constant(values),
                left);

            return containsExpression;
        }
        private static Expression CreateBasicExpression<TKey>(
            Func<Expression, Expression, Expression> expressionOperator,
            Expression left, TKey value) where TKey : struct
        {
            Expression right;
            if (IsNullableType(left.Type) && !IsNullableType(typeof(TKey)))
            {

                right = Expression.Constant(value, typeof(Nullable<TKey>));
            }
            else
            {
                right = Expression.Constant(value, typeof(TKey));
            }

            if (!IsNullableType(left.Type) && IsNullableType(right.Type))
            {
                left = Expression.Convert(left, typeof(TKey));
            }

            return expressionOperator.Invoke(left, right);
        }

        private static bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}