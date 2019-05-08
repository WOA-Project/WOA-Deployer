using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer.Console
{
    public class ConsolePrompt : IPrompt
    {
        public Task<Option> PickOptions(string markdown, IEnumerable<Option> options)
        {
            System.Console.WriteLine(
@"By continuing you are accepting the following license below.
If you decline it, press Control+C anytime during the deployment process.
" + markdown);
            return Task.FromResult(new Option("Accept", OptionValue.OK));
        }
    }
}