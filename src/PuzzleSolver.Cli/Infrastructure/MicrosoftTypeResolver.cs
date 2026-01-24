using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace PuzzleSolver.Cli.Infrastructure;

public class MicrosoftTypeResolver : ITypeResolver
{
    private readonly IServiceProvider _serviceProvider;

    public MicrosoftTypeResolver(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public object? Resolve(Type? type)
        => type is null ? null : _serviceProvider.GetRequiredService(type);
}
