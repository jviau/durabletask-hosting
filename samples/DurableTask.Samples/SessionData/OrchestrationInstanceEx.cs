using System.Runtime.Serialization;
using DurableTask.Core;

namespace DurableTask.Samples;

/// <summary>
/// This is an example for how to use <see cref="OrchestrationInstance.ExtensionData"/> to hold orchestration-wide session
/// data that can be accessed in all activities within the orchestration. This is done by taking the original
/// <see cref="OrchestrationInstance"/>, "casting" it via serialization to our type, populating the data, then letting the
/// orchestration service serialize it back - putting our extra data in the extension data structure. We can then retrieve
/// that data at a later time via serialization "casting".
///
/// Data is seeded into this session via usage of tags on
/// <see cref="TaskHubClient.CreateOrchestrationInstanceAsync(string, string, string, object, IDictionary{string, string})"/>.
/// </summary>
/// <remarks>
/// The data contract purposefully is identical to <see cref="OrchestrationInstance"/> data contract. This lets the serializers
/// serialize this type as its base type without any <see cref="KnownTypeAttribute"/>.
/// </remarks>
[DataContract(Name = "OrchestrationInstance", Namespace = "http://schemas.datacontract.org/2004/07/DurableTask.Core")]
public class OrchestrationInstanceEx : OrchestrationInstance
{
    private static readonly DataContractSerializer s_customSerializer =
           new(typeof(OrchestrationInstanceEx));

    private static readonly DataContractSerializer s_defaultSerializer =
        new(typeof(OrchestrationInstance));

    /// <summary>
    /// Gets or sets the correlation id.
    /// </summary>
    [DataMember]
    public string CorrelationId { get; set; }

    /// <summary>
    /// Creates a <see cref="OrchestrationInstanceEx"/> from the provided
    /// <see cref="OrchestrationRuntimeState"/>, using its extension data if available.
    /// </summary>
    /// <param name="runtimeState">The runtime state to create this instance from.</param>
    /// <returns>A new or deserialized instance.</returns>
    public static OrchestrationInstanceEx Initialize(OrchestrationRuntimeState runtimeState)
    {
        if (runtimeState is null)
        {
            throw new ArgumentNullException(nameof(runtimeState));
        }

        OrchestrationInstance instance = runtimeState.OrchestrationInstance;
        if (instance is OrchestrationInstanceEx custom)
        {
            return custom;
        }

        custom = Get(instance);

        // Populate values not there.
        // In the case of SubOrchestrations, the CustomOrchestrationInstance is not carried
        // over, but the tags are! It is possible to carry the session over to the sub orchestration
        // as the parent orchestration instance is available here in runtimeState.ParentInstance.
        if (string.IsNullOrEmpty(custom.CorrelationId))
        {
            if (runtimeState.ExecutionStartedEvent.Tags == null)
            {
                runtimeState.ExecutionStartedEvent.Tags = new Dictionary<string, string>();
            }

            if (!runtimeState.ExecutionStartedEvent.Tags.TryGetValue(
                nameof(CorrelationId), out string correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                runtimeState.ExecutionStartedEvent.Tags[nameof(CorrelationId)] = correlationId;
            }

            custom.CorrelationId = correlationId;
        }

        runtimeState.ExecutionStartedEvent.OrchestrationInstance = custom;
        return custom;
    }

    /// <summary>
    /// Gets a <see cref="OrchestrationInstanceEx"/> from a <see cref="OrchestrationInstance"/>,
    /// using its extension data if available.
    /// </summary>
    /// <param name="instance">The orchestration instance. Not null.</param>
    /// <returns>The custom orchestration instance.</returns>
    public static OrchestrationInstanceEx Get(OrchestrationInstance instance)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (instance is OrchestrationInstanceEx custom)
        {
            return custom;
        }

        // We need to first get custom extension data by serializing & deserializing.
        using MemoryStream stream = new();
        s_defaultSerializer.WriteObject(stream, instance);
        stream.Position = 0;
        return (OrchestrationInstanceEx)s_customSerializer.ReadObject(stream);
    }
}
