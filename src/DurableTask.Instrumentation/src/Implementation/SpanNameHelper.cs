// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DurableTask.Instrumentation;

/// <summary>
/// Helper for DurableTask operations.
/// </summary>
internal static class SpanNameHelper
{
    /// <summary>
    /// Gets the name of a span given a prefix, task name, and task version. The name will be of the format:
    /// "{prefix}:{taskNameSimplified}(@{taskVersion})?".
    /// </summary>
    /// <param name="prefix">The span prefix.</param>
    /// <param name="taskName">The task name.</param>
    /// <param name="taskVersion">The task version.</param>
    /// <returns>A formatted span name.</returns>
    public static string GetSpanName(string prefix, string taskName, string? taskVersion)
    {
        taskName = SimplifyName(taskName);
        return string.IsNullOrEmpty(taskVersion) ? $"{prefix}:{taskName}" : $"{prefix}:{taskName}@{taskVersion}";
    }

    /// <summary>
    /// Gets the name of a span given a prefix, task name, and task version. The name will be of the format:
    /// "{prefix}:{taskNameSimplified}(@{taskVersion})?".
    /// </summary>
    /// <param name="prefix">The span prefix.</param>
    /// <param name="taskName">The task name.</param>
    /// <param name="taskVersion">The task version.</param>
    /// <returns>A formatted span name.</returns>
    public static string GetSpanName(string prefix, ReadOnlySpan<char> taskName, string? taskVersion)
    {
        string name = SimplifyName(taskName);
        return string.IsNullOrEmpty(taskVersion) ? $"{prefix}:{name}" : $"{prefix}:{name}@{taskVersion}";
    }

    /// <summary>
    /// Simplifies a type name.
    /// </summary>
    /// <param name="name">The name to simplify.</param>
    /// <returns>A name suitable for use as an operation name.</returns>
    /// <remarks>
    /// Example output:
    /// "Namespace.Name" -> "Name"
    /// "Namespace.Name`1[[System.String, System.Private.CoreLib]]" -> "Name`1 [String]"
    /// "Namespace.Name`2[[System.String, System.Private.CoreLib]|[System.Object, System.Private.CoreLib]]" -> "Name`1 [String, Object]".
    /// </remarks>
    [return: NotNullIfNotNull("name")]
    public static string? SimplifyName(string? name)
    {
        if (name is null)
        {
            return null;
        }

        if (name is "")
        {
            return string.Empty;
        }

        if (!name.Any(c => c is '.' or '|'))
        {
            return name;
        }

        return SimplifyName(name.AsSpan());
    }

    /// <summary>
    /// Simplifies a type name.
    /// </summary>
    /// <param name="name">The name to simplify.</param>
    /// <returns>A name suitable for use as an operation name.</returns>
    /// <remarks>
    /// Example output:
    /// "Namespace.Name" -> "Name"
    /// "Namespace.Name`1[[System.String, System.Private.CoreLib]]" -> "Name`1 [String]"
    /// "Namespace.Name`2[[System.String, System.Private.CoreLib]|[System.Object, System.Private.CoreLib]]" -> "Name`1 [String, Object]".
    /// </remarks>
    public static string SimplifyName(ReadOnlySpan<char> name)
    {
        if (name.Length == 0)
        {
            return string.Empty;
        }

        int genericStart = name.IndexOf('[');
        ReadOnlySpan<char> generic = default;
        if (genericStart != -1)
        {
            // If we have a generic, capture it separately and trim.
            generic = name[genericStart..];
            name = name[..genericStart];
        }
        else
        {
            // If we have an assembly, trim it off.
            // If we had a generic and trimmed it already, this will also have already been trimmed as the generic
            // comes before the assembly. "Namespace.Name`1[[Generic, Assembly2]], Assembly1". The above trim at '['
            // will already have removed the ', Assembly1' portion.
            int typeEnd = name.IndexOf(',');
            if (typeEnd != -1)
            {
                name = name[..typeEnd];
            }
        }

        int namespaceEnd = name.LastIndexOf('.');
        if (namespaceEnd != -1)
        {
            // If we have a namespace, trim it off.
            name = name[(namespaceEnd + 1)..];
        }

        if (genericStart == -1)
        {
            return name.ToString();
        }
        else
        {
            StringBuilder sb = new();
            sb.Append(name.ToString()); // TODO use span overload when possible.
            sb.Append(' ');
            AppendGeneric(sb, generic);
            return sb.ToString();
        }
    }

