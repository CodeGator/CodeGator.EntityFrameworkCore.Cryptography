
#pragma warning disable IDE0130
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130

/// <summary>
/// This class utility contains extension methods related to the <see cref="IHostApplicationBuilder"/>
/// type.
/// </summary>
public static partial class HostApplicationBuilderExtensions
{
    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method adds abstractions and options required to support the 
    /// <c>CodeGator</c> Entity Framework Core data protector.
    /// </summary>
    /// <typeparam name="T">The type of associated host application builder.</typeparam>
    /// <param name="hostApplicationBuilder">The host application builder
    /// to use for the operation.</param>
    /// <param name="bootstrapLogger">An optional bootstrap logger to use
    /// for the operation.</param>
    /// <returns>The value of the <paramref name="hostApplicationBuilder"/>
    /// parameter, for chaining method calls together, Fluent style.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more arguments are missing, or invalid.</exception>
    public static T AddCodeGatorDbContextProtector<T>(
        [NotNull] this T hostApplicationBuilder,
        ILogger? bootstrapLogger = null
        ) where T : IHostApplicationBuilder
    {
        Guard.Instance().ThrowIfNull(hostApplicationBuilder, nameof(hostApplicationBuilder));

        bootstrapLogger?.LogDebug(
            "Adding options for the CodeGator EFCORE data protection provider."
            );

        hostApplicationBuilder.Services.TryAddOptions<ProtectionProviderOptions>()
            .BindConfiguration(ProtectionProviderOptions.SectionPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        bootstrapLogger?.LogDebug(
            "Adding the CodeGator cryptography services."
            );

        hostApplicationBuilder.AddCodeGatorCryptography();

        return hostApplicationBuilder;
    }

    #endregion
}
