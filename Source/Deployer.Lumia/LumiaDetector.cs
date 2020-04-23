using System;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.FileSystem;

namespace Deployer.Lumia
{
    public class LumiaDetector : IDetector
    {
        private readonly IFileSystem fileSystem;
        private readonly IPhoneModelInfoReader reader;

        public LumiaDetector(IFileSystem fileSystem, IPhoneModelInfoReader reader)
        {
            this.fileSystem = fileSystem;
            this.reader = reader;
        }

        public async Task<Device> Detect()
        {
            var disk = await Detection.GetDisk(fileSystem);
            if (disk != null)
            {
                var model = reader.GetPhoneModel(disk.Number);

                switch (model.Model)
                {
                    case PhoneModel.Cityman:
                        switch (model.Variant)
                        {
                            case Variant.SingleSim:
                                return Device.CitymanSs;
                            case Variant.DualSim:
                                return Device.CitymanDs;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                    case PhoneModel.Talkman:
                        switch (model.Variant)
                        {
                            case Variant.SingleSim:
                                return Device.TalkmanSs;
                            case Variant.DualSim:
                                return Device.TalkmanDs;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                    case PhoneModel.Hapanero:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return null;
        }
    }
}