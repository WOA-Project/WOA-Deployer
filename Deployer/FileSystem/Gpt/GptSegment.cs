namespace Deployer.FileSystem.Gpt
{
    public struct GptSegment
    {
        public GptSegment(ulong start, ulong length)
        {
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