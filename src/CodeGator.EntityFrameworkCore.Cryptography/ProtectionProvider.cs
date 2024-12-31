
namespace CodeGator.EntityFrameworkCore.Cryptography;

/// <summary>
/// This class performs cryptographic protection operations.
/// </summary>
internal sealed class ProtectionProvider
{
    // *******************************************************************
    // Fields.
    // *******************************************************************

    #region Fields

    /// <summary>
    /// This field is the backing field for the <see cref="ProtectionProvider.Name"/>
    /// property.
    /// </summary>
    private readonly string _name;

    /// <summary>
    /// This field contains the key and IV for this protection provider.
    /// </summary>
    private readonly KeyAndIV _keyAndIV;

    /// <summary>
    /// This field contains the table of all provider instances.
    /// </summary>
    private static readonly Dictionary<string, ProtectionProvider> _instances = [];

    #endregion

    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains the name of this provider instance.
    /// </summary>
    public string Name => _name;

    #endregion

    // *******************************************************************
    // Constructors.
    // *******************************************************************

    #region Constructors

    /// <summary>
    /// This constructor creates a new instance of the <see cref="ProtectionProvider"/>
    /// class.
    /// </summary>
    /// <param name="name">The name of this provider instance.</param>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// the <paramref name="name"/> parameter contains a name that has already
    /// been used as a provider.</exception>
    /// <exception cref="InvalidDataException">This exception is thrown whenever
    /// the options for the named provider do not exist in the configuration, or
    /// have not been properly bound.</exception>
    /// <exception cref="ServiceException">This exception is thrown whenever
    /// the underlying cryptographic service fails to produce a key and IV for the 
    /// named provider's credentials.</exception>
    private ProtectionProvider(
        [NotNull] string name
        )
    {
        ThrowIfNameHasAlreadyBeenUsed(name);

        _name = name;

        using var scope = ServiceLocator.Instance().CreateScope();
        
        var options = scope.ServiceProvider.GetRequiredService<
            IOptions<ProtectionProviderOptions>
            >();

        if (options is null)
        {
            throw new InvalidDataException(
                $"Failed to bind the options for protection provider: " +
                $"{name}"
                );
        }

        // Debugging note: The options here have duplicate entries for the 
        //   provider credentials. Microsoft assures us (I can't remember 
        //   the link, you can Google for it) that this is "by design" and
        //   completely normal. In any event, when you see multiple entries
        //   for each provider here, it's not our code that's broken, it's
        //   .NET. 

        var credentials = options.Value.Credentials.FirstOrDefault(x => 
            x.Name.Equals(
                name, 
                StringComparison.InvariantCultureIgnoreCase
                )
            );
        if (credentials is null)
        {
            throw new InvalidDataException(
                $"Failed to locate credentials for protection provider: {name}"
                );
        }

        var rfc2898Iterations = credentials.Rfc2898Iterations ?? 10000;
        if (rfc2898Iterations < 10000)
        {
            rfc2898Iterations = 10000;
        }

        var cryptoService = scope.ServiceProvider.GetRequiredService<
            ICryptoService
            >();

        _keyAndIV = cryptoService.GenerateKeyAndIVAsync(
            credentials.Password,
            credentials.SALT,
            "SHA512",
            rfc2898Iterations
            ).Result;
    }

    #endregion

    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method returns the named provider instance, creating a new provider
    /// if one does not already exist.
    /// </summary>
    /// <param name="name">The provider name to use for the operation.</param>
    /// <returns>A <see cref="ProtectionProvider"/> instance.</returns>
    public static ProtectionProvider Instance(
        [NotNull] string name = "Default"
        )
    {
        if (!_instances.TryGetValue(name.ToLower(), out var provider))
        {
            provider = new ProtectionProvider(name);
            _instances.TryAdd(name.ToLower(), provider);
        }
        return provider;
    }

    // *******************************************************************

    /// <summary>
    /// This method encrypts the given string.
    /// </summary>
    /// <param name="value">The string to be encrypted.</param>
    /// <returns>The encrypted string.</returns>
    public string Encrypt(
        [NotNull] string value
        )
    {
        try
        {
            using var scope = ServiceLocator.Instance().CreateScope();
            
            var cryptoService = scope.ServiceProvider.GetRequiredService<
                ICryptoService
                >();

            var encValue = cryptoService.AesEncryptAsync(
                _keyAndIV,
                value
                ).Result;

            return encValue;
        }
        catch (Exception ex)
        {
            throw new ProviderException(
                innerException: ex,
                message: "Failed to encrypt a value!"
                );
        }        
    }

    // *******************************************************************

    /// <summary>
    /// This method encrypts the given byte array.
    /// </summary>
    /// <param name="value">The byte array to be encrypted.</param>
    /// <returns>The encrypted byte array.</returns>
    public byte[] Encrypt(
        [NotNull] byte[] value
        )
    {
        try
        {
            using var scope = ServiceLocator.Instance().CreateScope();
            
            var cryptoService = scope.ServiceProvider.GetRequiredService<
                ICryptoService
                >();

            var encValue = cryptoService.AesEncryptAsync(
                _keyAndIV,
                value
                ).Result;

            return encValue;
        }
        catch (Exception ex)
        {
            throw new ProviderException(
                innerException: ex,
                message: "Failed to encrypt a value!"
                );
        }
    }

    // *******************************************************************

    /// <summary>
    /// This method decrypts the given string.
    /// </summary>
    /// <param name="value">The string to be decrypted.</param>
    /// <returns>The decrypted string.</returns>
    public string Decrypt(
        [NotNull] string value
        )
    {
        try
        {
            using var scope = ServiceLocator.Instance().CreateScope();
            
            var cryptoService = scope.ServiceProvider.GetRequiredService<
                ICryptoService
                >();

            var decValue = cryptoService.AesDecryptAsync(
                _keyAndIV,
                value
                ).Result;

            return decValue;
        }
        catch (Exception ex)
        {
            throw new ProviderException(
                innerException: ex,
                message: "Failed to decrypt a value!"
                );
        }
    }

    // *******************************************************************

    /// <summary>
    /// This method decrypts the given byte array.
    /// </summary>
    /// <param name="value">The byte array to be decrypted.</param>
    /// <returns>The decrypted byte array.</returns>
    public byte[] Decrypt(
        [NotNull] byte[] value
        )
    {
        try
        {
            using var scope = ServiceLocator.Instance().CreateScope();
            
            var cryptoService = scope.ServiceProvider.GetRequiredService<
                ICryptoService
                >();

            var decValue = cryptoService.AesDecryptAsync(
                _keyAndIV,
                value
                ).Result;

            return decValue;
        }
        catch (Exception ex)
        {
            throw new ProviderException(
                innerException: ex,
                message: "Failed to decrypt a value!"
                );
        }
    }

    #endregion

    // *******************************************************************
    // Private methods.
    // *******************************************************************

    #region Private methods

    /// <summary>
    /// This method throws an exception if the given provider name has 
    /// already been used.
    /// </summary>
    /// <param name="name">The name to use for the operation.</param>
    /// <exception cref="ArgumentException">This exception is thrown 
    /// whenever the <paramref name="name"/> has already been used.</exception>
    private static void ThrowIfNameHasAlreadyBeenUsed(string name)
    {
        if (_instances.ContainsKey(name.ToLower()))
        {
            throw new ArgumentException(
                paramName: nameof(name),
                message: $"The provider name: '{name}' has already " +
                "been used!"
                );
        }
    }

    #endregion
}