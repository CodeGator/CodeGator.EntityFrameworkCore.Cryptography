
#pragma warning disable IDE0130
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130

/// <summary>
/// This class utility contains extension methods related to the <see cref="IHost"/>
/// type.
/// </summary>
public static partial class HostExtensions
{
    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method adds startup actions and/or pipeline middleware for the 
    /// <c>CodeGator</c> Entity Framework Core data protector.
    /// </summary>
    /// <typeparam name="T">The type of host to use for the operation.</typeparam>
    /// <param name="host">The host to use for the operation.</param>
    /// <returns>The value of the <paramref name="host"/> parameter, for
    /// chaining method calls together, Fluent style.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more arguments are missing, or invalid.</exception>
    public static T UseCodeGatorDbContextProtector<T>(
        [NotNull] this T host
        ) where T : IHost
    {
        Guard.Instance().ThrowIfNull(host, nameof(host));

        var logger = host.Services.GetRequiredService<ILogger<IHost>>();
        
        logger.LogDebug(
            "Registering the CodeGator service locator."
            );

        host.UseCodeGatorServiceLocator();

        return host;
    }

    #endregion
}
