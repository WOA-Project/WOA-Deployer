using System;
using System.Linq;
using Serilog;

namespace Deployer.Filesystem.Gpt
{
    public static class GptMixin
    {
        public static void RemoveExisting(this GptContext self, string name)
        {
            var chosen = self.Partitions.FirstOrDefault(x => string.Equals(x.Name ,name, StringComparison.OrdinalIgnoreCase));
            if (chosen != null)
            {
                Log.Verbose("Deleting {Partition}", chosen);
                self.Delete(chosen);
            }
        }

        public static Partition Get(this GptContext self, string name)
        {
            return self.Partitions.FirstOrDefault(x => x.Name == name);            
        }
    }
}