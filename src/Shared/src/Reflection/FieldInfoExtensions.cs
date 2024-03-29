// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reflection;

namespace DurableTask.Reflection;

/// <summary>
/// Extensions for <see cref="FieldInfo"/>.
/// </summary>
internal static class FieldInfoExtensions
{
    /// <summary>
    /// Create a re-usable setter for a <see cref="FieldInfo"/>.
    /// When cached and reused, This is quicker than using <see cref="FieldInfo.SetValue(object, object)"/>.
    /// </summary>
    /// <typeparam name="TTarget">The target type of the object.</typeparam>
    /// <typeparam name="TValue">The value type of the field.</typeparam>
    /// <param name="fieldInfo">The field info.</param>
    /// <returns>A re-usable action to set the field.</returns>
    public static Action<TTarget, TValue> CreateSetter<TTarget, TValue>(this FieldInfo fieldInfo)
    {
        Check.NotNull(fieldInfo);
        ParameterExpression targetExp = Expression.Parameter(typeof(TTarget), "target");
        Expression source = targetExp;

        if (typeof(TTarget) != fieldInfo.DeclaringType)
        {
            source = Expression.Convert(targetExp, fieldInfo.DeclaringType);
        }

        ParameterExpression valueExp = Expression.Parameter(typeof(TValue), "value");
        MemberExpression fieldExp = Expression.Field(source, fieldInfo);
        BinaryExpression assignExp = Expression.Assign(fieldExp, valueExp);
        return Expression.Lambda<Action<TTarget, TValue>>(assignExp, targetExp, valueExp).Compile();
    }

    /// <summary>
    /// Create a re-usable setter for a <see cref="FieldInfo"/>.
    /// When cached and reused, This is quicker than using <see cref="FieldInfo.SetValue(object, object)"/>.
    /// </summary>
    /// <typeparam name="TTarget">The target type of the object.</typeparam>
    /// <typeparam name="TValue">The value type of the field.</typeparam>
    /// <param name="fieldInfo">The field info.</param>
    /// <returns>A re-usable action to set the field.</returns>
    public static Func<TTarget, TValue> CreateGetter<TTarget, TValue>(this FieldInfo fieldInfo)
    {
        Check.NotNull(fieldInfo);
        ParameterExpression targetExp = Expression.Parameter(typeof(TTarget), "target");
        Expression source = targetExp;

        if (typeof(TTarget) != fieldInfo.DeclaringType)
        {
            source = Expression.Convert(targetExp, fieldInfo.DeclaringType);
        }

        MemberExpression fieldExp = Expression.Field(source, fieldInfo);
        return Expression.Lambda<Func<TTarget, TValue>>(fieldExp, targetExp).Compile();
    }
}
