using TrashLib.Config.Services;

namespace Recyclarr.Config;

public interface IConfigurationLoader<T>
    where T : IServiceConfiguration
{
    ICollection<T> LoadMany(IEnumerable<string> configFiles, string configSection);
    ICollection<T> Load(string file, string configSection);
    ICollection<T> LoadFromStream(TextReader stream, string requestedSection);
}
