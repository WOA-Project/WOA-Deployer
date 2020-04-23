using System;
using System.Linq;

namespace Deployer.Core.Console
{
    public class ConsoleProgressUpdater : IDisposable
    {
        private readonly IDisposable progressUpdater;

        public ConsoleProgressUpdater(IOperationProgress progress)
        {
            progressUpdater = progress.Percentage.Subscribe(DisplayProgress);
        }

        public int Width { get; set; } = 50;

        private void DisplayProgress(double progress)
        {
            System.Console.CursorVisible = false;

            if (double.IsNaN(progress) || double.IsInfinity(progress))
            {
                System.Console.WriteLine();
                return;
            }

            var barLength = double.IsInfinity(progress) ? 0 : progress * Width;
            System.Console.CursorLeft = 0;
            System.Console.Write("[");
            var bar = new string(Enumerable.Range(1, (int) barLength).Select(_ => '=').ToArray());
            
            System.Console.Write(bar);
            
            var label = $@"{progress:P0}";
            System.Console.CursorLeft = (Width -label.Length) / 2;
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