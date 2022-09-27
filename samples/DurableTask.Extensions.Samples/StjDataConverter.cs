// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Text.Json;
using DurableTask.Core.Serializing;

namespace DurableTask.Extensions.Samples;

/// <summary>
/// DataConverter using System.Text.Json.
/// </summary>
internal class StjDataConverter : JsonDataConverter
{
    private readonly JsonSerializerOptions _options;

    public StjDataConverter(JsonSerializerOptions options = null)
        => _options = options ?? new();

    public override string Serialize(object value) => Serialize(value, false);

    public override string Serialize(object value, bool formatted)
        => JsonSerializer.Serialize(value, _options);

    public override object Deserialize(string data, Type objectType)
        => JsonSerializer.Deserialize(data, objectType, _options);
}
