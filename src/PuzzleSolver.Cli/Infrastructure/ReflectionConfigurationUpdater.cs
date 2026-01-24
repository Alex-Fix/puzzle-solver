using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using PuzzleSolver.Cli.Interfaces;
using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Cli.Infrastructure;

internal sealed class ReflectionConfigurationUpdater : IConfigurationUpdater
{
    private readonly ConcurrentDictionary<Type, (string Key, PropertyInfo[] Properties)> _propertiesCache = new();
    
    private readonly IConfigurationRoot _configuration;
    
    public ReflectionConfigurationUpdater(IConfiguration configuration)
        => _configuration = configuration as IConfigurationRoot 
                ?? throw new ArgumentNullException(nameof(configuration));
    
    public void Update(params IOptions[] options)
    {
        foreach (IOptions option in options)
        {
            (string key, PropertyInfo[] properties) = _propertiesCache.GetOrAdd(
                option.GetType(),
                type =>
                {
                    string key = type
                                     .GetProperty(nameof(IOptions.Name), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                     ?.GetValue(null) as string 
                                 ?? throw new NullReferenceException("Key not defined.");
                    
                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    
                    return (key, properties);
                });
            
            foreach(PropertyInfo property in properties)
            {
                object? value = property.GetValue(option);
                _configuration[$"{key}:{property.Name}"] = value?.ToString();
            }
        }
        
        _configuration.Reload();
    }
}
