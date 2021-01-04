using System;
using System.Linq;
using MoreLinq.Extensions;
using Zafiro.Core;

namespace Deployer.Core.Console
{
    public class ConsoleProgressUpdater : IDisposable
    {
        private readonly IDisposable progressUpdater;

        public ConsoleProgressUpdater(IOperationProgress progress)
        {
            progressUpdater = progress.Progress.Subscribe(DisplayProgress);
        }

        public int Width { get; set; } = 50;

        private void DisplayProgress(Progress progress)
        {
            System.Console.CursorVisible = false;

            switch (progress)
            {
                case Done done:
                    System.Console.WriteLine();
                    break;
                case Percentage percentage:

                    DrawPercentageValue(percentage.Value);

                    break;
                case AbsoluteProgress<ulong> undefinedProgress:

                    DrawBytesValue(undefinedProgress.Value);

                    break;
                case Unknown unknown:
                    System.Console.WriteLine("[...]");

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(progress));
            }

          
        }

        private void DrawBytesValue(ulong bytes)
        {
            System.Console.WriteLine($"> {ByteSizeLib.ByteSize.FromBytes(bytes)} completed");
        }

        private void DrawPercentageValue(double progress)
        {
            var barLength = progress * Width;
            System.Console.CursorLeft = 0;
            System.Console.Write("[");
            var bar = new string(Enumerable.Range(1, (int)barLength).Select(_ => '=').ToArray());

            System.Console.Write(bar);

            var label = $@"{progress:P0}";
            System.Console.CursorLeft = (Width - label.Length) / 2;
            System.Console.Write(label);
            System.Console.CursorLeft = Width;
            System.Console.Write("]");

            System.Console.CursorVisible = true;
        }

        public void Dispose()
        {
            progressUpdater?.Dispose();            
        }
    }
}