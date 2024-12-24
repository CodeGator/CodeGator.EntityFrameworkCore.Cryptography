
#pragma warning disable IDE0130
namespace System.ComponentModel.DataAnnotations;
#pragma warning restore IDE0130

/// <summary>
/// This attribute denotes a property whose contents are protected at rest.
/// </summary>
/// <param name="providerName">A provider name to use when protecting and 
/// unprotecting the data for the associated property.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class ProtectedAttribute(
    [NotNull] string providerName = "Default"
    ) : Attribute
{
    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains an optional provider name to use when protecting
    /// and unprotecting the data for the associated property.
    /// </summary>
    public string? ProviderName => providerName;

    #endregion
}
