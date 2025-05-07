using SS14.ConfigProvider.Model;
using SS14.Jetfish.Database;

namespace SS14.Jetfish.Core.Services;

/// <summary>
/// Provides proxy methods with type casting for the ConfigurationStore.
/// </summary>
/// <remarks>
/// Usually IConfiguration or any other Options pattern should be used instead.
/// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options
/// </remarks>
public class ConfigurationStoreService
{
    private readonly ApplicationDbContext _applicationDbContext;

    public ConfigurationStoreService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }


    /// <summary>
    /// Proxy method of <see cref="Get{T}"/> for getting a string from a store.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the store is null or has no value.</exception>
    public string Get(string name)
    {
        return Get<string>(name);
    }

    /// <summary>
    /// Returns the value of a store cast to T.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the store is null or has no value.</exception>
    public T Get<T>(string name)
    {
        var store = _applicationDbContext.ConfigurationStore.FirstOrDefault(s => s.Name == name);

        if (store == null)
            throw new InvalidOperationException($"Store with name {name} not found");

        if (store.Value == null)
            throw new InvalidOperationException($"Store {name} has no value.");

        return (T)Convert.ChangeType(store.Value, typeof(T));
    }

    /// <summary>
    /// Proxy method of <see cref="GetOrNull{T}"/> for getting a string from a store.
    /// </summary>
    public string? GetOrNull(string name)
    {
        return GetOrNull<string>(name);
    }

    /// <summary>
    /// Returns the value of a store cast to T or the default of T if the store does not exist or has no value.
    /// </summary>
    public T? GetOrNull<T>(string name)
    {
        var store = _applicationDbContext.ConfigurationStore.FirstOrDefault(s => s.Name == name);

        if (store?.Value == null)
            return default;

        return (T)Convert.ChangeType(store.Value, typeof(T));
    }

    /// <summary>
    /// Returns whether a store of that name exists.
    /// </summary>
    public bool HasStore(string name)
    {
        return _applicationDbContext.ConfigurationStore.Any(s => s.Name == name);
    }
}
