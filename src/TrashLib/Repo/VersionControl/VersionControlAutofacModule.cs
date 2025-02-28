using Autofac;

namespace TrashLib.Repo.VersionControl;

public class VersionControlAutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<GitRepository>().As<IGitRepository>();
        builder.RegisterType<GitRepositoryFactory>().As<IGitRepositoryFactory>();
        builder.RegisterType<GitPath>().As<IGitPath>();
        base.Load(builder);
    }
}
