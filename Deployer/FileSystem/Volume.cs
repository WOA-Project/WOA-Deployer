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
            Log.Verbose("Mounting volume {Volume}", this);
            var driveLetter = DiskApi.GetFreeDriveLetter();
            await DiskApi.AssignDriveLetter(this, driveLetter);

            await Observable.Defer(() => Observable.Return(UpdateLetter(driveLetter))).RetryWithBackoffStrategy();
        }

        private Unit UpdateLetter(char driveLetter)
        {
            try
            {
                if (!Directory.Exists($"{driveLetter}:"))
                {
                    throw new ApplicationException($"The letter driver letter '{driveLetter}' isn't available yet");
                }

                Letter = driveLetter;
                return Unit.Default;
            }
            catch (Exception)
            {
                Log.Verbose("Cannot get path for drive letter {DriveLetter} while mounting partition {Partition}",
                    driveLetter, this);
                throw;
            }
        }

        public Task<ICollection<DriverMetadata>> GetDrivers()
        {
            return DiskApi.GetDrivers(this);
        }

        public override string ToString()
        {
            var foo = Label ?? "No label";
            return $"'{foo}' - Partition {Partition}";
        }
    }
}