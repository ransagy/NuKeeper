using System.Threading.Tasks;
using NuKeeper.Abstractions.Configuration;
using NuKeeper.Abstractions.NuGet;
using NuKeeper.Abstractions.RepositoryInspection;

namespace NuKeeper.Update
{
    public interface IUpdateRunner
    {
        Task Update(PackageUpdateSet updateSet, NuGetSources sources, SettingsContainer settings);
    }
}
