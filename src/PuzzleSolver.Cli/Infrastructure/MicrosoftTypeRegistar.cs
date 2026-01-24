using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace PuzzleSolver.Cli.Infrastructure;

public class MicrosoftTypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _services;

    public MicrosoftTypeRegistrar(IServiceCollection services)
        => _services = services;

    public void Register(Type service, Type implementation)
        => _services.AddSingleton(service, implementation);

    public void RegisterInstance(Type service, object implementation)
        => _services.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object> factory)
        // ReSharper disable once ConvertClosureToMethodGroup
        => _services.AddSingleton(service, _ => factory());

    public ITypeResolver Build()
        => new MicrosoftTypeResolver(_services.BuildServiceProvider());
}
