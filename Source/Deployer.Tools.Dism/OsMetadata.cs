using System;

namespace Deployer.Tools.Dism
{
    public static class OsMetadata 
    {
        public static ProcessorArchitecture Architecture
        {
            get
            {
                var variable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") ?? throw new ArgumentNullException("variable");

                switch (variable.ToUpperInvariant())
                {
                    case "AMD64":
                        return ProcessorArchitecture.Amd64;
                    case "ARM64":
                        return ProcessorArchitecture.Arm64;
                    case "X86":
                        return ProcessorArchitecture.X86;

                    default:
                        throw new NotSupportedException($"Processor {variable} isn't supported");
                }
            }
        }
    }
}