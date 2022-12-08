// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reflection;

namespace DurableTask.Reflection;

/// <summary>
/// Extensions for <see cref="PropertyInfo"/>.
/// </summary>
internal static class PropertyInfoExtensions
{
    /// <summary>
    /// Create a re-usable setter for a <see cref="PropertyInfo"/>.
    /// When cached and reused, This is quicker than using <see cref="PropertyInfo.SetValue(object, object)"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type of the property.</typeparam>
    /// <param name="propertyInfo">The property info.</param>
    /// <param name="targetType">The target type.</param>
    /// <returns>A re-usable action to set the property.</returns>
    public static Action<object, TValue> CreateSetter<TValue>(this PropertyInfo propertyInfo, Type targetType)
    {
        Check.NotNull(propertyInfo);
        ParameterExpression targetExp = Expression.Parameter(typeof(object), "target");
        Expression source = Expression.Convert(targetExp, targetType);
        ParameterExpression valueExp = Expression.Parameter(typeof(TValue), "value");
        MemberExpression fieldExp = Expression.Property(source, propertyInfo);
        BinaryExpression assignExp = Expression.Assign(fieldExp, valueExp);
        return Expression.Lambda<Action<object, TValue>>(assignExp, targetExp, valueExp).Compile();
    }

    /// <summary>
    /// Create a re-usable setter for a <see cref="PropertyInfo"/>.
    /// When cached and reused, This is quicker than using <see cref="PropertyInfo.SetValue(object, object)"/>.
    /// </summary>
    /// <typeparam name="TTarget">The target type of the object.</typeparam>
    /// <typeparam name="TValue">The value type of the property.</typeparam>
    /// <param name="propertyInfo">The property info.</param>
    /// <returns>A re-usable action to set the property.</returns>
    public static Action<TTarget, TValue> CreateSetter<TTarget, TValue>(this PropertyInfo propertyInfo)
    {
        Check.NotNull(propertyInfo);
        ParameterExpression targetExp = Expression.Parameter(typeof(TTarget), "target");
        Expression source = targetExp;

        if (typeof(TTarget) != propertyInfo.DeclaringType)
        {
            source = Expression.Convert(targetExp, propertyInfo.DeclaringType);
        }

        ParameterExpression valueExp = Expression.Parameter(typeof(TValue), "value");
        MemberExpression fieldExp = Expression.Property(source, propertyInfo);
        BinaryExpression assignExp = Expression.Assign(fieldExp, valueExp);
        return Expression.Lambda<Action<TTarget, TValue>>(assignExp, targetExp, valueExp).Compile();
    }

    /// <summary>
    /// Create a re-usable setter for a <see cref="PropertyInfo"/>.
    /// When cached and reused, This is quicker than using <see cref="PropertyInfo.SetValue(object, object)"/>.
    /// </summary>
    /// <typeparam name="TTarget">The target type of the object.</typeparam>
    /// <typeparam name="TValue">The value type of the property.</typeparam>
    /// <param name="propertyInfo">The property info.</param>
    /// <returns>A re-usable action to set the property.</returns>
    public static Func<TTarget, TValue> CreateGetter<TTarget, TValue>(this PropertyInfo propertyInfo)
    {
        Check.NotNull(propertyInfo);
        ParameterExpression targetExp = Expression.Parameter(typeof(TTarget), "target");
        Expression source = targetExp;

        if (typeof(TTarget) != propertyInfo.DeclaringType)
        {
            source = Expression.Convert(targetExp, propertyInfo.DeclaringType);
        }

        MemberExpression fieldExp = Expression.Property(source, propertyInfo);
        return Expression.Lambda<Func<TTarget, TValue>>(fieldExp, targetExp).Compile();
    }
}
