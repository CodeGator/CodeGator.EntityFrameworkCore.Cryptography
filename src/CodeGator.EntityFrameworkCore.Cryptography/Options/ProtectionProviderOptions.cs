
namespace CodeGator.EntityFrameworkCore.Cryptography.Options;

/// <summary>
/// This class contains configuration settings for the <see cref="ProtectionProvider"/>
/// class.
/// </summary>
public class ProtectionProviderOptions
{
    // *******************************************************************
    // Fields.
    // *******************************************************************

    #region Fields

    /// <summary>
    /// This field contains the path to these options in the configuration.
    /// </summary>
    public const string SectionPath = "Providers:EfCoreProtection";

    #endregion

    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains credentials for named protection providers.
    /// </summary>
    [Required]
    public List<ProtectionProviderCredentials> Credentials { get; set; } = [];

    #endregion
}


/// <summary>
/// This class contains a password and SALT value for a protection provider.
/// </summary>
public class ProtectionProviderCredentials
{
    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains the name of the protection provider.
    /// </summary>
    [Required]
    public string Name { get; set; } = null!;

    /// <summary>
    /// This property contains the password for the protection provider.
    /// </summary>
    [Required]
    [MinLength(10)]
    public string Password { get; set; } = null!;

    /// <summary>
    /// This property contains a SALT value for the protection provider.
    /// </summary>
    [Required]
    [MinLength(16)]
    public string SALT { get; set; } = null!;

    /// <summary>
    /// This property contains the number of RFC1898 iterations to use
    /// for generating a key and IV.
    /// </summary>
    public int? Rfc2898Iterations { get; set; }

    #endregion
}