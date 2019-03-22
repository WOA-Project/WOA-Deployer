using System.Linq;
using CommonPartition =  Deployer.FileSystem.Partition;

namespace Deployer.FileSystem.Gpt
{
    public static class GptMixin
    {
        public static void RemoveExisting(this GptContext self, string name)
        {
            var chosen = self.Partitions.FirstOrDefault(x => x.Name == name);
            self.Delete(chosen);
        }

        public static Partition Get(this GptContext self, string name)
        {
            return self.Partitions.FirstOrDefault(x => x.Name == name);            
        }
        
        public static CommonPartition AsCommon(this Partition self, Disk disk)
        {
            var partition = new CommonPartition(disk)
            {
                Guid = self.PartitionGuid,
                Name = self.Name,                
                PartitionType = self.PartitionType,
                Number = self.Number,                
            };

            return partition;
        }
    }
}