// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core.Serializing;

namespace DurableTask.Extensions.Converters;

/// <summary>
/// A shim to work around some JsonDataConverter requirements.
/// </summary>
internal class JsonDataConverterShim : JsonDataConverter
{
    private readonly DataConverter _converter;

    /// <summary>
    /// Initializes a new instance of <see cref="JsonDataConverterShim" />.
    /// </summary>
    /// <param name="converter">The data converter.</param>
    public JsonDataConverterShim(DataConverter converter)
    {
        _converter = Check.NotNull(converter);
    }

    public override object Deserialize(string data, Type objectType)
        => _converter.Deserialize(data, objectType);

    public override string Serialize(object value)
        => _converter.Serialize(value);

    public override string Serialize(object value, bool formatted)
        => _converter.Serialize(value, formatted);
}
