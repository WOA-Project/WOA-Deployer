using System;

namespace Deployer.FileSystem.Gpt
{
    public struct GptSegment
    {
        public GptSegment(ulong start, ulong length)
        {
            if (length <= 0)
            {
                throw new InvalidOperationException($"Invalid GPT length: {length}");
            }

            Start = start;
            Length = length;
        }

        public ulong Start { get; }
        public ulong Length { get; }
        public ulong End => Start + Length + 1;

        public override string ToString()
        {
            return $"{Start} to {End} ({Length} sectors)";
        }
    }
}