// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core.Serializing;

namespace DurableTask.Extensions;

/// <summary>
/// Options for durable extensions.
/// </summary>
public sealed class DurableExtensionsOptions
{
    private DataConverter? _converter;

    /// <summary>
    /// Gets or sets the <see cref="Core.Serializing.DataConverter" /> to use with durable extensions.
    /// </summary>
    /// <remarks>
    /// Default is <see cref="JsonDataConverter" />.
    /// </remarks>
    public DataConverter DataConverter
    {
        get => _converter ??= JsonDataConverter.Default;
        set => _converter = value;
    }

    internal void ApplyIfNotSet(DataConverter converter)
    {
        _converter ??= converter;
    }
}
