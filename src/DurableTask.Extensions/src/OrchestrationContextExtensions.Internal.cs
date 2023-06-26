// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using DurableTask.Core;
using DurableTask.Core.Serializing;
using DurableTask.Extensions.Converters;

namespace DurableTask.Extensions;

/// <summary>
/// Extensions for <see cref="OrchestrationContext" />.
/// </summary>
public static partial class OrchestrationContextExtensions
{
    /// <summary>
    /// Sets the data converter, if it is not null.
    /// </summary>
    /// <param name="context">The orchestration context.</param>
    /// <param name="converter">The data converter to set.</param>
    internal static void SetDataConverter(this OrchestrationContext context, DataConverter? converter)
    {
        if (converter is null)
        {
            return;
        }

        if (converter is not JsonDataConverter jsonDataConverter)
        {
            jsonDataConverter = new JsonDataConverterShim(converter);
        }

        context.MessageDataConverter = jsonDataConverter;
        context.ErrorDataConverter = jsonDataConverter;
    }
}