    /// <summary>
    /// Appends a simplified generic to the provided builder.
    /// </summary>
    /// <param name="builder">The builder to append to.</param>
    /// <param name="generic">The generic portion of the serialized type name.</param>
    /// <remarks>
    /// This will:
    /// 1. Take only type name, drop namespace
    /// 2. Drop assembly name
    /// 3. Concat generics with ", " instead of "|".
    /// 4. Use only single square brackets to wrap generics (since assembly is now gone, no need to group them).
    /// 5. Step into nested generics and perform the same.
    /// Examples:
    /// "[[System.String, System.Private.CoreLib]]" -> "[String]"
    /// "[[System.Collections.Generic.List[[System.String, System.Private.CoreLib]], System.Private.CoreLib]]" -> "[List[String]]"
    /// "[[System.Collections.Generic.Dictionary[[System.String, System.Private.CoreLib]|[System.Int32, System.Private.CoreLib]], System.Private.CoreLib]]" -> "[Dictionary[String, Int32]]".
    /// </remarks>
    private static void AppendGeneric(StringBuilder builder, ReadOnlySpan<char> generic)
    {
        int state = 0;
        int sectionStart = 0;
        int depth = 0;
        builder.Append('[');
        for (int i = 0; i < generic.Length; i++)
        {
            char c = generic[i];
            switch (state)
            {
                case 0:
                    // State 0: At start of generic, or about to move on to next generic.
                    if (c == '|')
                    {
                        // Sibling generic param is starting.
                        builder.Append(", ");
                    }
                    else if (c == '[')
                    {
                        // At the start of a generic.
                        state = 1;
                    }

                    break;
                case 1:
                    // State 1: Scanning type name, setting section start.
                    if (c != '[' && c != '.')
                    {
                        // Non-separator found, mark this as our copy start and begin looking for the copy end.
                        sectionStart = i;
                        state = 2;
                    }

                    break;
                case 2:
                    // State 2: Scanning for type end.
                    if (c == '.')
                    {
                        // Namespace separator found - go back to state 1 to search for next non-separator.
                        state = 1;
                    }
                    else if (c == ']' || c == ',' || c == '[')
                    {
                        // Type name end found. Copy name. "...System.String]..." -> we will copy "String".
                        builder.Append(generic[sectionStart..i].ToString()); // TODO: use span overload when possible.
                        state = 3;
                    }

                    break;
                case 3:
                    // State 3: Scanning for next type _or_ generic start.
                    // We are skipping over assembly name here.
                    if (c == ']')
                    {
                        // Type may end, or start another generic param. Next char expected is either another ']'
                        // (type ended) or a '|' (next generic started).
                        state = 4;
                    }
                    else if (c == '[')
                    {
                        // We just copied a type name, so this it was a generic type and this is the start of its
                        // generic params.
                        state = 1;
                        builder.Append('[');
                        depth++;
                    }

                    break;
                case 4:
                    // State 4: Scanning for type end.
                    if (c == ']')
                    {
                        if (depth > 0)
                        {
                            // Nested type has ended, generic param closed. Next expectation is either end of
                            // string, or continuation of parent generic.
                            builder.Append(']');
                            depth--;
                        }

                        state = 0;
                    }

                    if (c == '|')
                    {
                        // Next generic param has begun. Next expected char is '['.
                        builder.Append(", ");
                        state = 0;
                    }

                    break;
            }
        }

        builder.Append(']');
    }
}
