// Copyright (c) Jacob Viau. All rights reserved.
// Licensed under the APACHE 2.0. See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Text;

namespace DurableTask.DependencyInjection;

/// <summary>
/// A struct for representing a type short name. "{Namespace}.{Name}, {AssemblyShortName}".
/// </summary>
internal readonly struct TypeShortName
{
    private const char NameSeparator = ',';
    private const char GenericSeparator = '|';

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeShortName"/> struct.
    /// </summary>
    /// <param name="type">The type to get the name for.</param>
    public TypeShortName(Type type)
    {
        Check.NotNull(type, nameof(type));
        Name = type.IsConstructedGenericType ? type.GetGenericTypeDefinition().FullName : type.FullName;
        AssemblyName = type.Assembly.GetName().Name;

        if (type.IsGenericType && !type.IsGenericTypeDefinition)
        {
            GenericParams = type.GetGenericArguments().Select(t => new TypeShortName(t));
        }
        else
        {
            GenericParams = Enumerable.Empty<TypeShortName>();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeShortName"/> struct.
    /// </summary>
    /// <param name="typeShortName">The string to parse.</param>
    public TypeShortName(string typeShortName)
        : this(typeShortName, false)
    {
    }

    private TypeShortName(string typeShortName, bool genericParam)
    {
        Check.NotNullOrEmpty(typeShortName, nameof(typeShortName));

        // Find the ',' separating the assembly name. First check for last `]` make sure we don't grab an inner
        // generic assembly name.
        int nameSeparatorStart = typeShortName.LastIndexOf(']');
        if (nameSeparatorStart == -1)
        {
            nameSeparatorStart = 0;
        }

        int nameSeparator = typeShortName.IndexOf(NameSeparator, nameSeparatorStart);
        AssemblyName = nameSeparator > 0 ? typeShortName.Substring(nameSeparator + 1).Trim() : null;

        // If we are a generic parameter, assert that we have an assembly name.
        Check.Argument(
            !genericParam || !string.IsNullOrEmpty(AssemblyName),
            nameof(typeShortName),
            "'{0}' is a generic parameter but is missing assembly name.",
            typeShortName);

        // Now get the name. First check for generic start index - if it exists.
        string name = nameSeparator > 0 ? typeShortName.Substring(0, nameSeparator) : typeShortName;
        int genericStart = name.IndexOf('[');
        if (genericStart == -1)
        {
            // Not a generic type, just use name as is.
            Name = name;
            GenericParams = Enumerable.Empty<TypeShortName>();
            return;
        }

        // Generic parameter(s) found. Pull them out.
        Name = name[..genericStart];

        IEnumerable<string> generics = SplitGenerics(name.Substring(genericStart));
        GenericParams = generics.Select(s => new TypeShortName(s, true)).ToList();
    }

    /// <summary>
    /// Gets the type name, with namespace.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the assembly short name.
    /// </summary>
    public string AssemblyName { get; }

    /// <summary>
    /// Gets generic params, if any.
    /// </summary>
    public IEnumerable<TypeShortName> GenericParams { get; }

    /// <summary>
    /// Gets a value indicating whether if this type is generic or not.
    /// </summary>
    private bool IsGeneric => GenericParams.Any();

    /// <summary>
    /// Tries to parse <paramref name="typeShortName"/> into a <see cref="TypeShortName"/>.
    /// </summary>
    /// <param name="typeShortName">The name to parse.</param>
    /// <param name="typeName">The parsed name.</param>
    /// <returns>True if successfully parsed, false otherwise.</returns>
    public static bool TryParse(string typeShortName, out TypeShortName typeName)
    {
        typeName = default;
        if (string.IsNullOrEmpty(typeShortName))
        {
            return false;
        }

        try
        {
            typeName = new TypeShortName(typeShortName);
            return true;
        }
        catch (Exception ex) when (ex is ArgumentException || ex is IndexOutOfRangeException)
        {
        }

        return false;
    }

    /// <summary>
    /// Gets the short name string for a type.
    /// </summary>
    /// <param name="type">The type to get the name for. Not null.</param>
    /// <param name="includeTopAssembly">True to include top level assembly, false otherwise.</param>
    /// <returns>The short type name of "{Namespace}.{Name}[, {AssemblyShortName}]?".</returns>
    /// <remarks>
    /// Nested generic params will always have assembly name appended, regardless of
    /// <paramref name="includeTopAssembly"/>.
    /// </remarks>
    public static string ToString(Type type, bool includeTopAssembly = true)
        => new TypeShortName(type).ToString(includeTopAssembly);

    /// <summary>
    /// Gets the short type name represented by this struct.
    /// </summary>
    /// <returns>The short type name of "{Namespace}.{Name}, {AssemblyShortName}".</returns>
    public override string ToString() => ToString(true);

    /// <summary>
    /// Gets the short type name represented by this struct, with or without the assembly name appended.
    /// </summary>
    /// <param name="includeTopAssembly">True to include ", {AssemblyShortName}". False to leave it out.</param>
    /// <returns>The short type name of "{Namespace}.{Name}[, {AssemblyShortName}]?".</returns>
    /// <remarks>
    /// Nested generic params will always have assembly name appended, regardless of
    /// <paramref name="includeTopAssembly"/>.
    /// </remarks>
    public string ToString(bool includeTopAssembly)
    {
        if (string.IsNullOrEmpty(AssemblyName))
        {
            includeTopAssembly = false;
        }

        if (!IsGeneric)
        {
            return includeTopAssembly ? $"{Name}, {AssemblyName}" : Name;
        }

        StringBuilder builder = new();
        AppendTo(builder, includeTopAssembly, false);
        return builder.ToString();
    }

    /// <summary>
    /// Loads this type.
    /// </summary>
    /// <param name="assembly">The optional assembly to load from.</param>
    /// <returns>The loaded type.</returns>
    public Type Load(Assembly assembly = null)
    {
        if (assembly is null)
        {
            assembly = string.IsNullOrEmpty(AssemblyName)
                ? Assembly.GetEntryAssembly()
                : Assembly.Load(new AssemblyName { Name = AssemblyName });
        }

        Type type = assembly.GetType(Name, throwOnError: true);
        if (IsGeneric)
        {
            type = type.MakeGenericType(GenericParams.Select(x => x.Load()).ToArray());
        }

        return type;
    }

    private static IEnumerable<string> SplitGenerics(string typeString)
    {
        static IEnumerable<string> InnerEnum(string typeString)
        {
            int bracketCount = 0;
            int start = 0;
            for (int i = 1; i < typeString.Length - 1; i++)
            {
                char c = typeString[i];
                if (c == '[')
                {
                    bracketCount++;
                }
                else if (c == ']')
                {
                    bracketCount--;
                }
                else if (bracketCount == 0 && c == GenericSeparator)
                {
                    yield return typeString.Substring(start, i - start);
                    start = i + 1;
                }
            }

            yield return start == 0 ? typeString : typeString.Substring(start);
        }

        return InnerEnum(typeString).Select(s => s.Trim('[', ']'));
    }

    private void AppendTo(StringBuilder builder, bool includeAssembly, bool wrapBrackets)
    {
        if (wrapBrackets)
        {
            builder.Append('[');
        }

        builder.Append(Name);

        if (IsGeneric)
        {
            builder.Append('[');
            AppendGenerics(builder);
            builder.Append(']');
        }

        if (includeAssembly)
        {
            builder.Append(", ");
            builder.Append(AssemblyName);
        }

        if (wrapBrackets)
        {
            builder.Append(']');
        }
    }

    private void AppendGenerics(StringBuilder builder)
    {
        IEnumerator<TypeShortName> en = GenericParams.GetEnumerator();
        if (!en.MoveNext())
        {
            return;
        }

        TypeShortName value = en.Current;
        value.AppendTo(builder, true, true);
        while (en.MoveNext())
        {
            builder.Append(GenericSeparator);
            value = en.Current;
            value.AppendTo(builder, true, true);
        }
    }
}
