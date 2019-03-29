using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Serilog;

namespace Deployer.FileSystem
{
    public class Volume
    {
        public Volume(Partition partition)
        {
            Partition = partition;
        }

        public Partition Partition { get; set; }
        public ByteSize Size { get; set; }
        public string Label { get; set; }
        public char? Letter { get; set; }
        public string Root => Letter.HasValue ? $"{Letter}:\\" : null;

        public IDiskApi DiskApi => Partition.DiskApi;

        public async Task Mount()
        {
            Log.Verbose("Mounting {Volume}", this);

            if (Root != null)
            {
                Log.Verbose("{Volume} already mounted. Skipping.", this);
                return;
            }

            var driveLetter = DiskApi.GetFreeDriveLetter();
            await DiskApi.AssignDriveLetter(Partition, driveLetter);

            await Observable.Defer(() => Observable.Return(EnsurePathIsReady($@"{driveLetter}:\"))).RetryWithBackoffStrategy(5);
            Letter = driveLetter;
        }

        private static Unit EnsurePathIsReady(string path)
        {
            Log.Debug("Checking path {Path}", path);
            if (!Directory.Exists(path))
            {
                throw new ApplicationException($"The path '{path}' isn't ready yet");
            }

            Log.Debug($"The {path} is ready");

            return Unit.Default;
        }

        public Task<ICollection<DriverMetadata>> GetDrivers()
        {
            return DiskApi.GetDrivers(this);
        }

        public override string ToString()
        {
            var label = Label ?? "No label";
            return $"Volume '{label}' at {Partition}";
        }
    }
}