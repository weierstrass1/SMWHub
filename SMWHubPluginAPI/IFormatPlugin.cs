using OneOf;
using SMWHubASMCodeLibrary;
using SMWHubEnumerators;
using Validations;

namespace SMWHubPluginAPI;
/// <summary>
/// Represent a Custom Format Plugin that can be used to read and process data from a specific format.
/// </summary>
public interface IFormatPlugin
{
    public PluginContext Context { get; }
    public IEnumerable<FormatDefinition> FormatDefinitions { get; }
    /// <summary>
    /// Indicates whether the format can be embedded from the specified IScopeType.
    /// </summary>
    /// <param name="t">The IScopeType to check.</param>
    /// <returns>True if the format can be embedded from the specified IScopeType; otherwise, false.</returns>
    public bool CanBeEmbeddedFrom(Type t);
    /// <summary>
    /// Indicates whether the format can be included from the specified IScopeType.
    /// </summary>
    /// <param name="t">The IScopeType to check.</param>
    /// <returns>True if the format can be included from the specified IScopeType; otherwise, false.</returns>
    public bool CanBeIncludedFrom(Type t);
}
/// <summary>
/// Represent a Strongly Typed Custom Format Plugin that can be used to read and process data from a specific format.
/// </summary>
/// <typeparam name="TFormat"></typeparam>
public interface IFormatPlugin<TFormat> : IFormatPlugin
{
    /// <summary>
    /// Reads the data from the specified IFormattedEnumerable and returns a collection of ValidationResult or TFormat indicating the result of the read operation for each item.
    /// </summary>
    /// <param name="context">Context used by the plugin</param>
    /// <param name="readerEnum">The IFormattedEnumerable to read from</param>
    /// <returns>A collection that has ValidationResults when the read fails and TFormat when it succeeds.</returns>
    public IEnumerable<OneOf<ValidationResult, TFormat>> Read(CodeScope scope, IFormattedEnumerable readerEnum);
    /// <summary>
    /// Processes the specified collection of TFormat and returns a ValidationResult with the possible errors that occurred during the processing operation.
    /// </summary>
    /// <param name="context">Context used by the plugin</param>
    /// <param name="obj">The collection of TFormat to process</param>
    /// <returns>A ValidationResult with the possible errors that occurred during the processing operation.</returns>
    public ValidationResult Process(CodeScope scope, IEnumerable<TFormat> obj);
}
