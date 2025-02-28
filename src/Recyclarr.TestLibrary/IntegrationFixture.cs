using System.IO.Abstractions;
using System.IO.Abstractions.Extensions;
using System.IO.Abstractions.TestingHelpers;
using Autofac;
using Autofac.Features.ResolveAnything;
using CliFx.Infrastructure;
using Common.TestLibrary;
using NUnit.Framework;
using Recyclarr.Command;
using Serilog;
using Serilog.Events;
using TestLibrary;
using TrashLib;
using TrashLib.Repo.VersionControl;
using TrashLib.Startup;

namespace Recyclarr.TestLibrary;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class IntegrationFixture : IDisposable
{
    protected IntegrationFixture()
    {
        Paths = new AppPaths(Fs.CurrentDirectory().SubDirectory("test").SubDirectory("recyclarr"));
        Logger = CreateLogger();

        Container = CompositionRoot.Setup(builder =>
        {
            builder.RegisterInstance(Fs).As<IFileSystem>();
            builder.RegisterInstance(Paths).As<IAppPaths>();
            builder.RegisterInstance(Console).As<IConsole>();
            builder.RegisterInstance(Logger).As<ILogger>().SingleInstance();

            builder.RegisterMockFor<IServiceCommand>();
            builder.RegisterMockFor<IGitRepository>();
            builder.RegisterMockFor<IGitRepositoryFactory>();

            RegisterExtraTypes(builder);

            builder.RegisterSource<AnyConcreteTypeNotAlreadyRegisteredSource>();
        });

        SetupMetadataJson();
    }

    // ReSharper disable once VirtualMemberNeverOverridden.Global
    // ReSharper disable once UnusedParameter.Global
    protected virtual void RegisterExtraTypes(ContainerBuilder builder)
    {
    }

    private static ILogger CreateLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Is(LogEventLevel.Debug)
            .WriteTo.TestCorrelator()
            .CreateLogger();
    }

    private void SetupMetadataJson()
    {
        var metadataFile = Paths.RepoDirectory.File("metadata.json");
        Fs.AddFileFromResource(metadataFile, "metadata.json");
    }

    // ReSharper disable MemberCanBePrivate.Global

    protected MockFileSystem Fs { get; } = new();
    protected FakeInMemoryConsole Console { get; } = new();
    protected ILifetimeScope Container { get; }
    protected IAppPaths Paths { get; }
    protected ILogger Logger { get; }

    // ReSharper restore MemberCanBePrivate.Global

    protected T Resolve<T>() where T : notnull
    {
        return Container.Resolve<T>();
    }

    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        Container.Dispose();
        Console.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
