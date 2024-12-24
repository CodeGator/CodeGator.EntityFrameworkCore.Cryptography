
#pragma warning disable IDE0130
namespace Microsoft.EntityFrameworkCore.Storage.ValueConversion;
#pragma warning restore IDE0130

/// <summary>
/// This class is a value converter for protected strings.
/// </summary>
internal sealed class ProtectedStringConverter : ValueConverter<string?, string>
{
    /// <summary>
    /// This constructor creates a new instance of the <see cref="ProtectedStringConverter"/>
    /// class.
    /// </summary>
    /// <param name="providerName">The provider to name to use for the operation.</param>
    /// <param name="mappingHints">The mapping hints to use for the operation.</param>
    public ProtectedStringConverter(
        [NotNull] string providerName = "Default",
        [AllowNull] ConverterMappingHints? mappingHints = null
        ) : base(
                x => ProtectionProvider.Instance(providerName).Encrypt(x),
                x => ProtectionProvider.Instance(providerName).Decrypt(x),
                mappingHints
            )
    {

    }
}

/// <summary>
/// This class is a value converter for protected byte arrays.
/// </summary>
internal sealed class ProtectedByteArrayConverter : ValueConverter<byte[], byte[]>
{
    /// <summary>
    /// This constructor creates a new instance of the <see cref="ProtectedByteArrayConverter"/>
    /// class.
    /// </summary>
    /// <param name="providerName">The provider to name to use for the operation.</param>
    /// <param name="mappingHints">The mapping hints to use for the operation.</param>
    public ProtectedByteArrayConverter(
        [NotNull] string providerName = "Default",
        [AllowNull] ConverterMappingHints? mappingHints = null
        ) : base(
                x => ProtectionProvider.Instance(providerName).Encrypt(x),
                x => ProtectionProvider.Instance(providerName).Decrypt(x),
                mappingHints
            )
    {

    }
}
