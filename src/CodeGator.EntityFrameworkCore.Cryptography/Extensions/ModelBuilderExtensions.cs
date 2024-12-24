
#pragma warning disable IDE0130
namespace Microsoft.EntityFrameworkCore;
#pragma warning restore IDE0130

// I loosely based my code on this article: https://gor-grigoryan.medium.com/encryption-and-data-security-in-clean-architecture-using-ef-core-value-converters-a-guide-to-911711a1ec52

/// <summary>
/// This class utility contains extension methods related to the <see cref="ModelBuilder"/>
/// type.
/// </summary>

public static partial class ModelBuilderExtensions
{
    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method adds a custom <c>CodeGator</c> data protector for the 
    /// given entity property - even if that property isn't decorated with
    /// a <see cref="ProtectedAttribute"/> attribute.
    /// </summary>
    /// <typeparam name="TBuilder">The type of associated model builder.</typeparam>
    /// <typeparam name="TEntity">The type of associated entity.</typeparam>
    /// <typeparam name="TProperty">The type of associated property.</typeparam>
    /// <param name="modelBuilder">The model builder to use for the operation.</param>
    /// <param name="propertyExpression">The property expression to use for 
    /// the operation.</param>
    /// <param name="providerName">The provider name to use for the operation.</param>
    /// <returns>The value of the <paramref name="modelBuilder"/> parameter,
    /// for chaining method calls together, Fluent style.</returns>
    /// <exception cref="ArgumentException">This exception is throw whenever one or
    /// more arguments are missing, or invalid.</exception>
    public static TBuilder ProtectUndecoratedProperty<TBuilder, TEntity, TProperty>(
        [NotNull] this TBuilder modelBuilder,
        [NotNull] Expression<Func<TEntity, TProperty>> propertyExpression,
        [NotNull] string providerName = "Default"
        ) where TBuilder : ModelBuilder
          where TEntity : class
    {
        Guard.Instance().ThrowIfNull(modelBuilder, nameof(modelBuilder))
            .ThrowIfNull(propertyExpression, nameof(propertyExpression))
            .ThrowIfNullOrEmpty(providerName, nameof(providerName));

        if (typeof(TProperty) == typeof(string))
        {
            modelBuilder.Entity<TEntity>()
                .Property(propertyExpression)
                .HasConversion(
                    new ProtectedStringConverter(providerName)
                    );
        }
        
        if (typeof(TProperty) == typeof(byte[]))
        {
            modelBuilder.Entity<TEntity>()
                .Property(propertyExpression)
                .HasConversion(
                    new ProtectedByteArrayConverter(providerName),
                    new ValueComparer<byte[]>(
                        (obj, otherObj) => ReferenceEquals(obj, otherObj),
                        obj => obj.GetHashCode(),
                        obj => obj
                        )
                    );
        }

        return modelBuilder;
    }

    // *******************************************************************

    /// <summary>
    /// This method adds a custom <c>CodeGator</c> data protector for any 
    /// entity properties decorated with the <see cref="ProtectedAttribute"/>
    /// attribute.
    /// </summary>
    /// <typeparam name="TBuilder">The type of associated model builder.</typeparam>
    /// <param name="modelBuilder">The model builder to use for the operation.</param>
    /// <returns>The value of the <paramref name="modelBuilder"/> parameter,
    /// for chaining method calls together, Fluent style.</returns>
    /// <exception cref="ArgumentException">This exception is throw whenever one or
    /// more arguments are missing, or invalid.</exception>
    public static TBuilder UseCodeGatorEncryption<TBuilder>(
        [NotNull] this TBuilder modelBuilder
        ) where TBuilder : ModelBuilder
    {
        Guard.Instance().ThrowIfNull(modelBuilder, nameof(modelBuilder));   

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (IsDiscriminator(property))
                {
                    continue;
                }

                var attr = property.PropertyInfo?.GetCustomAttributes(
                    typeof(ProtectedAttribute), 
                    false
                    ).FirstOrDefault();

                if (attr is not null)
                {
                    var providerName = ((ProtectedAttribute)attr).ProviderName ?? "Default";

                    if (property.ClrType == typeof(string))
                    {
                        property.SetValueConverter(
                            new ProtectedStringConverter(providerName)
                            ); 
                    }

                    if (property.ClrType == typeof(byte[]))
                    {
                        property.SetValueConverter(
                            new ProtectedByteArrayConverter(providerName)
                            );

                        property.SetValueComparer(
                            new ValueComparer<byte[]>(
                                (obj, otherObj) => ReferenceEquals(obj, otherObj),
                                obj => obj.GetHashCode(),
                                obj => obj
                                )
                            );
                    }
                }
            }
        }

        return modelBuilder;
    }

    #endregion

    // *******************************************************************
    // Private methods.
    // *******************************************************************

    #region Private methods

    /// <summary>
    /// This method indicates whether the given property is an EFCORE discriminator.
    /// </summary>
    /// <param name="property">The property to use for the operation.</param>
    /// <returns><c>true</c> if the given property is an EFCORE discriminator, 
    /// or <c>false otherwise.</c></returns>
    private static bool IsDiscriminator(IMutableProperty property)
    {
        return property.Name == "Discriminator" || property.PropertyInfo is null;
    }

    #endregion
}


