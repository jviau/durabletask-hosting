// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

namespace DurableTask.Extensions.Abstractions;

/// <summary>
/// Represents an empty (void) result.
/// </summary>
public sealed class Empty : IEquatable<Empty>, IComparable<Empty>
{
#pragma warning disable CA1801 // unused parameters
#pragma warning disable IDE0060 // unused parameters

    /// <summary>
    /// Default value for <see cref="Empty" />.
    /// </summary>
    public static readonly Empty Value = new();

    /// <summary>
    /// Task result for a <see cref="Empty" />.
    /// </summary>
    public static readonly Task<Empty> Task = System.Threading.Tasks.Task.FromResult(Value);

    /// <summary>
    /// Compares two empties for equality. Always true.
    /// </summary>
    /// <param name="left">The left empty.</param>
    /// <param name="right">The right empty.</param>
    /// <returns>Always true.</returns>
    public static bool operator ==(Empty left, Empty right) => Equals(left, right);

    /// <summary>
    /// Compares two empties for inequality. Always false.
    /// </summary>
    /// <param name="left">The left empty.</param>
    /// <param name="right">The right empty.</param>
    /// <returns>Always false.</returns>
    public static bool operator !=(Empty left, Empty right) => !Equals(left, right);

    /// <summary>
    /// Compares two empties. Always false.
    /// </summary>
    /// <param name="left">The left empty.</param>
    /// <param name="right">The right empty.</param>
    /// <returns>Always false.</returns>
    public static bool operator <(Empty left, Empty right) => false;

    /// <summary>
    /// Compares two empties. Always true.
    /// </summary>
    /// <param name="left">The left empty.</param>
    /// <param name="right">The right empty.</param>
    /// <returns>Always true.</returns>
    public static bool operator <=(Empty left, Empty right) => Equals(left, right);

    /// <summary>
    /// Compares two empties. Always false.
    /// </summary>
    /// <param name="left">The left empty.</param>
    /// <param name="right">The right empty.</param>
    /// <returns>Always false.</returns>
    public static bool operator >(Empty left, Empty right) => false;

    /// <summary>
    /// Compares two empties. Always true.
    /// </summary>
    /// <param name="left">The left empty.</param>
    /// <param name="right">The right empty.</param>
    /// <returns>Always true.</returns>
    public static bool operator >=(Empty left, Empty right) => Equals(left, right);

    /// <inheritdoc />
    public int CompareTo(Empty other) => 0;

    /// <inheritdoc />
    public bool Equals(Empty other) => other is not null;

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is Empty;

    /// <inheritdoc />
    public override int GetHashCode() => 0;

    /// <inheritdoc />
    public override string ToString() => "{}";

    private static bool Equals(Empty left, Empty right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }

        return true;
    }

#pragma warning restore CA1801 // unused parameters
#pragma warning restore IDE0060 // unused parameters
}
