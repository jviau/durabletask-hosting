// "Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the Apache License 2.0. See LICENSE file in the project root for full license information."

using Microsoft.Extensions.Options;

namespace DurableTask.DependencyInjection
{
    public abstract class BaseTaskHubClient
    {
        public BaseTaskHubClient(string name)
        {
            Name = name ?? Options.DefaultName;
        }

        public string Name { get; } 
    }
}
