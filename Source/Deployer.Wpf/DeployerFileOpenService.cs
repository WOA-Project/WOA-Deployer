using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer.Core;
using Optional;
using Optional.Unsafe;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.UI;

namespace Deployer.Wpf
{
    public class DeployerFileOpenService
    {
        private readonly ISettingsService settingsService;
        private readonly IOpenFilePicker openFileService;

        public DeployerFileOpenService(ISettingsService settingsService, IOpenFilePicker openFileService)
        {
            this.settingsService = settingsService;
            this.openFileService = openFileService;
        }

        public Task<Option<IZafiroFile>> Pick(string settingsKey, IEnumerable<FileTypeFilter> extensions)
        {
            return openFileService.Pick(extensions, () => (string)settingsService[settingsKey].ValueOrDefault(),
                s => settingsService[settingsKey] = ((object) s).Some());
        }

        public IObservable<IZafiroFile> Picks(string settingsKey, IEnumerable<FileTypeFilter> filter)
        {
            return openFileService.Picks(filter, () => (string) settingsService[settingsKey].ValueOrDefault(),
                s => settingsService[settingsKey] = ((object) s).Some());
        }
    }
}