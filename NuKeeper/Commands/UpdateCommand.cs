using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using NuKeeper.Abstractions;
using NuKeeper.Abstractions.Configuration;
using NuKeeper.Inspection.Logging;
using NuKeeper.Local;

namespace NuKeeper.Commands
{
    [Command("update", Description = "Applies relevant updates to a local project.")]
    internal class UpdateCommand : LocalNuKeeperCommand
    {
        [Option(CommandOptionType.SingleValue, ShortName = "m", LongName = "maxpackageupdates",
            Description = "Maximum number of package updates to make. Defaults to 1.")]
        public int? MaxPackageUpdates { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "p", LongName = "allowpartialupdates",
            Description = "Maximum number of package updates to make. Defaults to 1.")]
        public bool? AllowPartialUpdates { get; set; }

        private readonly ILocalEngine _engine;

        public UpdateCommand(ILocalEngine engine, IConfigureLogger logger, IFileSettingsCache fileSettingsCache)
            : base(logger, fileSettingsCache)
        {
            _engine = engine;
        }

        protected override async Task<ValidationResult> PopulateSettings(SettingsContainer settings)
        {
            var baseResult = await base.PopulateSettings(settings);
            if (!baseResult.IsSuccess)
            {
                return baseResult;
            }

            const int defaultMaxPackageUpdates = 1;
            var fileSettings = FileSettingsCache.GetSettings();

            var maxUpdates = Concat.FirstValue(
                MaxPackageUpdates,
                fileSettings.MaxPackageUpdates,
                defaultMaxPackageUpdates);

            const bool defaultAllowPartialUpdates = false;
            var partialUpdates = Concat.FirstValue(
                AllowPartialUpdates,
                fileSettings.AllowPartialUpdates,
                defaultAllowPartialUpdates);

            if (maxUpdates < 1)
            {
                return ValidationResult.Failure($"Max package updates of {maxUpdates} is not valid");
            }

            settings.PackageFilters.MaxPackageUpdates = maxUpdates;
            settings.UserSettings.AllowPartialUpdates = partialUpdates;

            return ValidationResult.Success;
        }

        protected override async Task<int> Run(SettingsContainer settings)
        {
            await _engine.Run(settings, true);
            return 0;
        }
    }
}
